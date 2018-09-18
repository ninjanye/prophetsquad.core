using System;

namespace ProphetSquad.Core.Matcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Updating odds...");            

            var matcher = new OddsMatcher(null, null, null);
            matcher.Synchronise();
        }
    }
}
