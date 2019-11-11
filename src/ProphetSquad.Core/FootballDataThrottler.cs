using System.Threading;

namespace ProphetSquad.Core
{
    /// <summary>
    /// Conservatively waits 8 seconds so as to ensure we don't 
    /// exceed the limits imposed by foorball-data api
    /// </summary>
    public class FootballDataThrottler : IThrottler
    {
        public void Wait()
        {
            Thread.Sleep(8000);
        }
    }
}