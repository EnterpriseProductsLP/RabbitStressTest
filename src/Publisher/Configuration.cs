using System;
using System.Configuration;

namespace Publisher
{
    internal static class Configuration
    {
        private static string _clientName;

        private static int? _counterInterval;

        private static int? _maxQueueDepth;

        private static int? _messageSize;

        public static string ClientName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_clientName))
                {
                    _clientName = ConfigurationManager.AppSettings["ClientName"];
                }

                return _clientName;
            }
        }

        public static int CounterInterval
        {
            get
            {
                if (!_counterInterval.HasValue)
                {
                    _counterInterval = Convert.ToInt32(ConfigurationManager.AppSettings["CouterInterval"]);
                }

                return _counterInterval.Value;
            }
        }

        public static int MaxQueueDepth
        {
            get
            {
                if (!_maxQueueDepth.HasValue)
                {
                    _maxQueueDepth = Convert.ToInt32(ConfigurationManager.AppSettings["MaxQueueDepth"]);
                }

                return _maxQueueDepth.Value;
            }
        }

        public static int MessageSize
        {
            get
            {
                if (!_messageSize.HasValue)
                {
                    _messageSize = Convert.ToInt32(ConfigurationManager.AppSettings["MessageSize"]);
                }

                return _messageSize.Value;
            }
        }
    }
}