namespace Common
{
    public abstract class Worker
    {
        private readonly object _stopLock = new object();

        private bool _stopped;

        private bool _stopping;

        public bool Stopping
        {
            get
            {
                lock (_stopLock)
                {
                    return _stopping;
                }
            }
        }

        public bool Stopped
        {
            get
            {
                lock (_stopLock)
                {
                    return _stopped;
                }
            }
        }

        public void Start()
        {
            try
            {
                PrepareWorkLoop();

                while (!Stopping)
                {
                    WorkLoop();
                }

                this.CleanupWorkLoop();
            }
            finally
            {
                SetStopped();
            }
        }

        public void Stop()
        {
            lock (_stopLock)
            {
                _stopping = true;
            }
        }

        protected abstract void CleanupWorkLoop();

        protected abstract void PrepareWorkLoop();

        protected abstract void WorkLoop();

        private void SetStopped()
        {
            lock (_stopLock)
            {
                _stopped = true;
            }
        }
    }
}