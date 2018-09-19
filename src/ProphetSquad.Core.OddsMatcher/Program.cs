using System;
using System.Data.SqlClient;

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

        private static IFixturesDatabase BuildDatabase(AppSettings settings)
        {
            var sqlConnection = new SqlConnection(settings.Database.ConnectionString);
            var databaseConnection = new DatabaseConnection(sqlConnection);
            return new FixtureDatabase(databaseConnection);
        }        
    }
}
