using System.Configuration;

namespace Receiver
{
    internal static class Configuration
    {
        private static string[] _clusterMembers;

        private static string _clusterName;

        private static string _queueName;

        public static string QueueName => _queueName ?? (_queueName = ConfigurationManager.AppSettings["QueueName"]);

        public static string[] ClusterMembers => _clusterMembers ?? (_clusterMembers = ConfigurationManager.AppSettings["ClusterMembers"].Split(','));

        public static string ClusterName => _clusterName ?? (_clusterName = ConfigurationManager.AppSettings["ClusterName"]);
    }
}