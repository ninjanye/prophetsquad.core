using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public class OddsImporter
    {
        private IOddsProvider _oddsSource;
        private IOddsDatabase _database;

        public OddsImporter(IOddsDatabase database, IOddsProvider source)
        {
            _oddsSource = source;
            _database = database;
        }

        public async Task Import()
        {
            System.Console.WriteLine("Importing odds.....");
            var oddsCollection = await OddsCollection.RetrieveFrom(_oddsSource);
            oddsCollection.SaveTo(_database);
        }
    }

}
