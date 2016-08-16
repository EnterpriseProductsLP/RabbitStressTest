using System.Data.SqlClient;
using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;

namespace Migrations
{
    public class MigrationRunnerBuilder
    {
        private readonly string _connectionString;

        public MigrationRunnerBuilder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MigrationRunner BuildMigrationRunner()
        {
            var migrationAssembly = Assembly.GetAssembly(typeof(MigrationRunnerBuilder));
            var runnerContext = new RunnerContext(new NullAnnouncer());
            var connection = new SqlConnection(_connectionString);
            var serverProcessor = new SqlServerProcessor(connection, new SqlServer2014Generator(), new NullAnnouncer(), new ProcessorOptions(), new SqlServerDbFactory());
            var migrationRunner = new MigrationRunner(migrationAssembly, runnerContext, serverProcessor);
            return migrationRunner;
        }
    }
}
