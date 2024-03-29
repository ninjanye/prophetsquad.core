using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers;
using ProphetSquad.Core.Providers;
using ProphetSquad.Core.Providers.FootballData;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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

                IProvider<Competition> competitionDb = new CompetitionDatabase(databaseConnection);
                IDatabase<Team> teamDb = new TeamDatabase(databaseConnection);
                IGameweekDatabase gameweekDb = new GameweekDatabase(databaseConnection);

                var fixtureMapper = new Mappers.ApiFootball.FixtureMapper(competitionDb, teamDb, gameweekDb);
                var fixtureProvider = new ApiFootballFixtureProvider(httpClientFactory, settings.Api.AuthToken, fixtureMapper);

                var fixtureDb = new FixtureDatabase(databaseConnection);
                var today = DateTime.Today;
                var fixtures = await FixtureCollection.RetrieveFrom(fixtureProvider, today.AddDays(-2), today);
                log.LogInformation($"Retrieved {fixtures.Count()} fixtures");
                fixtures.SaveTo(fixtureDb);
            }
            log.LogInformation($"[COMPLETE] ResultsImporter: {DateTime.Now}");
        }
    }
}
