using System;

using Autofac;

using MassTransit;
using MassTransit.Policies;

namespace Receiver
{
    public class MessageReceiver
    {
        private readonly IBusControl _busControl;

        private readonly object _stopLock = new object();

        private bool _stopped;

        private bool _stopping;

        public MessageReceiver()
        {
            var container = BuildContainer();

            _busControl = container.Resolve<IBusControl>();
        }

        public bool Stopped
        {
            get
            {
                lock (_stopLock)
                {
                    return _stopped;
                }
            }
        }

        public bool Stopping
        {
            get
            {
                lock (_stopLock)
                {
                    return _stopping;
                }
            }
        }

        public void Start()
        {
            _busControl.Start();
        }

        public void Stop()
        {
            lock (_stopLock)
            {
                _stopping = true;
            }

            _busControl.Stop();
            SetStopped();
        }

        private static IBusControl BuildBusContol(string queueName)
        {
            return Bus.Factory.CreateUsingRabbitMq(
                rabbitMqBusFactoryConfigurator =>
                    {
                        var hostName = new Uri($"rabbitmq://{Configuration.ClusterName}/{Configuration.VirtualHost}");
                        rabbitMqBusFactoryConfigurator.Host(
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
                        rabbitMqBusFactoryConfigurator.ReceiveEndpoint(queueName, receiveEndpointConfigurator => { receiveEndpointConfigurator.Consumer<TestEventConsumer>(); });
                    });
        }

        private static IContainer BuildContainer()
        {
            var busControl = BuildBusContol(Configuration.QueueName);
            var builder = new ContainerBuilder();
            builder.RegisterInstance(busControl).As<IBusControl>();

            return builder.Build();
        }

        private void SetStopped()
        {
            lock (_stopLock)
            {
                _stopped = true;
            }
        }
    }
}
