using System;

namespace Receiver
{
    class Program
    {
        private static MessageReceiver _publisher;

        static void Main()
        {
            _publisher = new MessageReceiver();
            _publisher.Start();
            Console.CancelKeyPress += OnCancelKeyPress;
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Console.WriteLine("Stopping publisher");
            _publisher.Stop();
            Console.WriteLine("Publisher stopped");
        }
    }
}
