using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class OddsImporterTests : IOddsDatabase, IOddsProvider
    {
        bool _saveToDatabaseCalled;
        bool _oddsRetrieved;
        IEnumerable<MatchOdds> _oddsReturned = new[]{new MatchOdds()};
        List<MatchOdds> _oddsSaved = new List<MatchOdds>();

        public OddsImporterTests()
        {
            var competition = new Competition{ Id = "compId", Name = "compName" };
            var homeTeam = new Team{SelectionId = "home", Name = "homeTeamName", Metadata = new Metadata { Id = "123456"}, Odds = 3.5m};
            var awayTeam = new Team{SelectionId = "away", Name = "awayTeamName", Metadata = new Metadata { Id = "987654"}, Odds = 12m};
            var source = new Market { Id = "id", Competition = competition, StartTime = DateTime.UtcNow, Teams = new[]{homeTeam, awayTeam} };
            _oddsReturned = new[]{MatchOdds.From(source)};
            var importer = new OddsImporter(this, this);

            importer.Import().Wait();                       
        }

        [Fact]
        public void OddsImporterSavesOddsToSuppliedDatabase()
        { 
            Assert.True(_saveToDatabaseCalled);
        }

        [Fact]
        public void OddsImporterRetrievesOddsFromSuppliedSource()
        {
            Assert.True(_oddsRetrieved);            
        }

        [Fact]
        public void OddsImporterSavesOddsRetrieved()
        {
            foreach (var matchOdds in _oddsReturned)
            {
                Assert.True(_oddsSaved.Contains(matchOdds));
            }
        }

        public void Save(MatchOdds odds)
        {
            _saveToDatabaseCalled = true;
            _oddsSaved.Add(odds);
        }

        public Task<IEnumerable<MatchOdds>> Retrieve()
        {
            _oddsRetrieved = true;
            return Task.FromResult(_oddsReturned);
        }
    }
}