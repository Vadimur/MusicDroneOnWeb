using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace MusicDrone.IntegrationTests.Shared
{
    public static class OptionsStorage
    {
        public static string TestDatabaseConnectionString { get; }
        private static ILogger _logger;

        static OptionsStorage()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            _logger = loggerFactory.CreateLogger<object>();

            var config = new ConfigurationBuilder()
                .AddJsonFile("Configuration/testsSettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            TestDatabaseConnectionString = ReadConnectionStringFromEnvironemntVariables(config) ?? ReadConnectionStringFromFile(config);

            _logger.LogInformation($"TestDatabaseConnectionString: {TestDatabaseConnectionString}");

        }

        private static string ReadConnectionStringFromEnvironemntVariables(IConfigurationRoot config)
        {

            var env = Environment.GetEnvironmentVariable("TESTING_ENVIRONMENT");

            _logger.LogInformation($"TESTING ENVIRONMENT: {env}");
            if (string.IsNullOrEmpty(env) || env.Equals("CI") == false)
            {
                return null;
            }

            var server = config["DbServer"];
            var port = config["DbPort"];
            var user = config["DbUser"];
            var password = config["DbPassword"];
            var dbName = config["IdentityDatabase"];

            var connectionString = $"Data Source={server},{port}; Initial Catalog={dbName}; User Id={user}; Password={password};";
            _logger.LogInformation($"connectionString: {connectionString}");

            return connectionString;
        }

        private static string ReadConnectionStringFromFile(IConfigurationRoot config)
        {            
            var connectionString = config["ConnectionStrings:TestDatabase"];

            return connectionString;
        }
    }
}
