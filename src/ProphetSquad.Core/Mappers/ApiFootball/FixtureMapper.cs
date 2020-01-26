using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.ApiFootball;
using ProphetSquad.Core.Databases;
using ProphetSquad.Core.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Mappers.ApiFootball
{
    public class FixtureMapper : IMapper<MatchResponse, IEnumerable<Fixture>>
    {
        private readonly IProvider<Data.Models.Competition> _competitionDb;
        private readonly IDatabase<Data.Models.Team> _teamDb;
        private readonly IGameweekDatabase _gameweekDb;

        public FixtureMapper(IProvider<Data.Models.Competition> competitionDb, IDatabase<Data.Models.Team> teamDb, IGameweekDatabase gameweekDb)
        {
            _competitionDb = competitionDb;
            _teamDb = teamDb;
            _gameweekDb = gameweekDb;
        }

        public async Task<IEnumerable<Fixture>> MapAsync(MatchResponse source)
        {
            var fixtures = new List<Fixture>();
            foreach (var match in source.Fixtures)
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
            var competition = await _competitionDb.RetrieveBySourceId(match.LeagueId);
            var homeTeam = await GetTeamOrCreate(match.HomeTeam);
            var awayTeam = await GetTeamOrCreate(match.AwayTeam);
            var gameweek = await _gameweekDb.Retrieve(DateTimeOffset.FromUnixTimeSeconds(match.EventDate).UtcDateTime);

            Data.Models.Team winner = match.GoalsHomeTeam > match.GoalsAwayTeam ? homeTeam :
                                      match.GoalsAwayTeam > match.GoalsHomeTeam ? awayTeam : null;

            if (homeTeam == null || awayTeam == null)
            {
                return null;
            }

            return new Fixture
            {
                OpenFootyId = match.Id,
                Date = new DateTime(match.EventDate, DateTimeKind.Utc),
                CompetitionId = competition.Id,
                Competition = competition,
                HomeTeam = homeTeam,
                HomeTeamId = homeTeam.Id,
                HomeTeamScore = match.GoalsHomeTeam ?? 0,
                AwayTeam = awayTeam,
                AwayTeamId = awayTeam.Id,
                AwayTeamScore = match.GoalsAwayTeam ?? 0,
                WinnerId = winner?.Id,
                GameweekId = gameweek.Id,
                IsResult = match.Status == "FT"
            };

            async Task<Data.Models.Team> GetTeamOrCreate(Data.Models.ApiFootball.Team team)
            {
                var result = await _teamDb.RetrieveBySourceId(team.Id);
                if (result == null)
                {
                    var dbTeam = new Data.Models.Team
                    {
                        OpenFootyId = team.Id,
                        Name = team.Name,
                        Badge = team.Logo
                    };
                    _teamDb.Save(dbTeam);                    
                }
                return await _teamDb.RetrieveBySourceId(team.Id);
            }
        }
    }
}