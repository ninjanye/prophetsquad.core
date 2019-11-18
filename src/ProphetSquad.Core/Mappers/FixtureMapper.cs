using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Mappers
{
    public class FixtureMapper : IMapper<MatchResponse, IEnumerable<Fixture>>
    {
        private readonly IProvider<Data.Models.Competition> _competitionDb;
        private readonly IProvider<Data.Models.Team> _teamDb;
        private readonly IGameweekDatabase _gameweekDb;

        public FixtureMapper(IProvider<Data.Models.Competition> competitionDb, IProvider<Data.Models.Team> teamDb, IGameweekDatabase gameweekDb)
        {
            _competitionDb = competitionDb;
            _teamDb = teamDb;
            _gameweekDb = gameweekDb;
        }

        public async Task<IEnumerable<Fixture>> MapAsync(MatchResponse source)
        {
            var fixtures = new List<Fixture>();
            foreach (var match in source.Matches)
            {
                var fixture = await BuildFixtureAsync(match);
                if (fixture != null)
                {
                    fixtures.Add(fixture);
                }
            }

            return fixtures;
        }

        private async Task<Fixture> BuildFixtureAsync(Match match)
        {
            var competition = await _competitionDb.RetrieveBySourceId(match.Competition.Id);
            var homeTeam = await _teamDb.RetrieveBySourceId(match.HomeTeam.Id);
            var awayTeam = await _teamDb.RetrieveBySourceId(match.AwayTeam.Id);
            var gameweek = await _gameweekDb.Retrieve(match.Date);

            Data.Models.Team winner = match.Score.Winner == "HOME_TEAM" ? homeTeam :
                                      match.Score.Winner == "AWAY_TEAM" ? awayTeam : null;

            if (homeTeam == null || awayTeam == null)
            {
                return null;
            }

            return new Fixture
            {
                OpenFootyId = match.Id,
                Date = match.Date,
                CompetitionId = competition.Id,
                Competition = competition,
                HomeTeam = homeTeam,
                HomeTeamId = homeTeam.Id,
                HomeTeamScore = match.Score.FullTime.HomeTeam ?? 0,
                AwayTeam = awayTeam,
                AwayTeamId = awayTeam.Id,
                AwayTeamScore = match.Score.FullTime.AwayTeam ?? 0,
                WinnerId = winner?.Id,
                GameweekId = gameweek.Id,
                IsResult = match.Status == "FINISHED"
            };
        }
    }
}