using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ProphetSquad.Core.Importer
{
    public static class StandingsImporter
    {
        [FunctionName("StandingsImporter")]
        public static void Run([TimerTrigger("0 0 3 * * *")]TimerInfo myTimer, ILogger log)
        {
            // Retrieve standings

            // Save/Update db

        }
    }
}
