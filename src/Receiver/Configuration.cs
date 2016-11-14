using System.Configuration;

namespace Receiver
{
    internal static class Configuration
    {
        private static string _clientPassword;

        private static string _clientUsername;

        private static string[] _clusterMembers;

        private static string _clusterName;

        private static string _queueName;

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

        public static string QueueName => _queueName ?? (_queueName = ConfigurationManager.AppSettings["QueueName"]);
    }
}