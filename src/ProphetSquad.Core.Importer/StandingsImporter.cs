using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers;
using ProphetSquad.Core.Providers.FootballData;

namespace ProphetSquad.Core.Importer
{
    public static class StandingsImporter
    {
        [FunctionName("StandingsImporter")]
        public static async Task Run([TimerTrigger("0 45 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"[BEGIN] StandingsImporter: {DateTime.Now}");
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            var settings = AppSettings.Configure();
            using (var sqlConnection = new SqlConnection(settings.Database.ConnectionString))
            {
                sqlConnection.Open();
                var databaseConnection = new DatabaseConnection(sqlConnection);

                var competitionDb = new CompetitionDatabase(databaseConnection);
                var teamDb = new TeamDatabase(databaseConnection);

                var standingsMapper = new StandingsMapper();
                var standingsProvider = new FootballDataStandingProvider(httpClientFactory, settings.Api.AuthToken, standingsMapper, competitionDb, new FootballDataThrottler());

                var fixtureDb = new FixtureDatabase(databaseConnection);
                var standings = await StandingsCollection.RetrieveFrom(standingsProvider);
                var standingsDb = new StandingsDatabase(databaseConnection, teamDb, competitionDb);

                log.LogInformation($"Retrieved {standings.Count()} league standings");
                await standings.SaveTo(standingsDb, competitionDb, teamDb);
            }

            log.LogInformation($"[COMPLETE] StandingsImporter: {DateTime.Now}");
        }
    }
}
