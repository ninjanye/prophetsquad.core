using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Importer
{
    public static class PointsProcessorJob
    {
        [FunctionName("PointsProcessor")]
        public static async Task Run([TimerTrigger("0 0 2 * * *")]TimerInfo myTimer, ILogger log)        
        {
            // OPTION 1
            // var database = new BetsDatabase()
            // var bets = BetCollection.RetrieveFrom(IProvider<Bet> database) // Active bets only
            // await bets.Process(IProcessor<Bet> pointsProcessor);

            // // PROCESSOR
            // IProcessor<Bet> pointsProcessor = new PointsProcessor();
            // foreach (var bet in bets)
            // {
            //     bet.Process(pointsProcessor);
            // }
            // bets.SaveTo(database)

            // OPTION 2
            // var database = new UsersDatabase()
            // var users = UsersCollection.RetrieveFrom(IProvider<User> database)
            // users.Process(IProcesser<User> pointsProcessor)

            // // PROCESSOR
            // IProcessor<Bet> pointsProcessor = new PointsProcessor();
            // foreach (var user in users)
            // {
            //     var bets = betsDb.RetrieveFor(user);
            //     foreach (var bet in bets)
            //     {
            //         bet.Process(pointsProcessor);
            //     }
            //     bets.SaveTo(database)
            // }


            await Task.Delay(100);
        }        
    }
}
