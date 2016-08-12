using System;
using System.Threading.Tasks;

using MassTransit;
using MassTransit.Util;

namespace Publisher
{
    public class SqlLoggingPublicationObserver : IPublishObserver
    {
        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            // 1.  Save message ID to SQL Server
            // Console.Out.WriteLineAsync($"Publishing message: {context.MessageId}");

            return TaskUtil.Completed;
        }


        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            // 1.  Decrement queue depth

            // Console.Out.WriteLineAsync($"Published message: {context.MessageId}");
            PublicationQueueManager.DecrementQueueDepth();
            return TaskUtil.Completed;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            // 1.  Decrement queue depth
            // 2.  Delete message ID from SQL Server

            // Console.Out.WriteLineAsync($"Error publishing message: {context.MessageId}");
            PublicationQueueManager.DecrementQueueDepth();
            return TaskUtil.Completed;
        }
    }
}