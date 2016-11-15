using System;
using System.Configuration;
using System.Threading;
using Migrations;

namespace Publisher
{
    class Program
    {
        private static MessagePublisher _messageMessagePublisher;

        static void Main()
        {
            // Wait for the receiver to connect and get things set up.
            Thread.Sleep(5000);

            SetConsoleTitle();
            RunMigrations();
            StartMessagePublisher();
            RunUntilCancelKeyPress();
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine("CTRL+C detected");
            Console.WriteLine("Stopping publisher");
            _messageMessagePublisher.Stop();
        }

        private static void RunMigrations()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            var migrationRunnerBuilder = new MigrationRunnerBuilder(connectionString);
            var migrationRunner = migrationRunnerBuilder.BuildMigrationRunner();
            migrationRunner.MigrateUp();
        }

        private static void RunUntilCancelKeyPress()
        {
            Console.CancelKeyPress += OnCancelKeyPress;
            while (!_messageMessagePublisher.Stopped)
            {
            }

            Console.WriteLine("Publisher stopped");
            Console.WriteLine("Exited gracefully");
        }

        private static void SetConsoleTitle()
        {
            Console.Title = "RabbitMQ Stress Test - Publisher";
        }

        private static void StartMessagePublisher()
        {
            _messageMessagePublisher = new MessagePublisher();
            new Thread(_messageMessagePublisher.Start).Start();
        }
    }
}
