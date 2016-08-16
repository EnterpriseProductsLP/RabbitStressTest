using System;
using System.Configuration;

namespace Publisher
{
    internal static class Configuration
    {
        private static string _clientName;

        private static string[] _clusterMembers;

        private static string _clusterName;

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
    }
}