using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers;

namespace ProphetSquad.Core.Importer
{
    public static class ResultsImporter
    {
        [FunctionName("ResultsImporter")]
        public static async Task Run([TimerTrigger("0 30 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"[BEGIN] ResultsImporter: {DateTime.Now}");
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            var settings = AppSettings.Configure();
            using (var sqlConnection = new SqlConnection(settings.Database.ConnectionString))
            {
                sqlConnection.Open();
                var databaseConnection = new DatabaseConnection(sqlConnection);

                IDatabase<Competition> competitionDb = new CompetitionDatabase(databaseConnection);
                IDatabase<Team> teamDb = new TeamDatabase(databaseConnection);
                IGameweekDatabase gameweekDb = new GameweekDatabase(databaseConnection);

                var fixtureMapper = new FixtureMapper(competitionDb, teamDb, gameweekDb);
                var footballDataProvider = new FootballDataFixtureProvider(httpClientFactory, settings.Api.AuthToken, fixtureMapper);

                var fixtureDb = new FixtureDatabase(databaseConnection);
                var today = DateTime.Today;
                var fixtures = await FixtureCollection.RetrieveFrom(footballDataProvider, today.AddDays(-10), today);
                log.LogInformation($"Retrieved {fixtures.Count()} results");
                fixtures.SaveTo(fixtureDb);
            }
            log.LogInformation($"[COMPLETE] ResultsImporter: {DateTime.Now}");
        }
    }
}
