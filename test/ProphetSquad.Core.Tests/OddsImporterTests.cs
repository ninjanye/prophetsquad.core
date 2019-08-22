using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class OddsImporterTests : IOddsDatabase, IOddsProvider
    {
        bool _saveToDatabaseCalled;
        bool _oddsRetrieved;
        IEnumerable<MatchOdds> _oddsReturned;
        List<MatchOdds> _oddsSaved = new List<MatchOdds>();

        public OddsImporterTests()
        {
            var competition = new Models.Betfair.Response.Competition { Id = "compId", Name = "compName" };
            var homeTeam = new Models.Betfair.Response.Team {SelectionId = "home", Name = "homeTeamName", Metadata = new Metadata { Id = "123456"}, Odds = 3.5m};
            var awayTeam = new Models.Betfair.Response.Team {SelectionId = "away", Name = "awayTeamName", Metadata = new Metadata { Id = "987654"}, Odds = 12m};
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
                Assert.Contains(matchOdds, _oddsSaved);
            }
        }

        void IOddsDatabase.Save(MatchOdds odds)
        {
            _saveToDatabaseCalled = true;
            _oddsSaved.Add(odds);
        }

        Task<IEnumerable<MatchOdds>> IOddsProvider.RetrieveAsync()
        {
            _oddsRetrieved = true;
            return Task.FromResult(_oddsReturned);
        }
    }
}