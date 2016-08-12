using System;

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
                config =>
                    {
                        var host = config.Host(
                            new Uri("rabbitmq://localhost"), 
                            h =>
                                {
                                    h.Username("test");
                                    h.Password("test");
                                });
                        config.ReceiveEndpoint(host, queueName,
                            e =>
                                {
                                    e.Consumer<TestEventConsumer>();
                                });
                        config.Durable = true;
                    });

            return busControl;
        }
    }
}