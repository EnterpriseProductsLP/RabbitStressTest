﻿using System;

namespace Publisher
{
    class Program
    {
        private static MessagePublisher _messagePublisher;

        static void Main()
        {
            _messagePublisher = new MessagePublisher();
            _messagePublisher.Start();
            Console.CancelKeyPress += OnCancelKeyPress;
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Console.WriteLine("Stopping publisher");
            _messagePublisher.Stop();
            Console.WriteLine("Publisher stopped");
        }
    }
}
