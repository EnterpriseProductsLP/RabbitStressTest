using System;

using Autofac;

using MassTransit;

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
                lock (this._stopLock)
                {
                    return this._stopped;
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
            var busControl = Bus.Factory.CreateUsingRabbitMq(
                cfg =>
                    {
                        cfg.Host(
                            new Uri("rabbitmq://localhost"),
                            h =>
                                {
                                    h.Username("test");
                                    h.Password("test");
                                });
                        cfg.ReceiveEndpoint(queueName, e => { e.Consumer<TestEventConsumer>(); });
                        cfg.Durable = true;
                    });

            return busControl;
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
