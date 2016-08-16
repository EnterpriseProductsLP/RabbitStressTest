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
                c =>
                {
                    c.Host(
                        new Uri($"rabbitmq://{Configuration.ClusterName}"),
                        h =>
                        {
                            h.Username("test");
                            h.Password("test");
                            h.UseCluster(x =>
                            {
                                x.ClusterMembers = Configuration.ClusterMembers;
                            });
                        });
                    c.UseRetry(new IntervalRetryPolicy(new AllPolicyExceptionFilter(), new TimeSpan(0, 0, 1, 0)));
                    c.Durable = true;
                    c.PublisherConfirmation = true;
                    c.ReceiveEndpoint(queueName, e => { e.Consumer<TestEventConsumer>(); });
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
