using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Contracts;

using MassTransit;
using MassTransit.Util;

namespace Receiver
{
    internal class TestEventConsumer : IConsumer<ITestEvent>
    {
        public Task Consume(ConsumeContext<ITestEvent> context)
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
                                                      context.Message
                                                      .ClientName
                                              };

                var messageIdParameter = new SqlParameter("messageId", SqlDbType.UniqueIdentifier)
                                             {
                                                 Value =
                                                     context
                                                     .MessageId
                                             };

                command.Parameters.Add(clientNameParameter);
                command.Parameters.Add(messageIdParameter);
                command.ExecuteNonQuery();
                connection.Close();

                return TaskUtil.Completed;
            }
        }

        private static SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
        }
    }
}