using System;
using System.Threading;

namespace Publisher
{
    public static class PublicationQueueManager
    {
        private static long _queueDepth;

        private static readonly int MaxQueueDepth;

        static PublicationQueueManager()
        {
            _queueDepth = 0;
            MaxQueueDepth = Configuration.MaxQueueDepth;
        }

        public static bool CanPublish => QueueDepth < MaxQueueDepth;

        public static void DecrementQueueDepth()
        {
            Interlocked.Decrement(ref _queueDepth);
        }

        public static void IncrementQueueDepth()
        {
            Interlocked.Increment(ref _queueDepth);
        }

        public static int QueueDepth => Convert.ToInt32(Interlocked.Read(ref _queueDepth));
    }
}