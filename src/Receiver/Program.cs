using System;

namespace Receiver
{
    class Program
    {
        private static bool _keepRunning = true;

        private static MessageReceiver _messageReceiver;

        static void Main()
        {
            _messageReceiver = new MessageReceiver();
            _messageReceiver.Start();
            Console.CancelKeyPress += OnCancelKeyPress;

            while (_keepRunning)
            {
            }

            Console.WriteLine("Exited gracefully");
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine("CTRL+C detected");
            Console.WriteLine("Stopping receiver");
            _messageReceiver.Stop();
            Console.WriteLine("Receiver stopped");
            _keepRunning = false;
        }
    }
}