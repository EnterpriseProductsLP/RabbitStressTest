using System.Configuration;

namespace Receiver
{
    internal static class Configuration
    {
        private static string _queueName;

        public static string QueueName
        {
            get
            {
                if (_queueName == null)
                {
                    _queueName = ConfigurationManager.AppSettings["QueueName"];
                }

                return _queueName;
            }
        }
    }
}