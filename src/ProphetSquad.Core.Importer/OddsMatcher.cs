using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ProphetSquad.Core.Importer
{
    public static class OddsMatcher
    {
        [FunctionName("OddsMatcher")]
        public static async Task Run([TimerTrigger("0 0 */2 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var settings = AppSettings.Configure();
            var fixtureDatabase = BuildFixtureDatabase(settings);
            var oddsDatabase = BuildOddsDatabase(settings);

            log.LogInformation("Synchronising fixtures with odds...");
            var matcher = new Core.OddsMatcher(fixtureDatabase, oddsDatabase, fixtureDatabase);
            await matcher.Synchronise();
        }

        private static FixtureDatabase BuildFixtureDatabase(AppSettings settings)
        {
            var sqlConnection = new SqlConnection(settings.Database.ConnectionString);
            var databaseConnection = new DatabaseConnection(sqlConnection);
            return new FixtureDatabase(databaseConnection);
        }

        private static OddsDatabase BuildOddsDatabase(AppSettings settings)
        {
            var sqlConnection = new SqlConnection(settings.Database.ConnectionString);
            var databaseConnection = new DatabaseConnection(sqlConnection);
            return new OddsDatabase(databaseConnection);
        }
    }
}
