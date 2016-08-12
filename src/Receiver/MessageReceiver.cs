using System;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using MassTransit;

namespace Receiver
{
    public class MessageReceiver
    {
        private readonly IBusControl _busControl;

        public MessageReceiver()
        {
            var container = BuildContainer();

            _busControl = container.Resolve<IBusControl>();
        }

        public void Start()
        {
            _busControl.Start();
        }

        public void Stop()
        {
            // Stop the bus
            var stopTask = _busControl.StopAsync();
            stopTask.Wait();
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
