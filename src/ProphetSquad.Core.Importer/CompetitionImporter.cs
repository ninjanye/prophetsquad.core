using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers;
using ProphetSquad.Core.Providers.ApiFootball;
using ProphetSquad.Core.Providers.FootballData;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Importer
{
    public static class CompetitionImporter
    {
        [FunctionName("CompetitionImporter")]
        public static async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log) //0 0 1 * * *
        {
            log.LogInformation($"[BEGIN] CompetitionImporter: {DateTime.Now}");
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();

            var settings = AppSettings.Configure(); 
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            using (var sqlConnection = new SqlConnection(settings.Database.ConnectionString))
            {
                sqlConnection.Open();
                var databaseConnection = new DatabaseConnection(sqlConnection);
                var regionDb = new RegionDatabase(databaseConnection);
                var mapper = new ApiFootballCompetitionMapper(regionDb);
                //var provider = new FootballDataCompetitionProvider(httpClientFactory, settings.Api.AuthToken, mapper);
                var provider = new ApiFootballCompetitionProvider(httpClientFactory, settings.Api.AuthToken, mapper);

                var competitionDb = new CompetitionDatabase(databaseConnection);
                var competitions = await CompetitionCollection.RetrieveFrom(provider);
                log.LogInformation($"Retrieved {competitions.Count()} competitions");
                competitions.SaveTo(competitionDb);
            }
            log.LogInformation($"[COMPLETE] CompetitionImporter: {DateTime.Now}");
        }
    }
}
