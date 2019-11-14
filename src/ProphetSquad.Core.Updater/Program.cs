using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers.Betfair;
using System.Data.SqlClient;

namespace ProphetSquad.Core.Updater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var settings = AppSettings.Configure();
            var database = BuildDatabase(settings);
            var httpClient = new HttpClientWrapper();
            var authenticator = new BetfairAuthenticator(httpClient, settings.BetfairUsername, settings.BetfairPassword);
            var oddsSource = new BetfairOddsProvider(httpClient, authenticator, new BetfairThrottler());
            var oddsImporter = new OddsImporter(database, oddsSource);

            oddsImporter.Import().Wait();
        }

        private static IOddsDatabase BuildDatabase(AppSettings settings)
        {
            var sqlConnection = new SqlConnection(settings.Database.ConnectionString);
            var oddsConnection = new DatabaseConnection(sqlConnection);
            var database = new OddsDatabase(oddsConnection);

            return database;
        }
    }
}
