using System;
using System.Configuration;
using System.Threading;

using Migrations;

namespace Receiver
{
    class Program
    {
        private static MessageReceiver _messageMessageReceiver;

        static void Main()
        {
            SetConsoleTitle();
            RunMigrations();
            StartMessageReceiver();
            RunUntilCancelKeyPress();
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine("CTRL+C detected");
            Console.WriteLine("Stopping receiver");
            _messageMessageReceiver.Stop();

            Console.WriteLine("Receiver stopped");
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
            while (!_messageMessageReceiver.Stopped)
            {
            }

            Console.WriteLine("Exited gracefully");
        }

        private static void SetConsoleTitle()
        {
            Console.Title = "RabbitMQ Stress Test - Publisher";
        }

        private static void StartMessageReceiver()
        {
            _messageMessageReceiver = new MessageReceiver();
            new Thread(_messageMessageReceiver.Start).Start();
        }
    }
}
