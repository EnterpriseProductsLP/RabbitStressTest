using System.Data.SqlClient;
using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace Migrations
{
    public class MigrationRunnerBuilder
    {
        private readonly string _connectionString;

        public MigrationRunnerBuilder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IMigrationRunner BuildMigrationRunner()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(builder =>
                {
                    var assemblies = typeof(MigrationRunnerBuilder).Assembly;
                    builder.AddSqlServer2014()
                        .WithGlobalConnectionString(_connectionString)
                        .WithMigrationsIn(assemblies);
                })
                .BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            }
        }
    }
}
