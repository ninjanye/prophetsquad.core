using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Matcher
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Updating odds...");

            var settings = AppSettings.Configure();
            var db = BuildDatabase(settings);

            var httpClient = new HttpClientWrapper();
            var authenticator = new BetfairAuthenticator(httpClient, settings.BetfairUsername, settings.BetfairPassword);
            var betfairClient = new BetfairClient(httpClient, authenticator);
            var oddsSource = new BetfairOddsProvider(betfairClient);

            var matcher = new OddsMatcher(db, oddsSource, db);

            await matcher.Synchronise();
        }

        private static FixtureDatabase BuildDatabase(AppSettings settings)
        {
            var sqlConnection = new SqlConnection(settings.Database.ConnectionString);
            var databaseConnection = new DatabaseConnection(sqlConnection);
            return new FixtureDatabase(databaseConnection);
        }        
    }
}
