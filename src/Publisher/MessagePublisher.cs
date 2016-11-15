using System;

using Autofac;

using Common;

using MassTransit;
using MassTransit.Policies;

namespace Publisher
{
    public class MessagePublisher : Worker
    {
        private readonly IBusControl _busControl;

        private readonly int _messageSize;

        private readonly ConnectHandle _publicationObserverHandle;

        public MessagePublisher()
        {
            var container = BuildContainer();
            _messageSize = Configuration.MessageSize;
            _busControl = container.Resolve<IBusControl>();
            _publicationObserverHandle = _busControl.ConnectPublishObserver(new SqlLoggingPublicationObserver());
        }

        protected override void Run()
        {
            _busControl.Start();

            while (!Stopping)
            {
                if (!PublicationQueueManager.CanPublish)
                {
                    continue;
                }

                var clientName = Configuration.ClientName;
                var eventName = $"Event {PublicationQueueManager.QueueDepth}";
                var paylod = new string('*', _messageSize);
                var testEvent = new TestEvent(clientName, eventName, paylod);
                PublicationQueueManager.IncrementQueueDepth();
                _busControl.Publish(testEvent);
            }

            Console.WriteLine("Waiting for publication queue to drain.");
            while (PublicationQueueManager.QueueDepth > 0)
            {
            }

            Console.WriteLine("Finished processing publication queue.");
            _publicationObserverHandle.Disconnect();
            _busControl.Stop();
        }

        private static IBusControl BuildBusContol(string queueName)
        {
            return Bus.Factory.CreateUsingRabbitMq(
                rabbitMqBusFactoryConfigurator =>
                    {
                        var hostName = new Uri($"rabbitmq://{Configuration.ClusterName}/test");
                        var host = rabbitMqBusFactoryConfigurator.Host(
                            hostName,
                            rabbitMqHostConfigurator =>
                                {
                                    rabbitMqHostConfigurator.Username(Configuration.ClientUsername);
                                    rabbitMqHostConfigurator.Password(Configuration.ClientPassword);
                                    rabbitMqHostConfigurator.UseCluster(
                                        rabbitMqClusterConfigurator =>
                                            {
                                                rabbitMqClusterConfigurator.ClusterMembers = Configuration.ClusterMembers;
                                            });
                                });

                        rabbitMqBusFactoryConfigurator.UseRetry(new IntervalRetryPolicy(new AllPolicyExceptionFilter(), new TimeSpan(0, 0, 0, 10)));
                        rabbitMqBusFactoryConfigurator.Durable = true;
                        rabbitMqBusFactoryConfigurator.PublisherConfirmation = true;
                    });
        }

        private static IContainer BuildContainer()
        {
            var busControl = BuildBusContol(Configuration.QueueName);
            var builder = new ContainerBuilder();
            builder.RegisterInstance(busControl).As<IBusControl>();

            return builder.Build();
        }
    }
}