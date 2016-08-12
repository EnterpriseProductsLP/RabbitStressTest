using System;
using System.Timers;

using Autofac;

using MassTransit;

namespace Publisher
{
    internal class MessagePublisher
    {
        private readonly IBusControl _busControl;

        private readonly Timer _timer;

        private readonly int _messageSize;

        public MessagePublisher()
        {
            var container = BuildContainer();

            _busControl = container.Resolve<IBusControl>();
            _messageSize = Configuration.MessageSize;
            _timer = new Timer(Configuration.CounterInterval);
        }

        public void Start()
        {
            _busControl.ConnectPublishObserver(new SqlLoggingPublicationObserver());

            _busControl.Start();
            _timer.Elapsed += Publish;
            _timer.Start();
        }

        public void Stop()
        {
            // Stop publishing
            _timer.Stop();
            _timer.Elapsed -= Publish;

            // Stop the bus
            var stopTask = _busControl.StopAsync();
            stopTask.Wait();
        }

        private void Publish(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            if (PublicationQueueManager.CanPublish)
            {
                var eventName = $"Event {PublicationQueueManager.QueueDepth}";
                var paylod = new string('*', _messageSize);
                var testEvent = new TestEvent(eventName, paylod);
                Console.Out.WriteLineAsync($"Queueing {eventName}");
                _busControl.Publish(testEvent);
            }
            else
            {
                Console.Out.WriteLineAsync("Max queue value reached.  Waiting.");
            }
            _timer.Start();
        }

        private static IContainer BuildContainer()
        {
            var busControl = BuildBusContol();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(busControl).As<IBusControl>();

            return builder.Build();
        }

        private static IBusControl BuildBusContol()
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
                        config.Durable = true;
                    }
                );

            return busControl;
        }
    }
}