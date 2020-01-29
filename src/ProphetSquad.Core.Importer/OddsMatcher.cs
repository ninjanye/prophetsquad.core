//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Logging;
//using ProphetSquad.Core.Databases;
//using ProphetSquad.Core.Providers.Betfair;
//using System;
//using System.Data.SqlClient;
//using System.Threading.Tasks;

//namespace ProphetSquad.Core.Importer
//{
//    public static class OddsMatcher
//    {
//        [FunctionName("OddsMatcher")]
//        public static async Task Run([TimerTrigger("0 30 */2 * * *")]TimerInfo myTimer, ILogger log)
//        {
//            log.LogInformation($"[BEGIN] OddsMatcher: {DateTime.Now}");
//            var settings = AppSettings.Configure();
//            var fixtureDatabase = BuildFixtureDatabase(settings);
//            var oddsDatabase = BuildOddsDatabase(settings);

//            var matcher = new Core.OddsMatcher(fixtureDatabase, oddsDatabase, fixtureDatabase);
//            await matcher.Synchronise(log);
//            log.LogInformation($"[COMPLETE] OddsMatcher: {DateTime.Now}");

//        }

//        [FunctionName("OddsImporter")]
//        public static async Task RunImporter([TimerTrigger("0 0 */3 * * *")]TimerInfo myTimer, ILogger log)
//        {
//            log.LogInformation($"[BEGIN] OddsImporter: {DateTime.Now}");
//            var settings = AppSettings.Configure();
//            var database = BuildOddsDatabase(settings);
//            var httpClient = new HttpClientWrapper();
//            var authenticator = new BetfairAuthenticator(httpClient, settings.BetfairUsername, settings.BetfairPassword);
//            var oddsSource = new BetfairOddsProvider(httpClient, authenticator, new BetfairThrottler());
//            var oddsImporter = new OddsImporter(database, oddsSource);

//            await oddsImporter.Import();
//            log.LogInformation($"[COMPLETE] OddsImporter: {DateTime.Now}");

//        }

//        private static FixtureDatabase BuildFixtureDatabase(AppSettings settings)
//        {
//            var sqlConnection = new SqlConnection(settings.Database.ConnectionString);
//            var databaseConnection = new DatabaseConnection(sqlConnection);
//            return new FixtureDatabase(databaseConnection);
//        }

//        private static OddsDatabase BuildOddsDatabase(AppSettings settings)
//        {
//            var sqlConnection = new SqlConnection(settings.Database.ConnectionString);
//            var databaseConnection = new DatabaseConnection(sqlConnection);
//            return new OddsDatabase(databaseConnection);
//        }
//    }
//}
