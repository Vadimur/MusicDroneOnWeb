using Microsoft.Extensions.Configuration;
using System;

namespace MusicDrone.IntegrationTests.Shared
{
    public static class OptionsStorage
    {
        public static string TestDatabaseConnectionString { get; }

        static OptionsStorage()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("Configuration/testsSettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            TestDatabaseConnectionString = ReadConnectionStringFromEnvironemntVariables(config) ?? ReadConnectionStringFromFile(config);      
        }

        private static string ReadConnectionStringFromEnvironemntVariables(IConfigurationRoot config)
        {
            var env = Environment.GetEnvironmentVariable("TESTING_ENVIRONMENT");

            if (string.IsNullOrEmpty(env) || env.Equals("CI") == false)
            {
                return null;
            }

            var server = config["DbServer"];
            var user = config["DbUser"];
            var password = config["DbPassword"];
            var dbName = config["IdentityDatabase"];

            var connectionString = $"Server={server};Database={dbName};User={user};Password={password};";

            return connectionString;
        }

        private static string ReadConnectionStringFromFile(IConfigurationRoot config)
        {            
            var connectionString = config["ConnectionStrings:TestDatabase"];

            return connectionString;
        }
    }
}
