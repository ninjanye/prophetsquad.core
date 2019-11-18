using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public class OddsImporter
    {
        private IProvider<MatchOdds> _oddsSource;
        private IStore<MatchOdds> _database;

        public OddsImporter(IStore<MatchOdds> database, IProvider<MatchOdds> source)
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
