using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using MassTransit;

namespace Publisher
{
    internal class MessagePublisher
    {
        private readonly IBusControl _busControl;

        private readonly int _messageSize;

        private ConnectHandle _publicationObserverHandle;

        private bool _running;

        private readonly IList<Task> _tasks;

        public MessagePublisher()
        {
            var container = BuildContainer();

            _busControl = container.Resolve<IBusControl>();
            _messageSize = Configuration.MessageSize;
            _tasks = new List<Task>();
        }

        public void Start()
        {
            _publicationObserverHandle = _busControl.ConnectPublishObserver(new SqlLoggingPublicationObserver());
            _busControl.Start();
            _running = true;
            ThreadStart threadStart = new ThreadStart(Publish);
            var publishThread = new Thread(threadStart);
            publishThread.Start();
        }

        public void Stop()
        {
            // Stop publishing
            _running = false;
            Thread.Sleep(1000);

            // Stop the bus
            _publicationObserverHandle.Disconnect();
            var stopTasks = _tasks.ToArray();
            Task.WaitAll(stopTasks);

            var stopTask = _busControl.StopAsync();
            stopTask.Wait();
        }

        private void Publish()
        {
            var waiting = false;

            while (_running)
            {
                if (PublicationQueueManager.CanPublish)
                {
                    if (waiting)
                    {
                        waiting = false;
                        Console.WriteLine("Queue depth below maximum threshold.  Resuming.");
                    }

                    var eventName = $"Event {PublicationQueueManager.QueueDepth}";
                    var paylod = new string('*', _messageSize);
                    var testEvent = new TestEvent(eventName, paylod);
                    PublicationQueueManager.IncrementQueueDepth();
                    var publishTask = _busControl.Publish(testEvent);
                    _tasks.Add(publishTask);
                }
                else
                {
                    if (waiting)
                    {
                        continue;
                    }

                    Console.WriteLine("Max queue value reached.  Waiting.");
                    waiting = true;
                }
            }
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
    }
}
