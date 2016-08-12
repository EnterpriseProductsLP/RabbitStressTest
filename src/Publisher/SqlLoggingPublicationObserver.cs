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
            // 1.  Increment queue depth
            PublicationQueueManager.IncrementQueueDepth();

            // 2.  Save message ID to SQL Server

            return TaskUtil.Completed;
        }


        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            // 1.  Decrement queue depth
            PublicationQueueManager.DecrementQueueDepth();

            return TaskUtil.Completed;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            // 1.  Decrement queue depth
            PublicationQueueManager.DecrementQueueDepth();

            return TaskUtil.Completed;
            // 2.  Delete message ID from SQL Server
        }
    }
}