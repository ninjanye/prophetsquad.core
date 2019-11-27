using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ProphetSquad.Core.Collections;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Importer
{
    public static class PointsProcessorJob
    {
        // [FunctionName("PointsProcessor")]
        // public static async Task Run([TimerTrigger("0 0 2 * * *")]TimerInfo myTimer, ILogger log)        
        // {
        //     var settings = AppSettings.Configure();
        //     using var sqlConnection = new SqlConnection(settings.Database.ConnectionString);
        //     sqlConnection.Open();
        //     var databaseConnection = new DatabaseConnection(sqlConnection);
        //     var database = new BetDatabase(databaseConnection);
        //     var unprocessedBetProvider = new UnprocessedBetProvider(databaseConnection);
        //     // var processor = new BetProcessor(database); //TODO: <<<< Decorates BetDatabase
        //     var bets = await BetCollection.RetrieveFrom(unprocessedBetProvider);
        //     // bets.SaveTo(processor);
        //     await Task.Delay(100);
        // }        
    }
}
