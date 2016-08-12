﻿using System;
using System.Threading.Tasks;

using Contracts;

using MassTransit;
using MassTransit.Util;

namespace Receiver
{
    internal class TestEventConsumer : IConsumer<ITestEvent>
    {
        public Task Consume(ConsumeContext<ITestEvent> context)
        {
            Console.WriteLine($"Received event {context.Message.EventName}");
            return TaskUtil.Completed;
        }
    }
}