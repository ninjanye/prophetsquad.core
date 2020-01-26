using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers;
using ProphetSquad.Core.Mappers.ApiFootball;
using ProphetSquad.Core.Providers;
using ProphetSquad.Core.Providers.FootballData;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Importer
{
    public static class FixtureImporter
    {
        [FunctionName("FixtureImporter")]
        public static async Task Run([TimerTrigger("0 2 * * * *")]TimerInfo myTimer, ILogger log) //0 0 */5 * * *
        {
            log.LogInformation($"[BEGIN] FixtureImporter: {DateTime.Now}");
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

                // var fixtureMapper = new FixtureMapper(competitionDb, teamDb, gameweekDb);
                // var footballDataProvider = new FootballDataFixtureProvider(httpClientFactory, settings.Api.AuthToken, fixtureMapper);
                var fixtureMapper = new Mappers.ApiFootball.FixtureMapper(competitionDb, teamDb, gameweekDb);
                var fixtureProvider = new ApiFootballFixtureProvider(httpClientFactory, settings.Api.AuthToken, fixtureMapper);

                var fixtureDb = new FixtureDatabase(databaseConnection);
                var today = DateTime.Today;
                var fixtures = await FixtureCollection.RetrieveFrom(fixtureProvider, today, today.AddDays(10));
                log.LogInformation($"Retrieved {fixtures.Count()} fixtures");
                fixtures.SaveTo(fixtureDb);
            }
            log.LogInformation($"[COMPLETE] FixtureImporter: {DateTime.Now}");
        }
    }
}
