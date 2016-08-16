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
            var announcer = new ConsoleAnnouncer();
            var connection = new SqlConnection(_connectionString);
            var migrationAssembly = Assembly.GetAssembly(typeof(MigrationRunnerBuilder));
            var migrationGenerator = new SqlServer2014Generator();
            var processorOptions = new ProcessorOptions();
            var dbFactory = new SqlServerDbFactory();
            var runnerContext = new RunnerContext(announcer);
            var serverProcessor = new SqlServerProcessor(connection, migrationGenerator, announcer, processorOptions, dbFactory);

            return new MigrationRunner(migrationAssembly, runnerContext, serverProcessor);
        }
    }
}
