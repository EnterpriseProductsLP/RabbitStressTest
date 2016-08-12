using System;
using System.Threading;

using Autofac;

using MassTransit;

namespace Publisher
{
    public class MessagePublisher
    {
        private readonly IBusControl _busControl;

        private readonly int _messageSize;

        private readonly ConnectHandle _publicationObserverHandle;

        private readonly object _stopLock = new object();

        private bool _stopped;

        private bool _stopping;

        public MessagePublisher()
        {
            var container = BuildContainer();
            _messageSize = Configuration.MessageSize;
            _busControl = container.Resolve<IBusControl>();
            _publicationObserverHandle = _busControl.ConnectPublishObserver(new SqlLoggingPublicationObserver());
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

        public void Start()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                _busControl.Start();
                var waiting = false;

                while (!Stopping)
                {
                    if (PublicationQueueManager.CanPublish)
                    {
                        if (waiting)
                        {
                            waiting = false;
                            Console.Clear();
                            Console.WriteLine("Publishing messages.");
                            Console.WriteLine($"Current depth: {PublicationQueueManager.QueueDepth}");
                        }

                        var eventName = $"Event {PublicationQueueManager.QueueDepth}";
                        var paylod = new string('*', _messageSize);
                        var testEvent = new TestEvent(eventName, paylod);
                        PublicationQueueManager.IncrementQueueDepth();
                        _busControl.Publish(testEvent, cancellationToken);
                    }
                    else
                    {
                        if (waiting)
                        {
                            Console.Clear();
                            Console.WriteLine("Waiting for queue to drain.");
                            Console.WriteLine($"Current depth: {PublicationQueueManager.QueueDepth}");
                            Thread.Sleep(10000);
                            continue;
                        }

                        waiting = true;
                    }
                }

                cancellationTokenSource.Cancel(true);

                Console.WriteLine($"Waiting for publication queue to drain.");
                while (PublicationQueueManager.QueueDepth > 0)
                {
                }

                Console.WriteLine($"Finished processing publication queue.");
                _publicationObserverHandle.Disconnect();
                _busControl.Stop();
            }
            finally
            {
                SetStopped();
            }
        }

        public void Stop()
        {
            lock (_stopLock)
            {
                _stopping = true;
            }
        }

        private static IBusControl BuildBusContol()
        {
            return Bus.Factory.CreateUsingRabbitMq(
                c =>
                {
                    c.Host(
                        new Uri("rabbitmq://localhost"),
                        h =>
                        {
                            h.Username("test");
                            h.Password("test");
                        });
                    c.Durable = true;
                });
        }

        private static IContainer BuildContainer()
        {
            var busControl = BuildBusContol();
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