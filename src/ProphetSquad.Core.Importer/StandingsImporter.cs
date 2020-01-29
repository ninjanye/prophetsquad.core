using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers.ApiFootball;
using ProphetSquad.Core.Providers.ApiFootball;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Importer
{
    public static class StandingsImporter
    {
        [FunctionName("StandingsImporter")]
        public static async Task Run([TimerTrigger("0 45 */6 * * *")]TimerInfo myTimer, ILogger log) // 0 45 */6 * * *
        {
            log.LogInformation($"[BEGIN] StandingsImporter: {DateTime.Now}");
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            var settings = AppSettings.Configure();
            using (var sqlConnection = new SqlConnection(settings.Database.ConnectionString))
            {
                sqlConnection.Open();
                var databaseConnection = new DatabaseConnection(sqlConnection);

                var standingsMapper = new StandingsMapper();
                var topEuroCompetitions = new CompetitionDatabase(databaseConnection, new[] { 524, 565, 581, 582, 764, 770, 525, 526, 573, 574, 988, 754, 755, 771, 775, 776, 891, 902, 656, 766, 1264, 432, 577, 576, 357, 566, 571, 787, 1312, 1310 });
                var standingsProvider = new ApiFootballStandingProvider(httpClientFactory, settings.Api.AuthToken, standingsMapper, topEuroCompetitions, new ApiFootballThrottler());
                var standings = await StandingsCollection.RetrieveFrom(standingsProvider);

                var teamDb = new TeamDatabase(databaseConnection);
                var competitionDb = new CompetitionDatabase(databaseConnection);
                var standingsDb = new StandingsDatabase(databaseConnection, teamDb, competitionDb);

                log.LogInformation($"Retrieved {standings.Count()} league standings");
                await standings.SaveTo(standingsDb, competitionDb, teamDb);
            }

            log.LogInformation($"[COMPLETE] StandingsImporter: {DateTime.Now}");
        }
    }
}
