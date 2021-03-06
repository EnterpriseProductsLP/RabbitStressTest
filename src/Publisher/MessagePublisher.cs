﻿using System;

using Autofac;

using Common;

using MassTransit;

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

        private static IBusControl BuildBusContol()
        {
            return Bus.Factory.CreateUsingRabbitMq(
                c =>
                    {
                        c.Host(
                            new Uri($"rabbitmq://{Configuration.ClusterName}"), 
                            h =>
                                {
                                    h.Username(Configuration.ClientUsername);
                                    h.Password(Configuration.ClientPassword);
                                    h.UseCluster(x =>
                                            {
                                                x.ClusterMembers = Configuration.ClusterMembers;
                                            });
                                });
                        c.Durable = true;
                        c.PublisherConfirmation = true;
                    });
        }

        private static IContainer BuildContainer()
        {
            var busControl = BuildBusContol();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(busControl).As<IBusControl>();

            return builder.Build();
        }
    }
}