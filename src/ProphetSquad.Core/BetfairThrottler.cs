using System.Threading;

namespace ProphetSquad.Core
{
    public class BetfairThrottler : IThrottler
    {
        public void Wait()
        {
            Thread.Sleep(1000);
        }
    }
}