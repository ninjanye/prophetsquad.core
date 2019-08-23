using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Mappers;

namespace ProphetSquad.Core.Importer
{
    public static class TeamImporter
    {
        [FunctionName("TeamImporter")]
        public static async Task Run([TimerTrigger("0 0 2 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"[BEGIN] TeamImporter: {DateTime.Now}");
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();

            var settings = AppSettings.Configure();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            using (var sqlConnection = new SqlConnection(settings.Database.ConnectionString))
            {
                var databaseConnection = new DatabaseConnection(sqlConnection);
                var competitionDatabase = new CompetitionDatabase(databaseConnection);

                var mapper = new TeamMapper();
                var provider = new FootballDataTeamProvider(httpClientFactory,
                                                            settings.Api.AuthToken,
                                                            mapper,
                                                            competitionDatabase);

                sqlConnection.Open();
                var teamDatabase = new TeamDatabase(databaseConnection);
                var teams = await TeamCollection.RetrieveFrom(provider);
                log.LogInformation($"Retrieved {teams.Count()} teams");
                teams.SaveTo(teamDatabase);
            }
            log.LogInformation($"[COMPLETE] TeamImporter: {DateTime.Now}");
        }
    }
}
