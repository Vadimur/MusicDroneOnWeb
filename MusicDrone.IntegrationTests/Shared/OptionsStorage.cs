using Microsoft.Extensions.Configuration;

namespace MusicDrone.IntegrationTests.Shared
{
    public static class OptionsStorage
    {
        public static string TestDatabaseConnectionString { get; }

        static OptionsStorage()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("Properties/launchsettings.json")
                .Build();
            TestDatabaseConnectionString = config["ConnectionStrings:TestDatabase"];      
        }
    }
}
