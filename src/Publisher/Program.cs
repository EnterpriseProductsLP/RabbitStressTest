using System;

namespace Publisher
{
    class Program
    {
        private static bool _keepRunning = true;

        private static MessagePublisher _messagePublisher;

        static void Main()
        {
            _messagePublisher = new MessagePublisher();
            _messagePublisher.Start();
            Console.CancelKeyPress += OnCancelKeyPress;

            while (_keepRunning)
            {
            }

            Console.WriteLine("Exited gracefully");
            Console.ReadLine();
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine("CTRL+C detected");
            Console.WriteLine("Stopping publisher");
            _messagePublisher.Stop();
            Console.WriteLine("Publisher stopped");
            _keepRunning = false;
        }
    }
}