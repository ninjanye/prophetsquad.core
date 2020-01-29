using ProphetSquad.Core.Data.Models.ApiFootball;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Mappers.ApiFootball
{
    public class StandingsMapper : IMapper<StandingResponse, IEnumerable<Data.Models.Standing>>
    {
        public async Task<IEnumerable<Data.Models.Standing>> MapAsync(StandingResponse source)
        {
            var standing = source.Standings.FirstOrDefault();
            if (standing != null)
            {
                var rankings = standing.Select(BuildRanking).ToList();
                return await Task.FromResult(rankings);
            }

            return new List<Data.Models.Standing>();
        }

        private static Data.Models.Standing BuildRanking(Standing standing)
        {
            return new Data.Models.Standing
            {
                SourceTeamId = standing.TeamId,
                Played = standing.All.MatchesPlayed,
                Wins = standing.All.Win,
                Losses = standing.All.Lose,
                Draws = standing.All.Draw,
                GoalsFor = standing.All.GoalsFor,
                GoalsAgainst = standing.All.GoalsAgainst,
                Points = standing.Points
            };
        }
    }
}