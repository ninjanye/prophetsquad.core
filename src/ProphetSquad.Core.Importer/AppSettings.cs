using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProphetSquad.Core.Importer
{
    internal class AppSettings
    {
        private static AppSettings _options;

        public static AppSettings Configure()
        {
            if(_options != null)
            {
                return _options;
            }

            _options = new AppSettings();
            return _options;
        }

        private AppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            Database = Database.Configure(config);
            Api = Api.Configure(config);

            string username = Environment.GetEnvironmentVariable("bfUser");
            string password = Environment.GetEnvironmentVariable("bfPassword");
            if (username == null)
            {
                Console.WriteLine("Credentials required for odds retrieval:");
                Console.Write("Username:");
                username = Console.ReadLine();
                Console.Write("Password:");
                password = Console.ReadLine();
            }
            BetfairUsername = username;
            BetfairPassword = password;

        }

        public Database Database { get; }
        public Api Api { get; set; }
        public string BetfairUsername { get; }
        public string BetfairPassword { get; }
    }

    internal class Api
    {
        public static Api Configure(IConfigurationRoot config)
        {
            return new Api(config);
        }

        private Api(IConfigurationRoot config)
        {
            string apiToken = Environment.GetEnvironmentVariable("apiToken");
            if (apiToken == null)
            {
                Console.WriteLine("Api Token required for competitions retrieval:");
                Console.Write("ApiToken:");
                apiToken = Console.ReadLine();
            }
            AuthToken = apiToken;
        }

        public string AuthToken { get; }
    }

    internal class Database 
    {
        public static Database Configure(IConfigurationRoot config)
        {
            return new Database(config);                                    
        }

        private Database(IConfigurationRoot config)
        {
            ConnectionString = config.GetConnectionString("SqlConnectionString");
        }

        public string ConnectionString { get; }
    }
}
