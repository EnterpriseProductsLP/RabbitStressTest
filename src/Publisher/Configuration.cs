using System;
using System.Configuration;

namespace Publisher
{
    internal static class Configuration
    {
        private static string _clientName;

        private static string _clientPassword;

        private static string _clientUsername;

        private static string[] _clusterMembers;

        private static string _clusterName;

        private static int? _maxQueueDepth;

        private static int? _messageSize;

        private static string _queueName;
		
        private static string _virtualHost;

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

        public static string ClientPassword
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_clientPassword))
                {
                    _clientPassword = ConfigurationManager.AppSettings["ClientPassword"];
                }

                return _clientPassword;
            }
        }

        public static string ClientUsername
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_clientUsername))
                {
                    _clientUsername = ConfigurationManager.AppSettings["ClientUsername"];
                }

                return _clientUsername;
            }
        }

        public static string[] ClusterMembers => _clusterMembers ?? (_clusterMembers = ConfigurationManager.AppSettings["ClusterMembers"].Split(','));

        public static string ClusterName => _clusterName ?? (_clusterName = ConfigurationManager.AppSettings["ClusterName"]);

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

        public static string QueueName => _queueName ?? (_queueName = ConfigurationManager.AppSettings["QueueName"]);

        public static string VirtualHost => _virtualHost ?? (_virtualHost = ConfigurationManager.AppSettings["VirtualHost"]);
    }
}