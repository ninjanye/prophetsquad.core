using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProphetSquad.Core.Updater
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
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            Database = Database.Configure(config);

            string username = Environment.GetEnvironmentVariable("bfUser");
            string password = Environment.GetEnvironmentVariable("bfPassword");
            if(username == null){
                Console.WriteLine("Credentials required for odds retrieval:");
                Console.Write("Username:");
                username = Console.ReadLine();
                Console.Write("Password:");
                password = Console.ReadLine();
            }
            Console.WriteLine($"User: {username}:{password}");
            BetfairUsername = username;
            BetfairPassword = password;
        }

        public Database Database { get; }        
        public string BetfairUsername { get; }
        public string BetfairPassword { get; }
    }

    internal class Database 
    {
        public static Database Configure(IConfigurationRoot config)
        {
            return new Database(config);                                    
        }

        private Database(IConfigurationRoot config)
        {
            var database = config.GetSection("Application").GetSection("Database");
            ConnectionString = database.GetValue<string>("ConnectionString");            
        }

        public string ConnectionString { get; }
    }
}
