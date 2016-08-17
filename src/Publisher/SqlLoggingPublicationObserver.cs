using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using MassTransit;
using MassTransit.Util;

namespace Publisher
{
    public class SqlLoggingPublicationObserver : IPublishObserver
    {
        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            InsertMessageInfo(context.MessageId);

            return TaskUtil.Completed;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            PublicationQueueManager.DecrementQueueDepth();

            return TaskUtil.Completed;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            PublicationQueueManager.DecrementQueueDepth();
            DeleteMessageInfo(context.MessageId);
            Console.Out.WriteLineAsync(exception.Message);

            return TaskUtil.Completed;
        }

        private static void DeleteMessageInfo(Guid? messageId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.spUnconsumedMessageDelete";
                var clientNameParameter = new SqlParameter("clientName", SqlDbType.VarChar, 100)
                                              {
                                                  Value =
                                                      Configuration
                                                      .ClientName
                                              };
                var messageIdParameter = new SqlParameter("messageId", SqlDbType.UniqueIdentifier) { Value = messageId };

                command.Parameters.Add(clientNameParameter);
                command.Parameters.Add(messageIdParameter);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private static SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
        }

        private static void InsertMessageInfo(Guid? messageId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.spUnconsumedMessageInsert";
                var clientNameParameter = new SqlParameter("clientName", SqlDbType.VarChar, 100)
                                              {
                                                  Value =
                                                      Configuration
                                                      .ClientName
                                              };
                var messageIdParameter = new SqlParameter("messageId", SqlDbType.UniqueIdentifier) { Value = messageId };

                command.Parameters.Add(clientNameParameter);
                command.Parameters.Add(messageIdParameter);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}