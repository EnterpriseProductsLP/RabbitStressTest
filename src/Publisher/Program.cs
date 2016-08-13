using System;
using System.Threading;

namespace Publisher
{
    class Program
    {
        private static MessagePublisher _messageMessagePublisher;

        static void Main()
        {
            Console.Title = "RabbitMQ Stress Test - Publisher";

            _messageMessagePublisher = new MessagePublisher();
            new Thread(_messageMessagePublisher.Start).Start();
            Console.CancelKeyPress += OnCancelKeyPress;

            while (!_messageMessagePublisher.Stopped)
            {
            }

            Console.WriteLine("Exited gracefully");
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine("CTRL+C detected");
            Console.WriteLine("Stopping publisher");
            _messageMessagePublisher.Stop();

            Console.WriteLine("Publisher stopped");
        }
    }
}
