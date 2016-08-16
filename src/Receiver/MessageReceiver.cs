using System;

using Autofac;

using Common;

using MassTransit;

namespace Receiver
{
    public class MessageReceiver : Worker
    {
        private readonly IBusControl _busControl;

        public MessageReceiver()
        {
            var container = BuildContainer();

            _busControl = container.Resolve<IBusControl>();
        }

        protected override void CleanupWorkLoop()
        {
            _busControl.Stop();
        }

        protected override void PrepareWorkLoop()
        {
            _busControl.Start();
        }

        protected override void WorkLoop()
        {
        }

        private static IContainer BuildContainer()
        {
            var busControl = BuildBusContol(Configuration.QueueName);
            var builder = new ContainerBuilder();
            builder.RegisterInstance(busControl).As<IBusControl>();

            return builder.Build();
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
    }
}
