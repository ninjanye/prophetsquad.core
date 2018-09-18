using System;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Models.Betfair.Response;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class MatchOddsTests
    {
        [Fact]
        public void IsMappedCorrectly()
        {
            var competition = new Models.Betfair.Response.Competition { Id = "compId", Name = "compName" };
            var homeTeam = new Models.Betfair.Response.Team{SelectionId = "home", Name = "homeTeamName", Metadata = new Metadata { Id = "123456"}, Odds = 3.5m};
            var awayTeam = new Models.Betfair.Response.Team {SelectionId = "away", Name = "awayTeamName", Metadata = new Metadata { Id = "987654"}, Odds = 12m};
            var source = new Market { Id = "id", Competition = competition, StartTime = DateTime.UtcNow, Teams = new[]{homeTeam, awayTeam} };

            var result = MatchOdds.From(source);
            
            Assert.Equal(source.Id, result.Id);            
            Assert.Equal(competition.Id, result.CompetitionId);
            Assert.Equal(competition.Name, result.CompetitionName);
            Assert.Equal(source.StartTime, result.Date);
            Assert.Equal(homeTeam.Id, result.HomeTeamId);
            Assert.Equal(awayTeam.Id, result.AwayTeamId);
            Assert.Equal(homeTeam.Name, result.HomeTeamName);
            Assert.Equal(awayTeam.Name, result.AwayTeamName);
            Assert.Equal(homeTeam.Odds, result.HomeTeamOddsDecimal);
            Assert.Equal(awayTeam.Odds, result.AwayTeamOddsDecimal);
            Assert.Equal(OddsConverter.ToFractional(homeTeam.Odds), result.HomeTeamOdds);
            Assert.Equal(OddsConverter.ToFractional(awayTeam.Odds), result.AwayTeamOdds);
        }
    }
}