using System.Threading;

namespace ProphetSquad.Core
{
    public class ApiFootballThrottler : IThrottler
    {
        public void Wait()
        {
            Thread.Sleep(2000);
        }
    }
}