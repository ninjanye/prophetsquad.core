using ProphetSquad.Core.Data.Models.FootballDataApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Mappers
{
    public class StandingsMapper : IMapper<StandingResponse, IEnumerable<Data.Models.Standing>>
    {
        public async Task<IEnumerable<Data.Models.Standing>> MapAsync(StandingResponse source)
        {
            var standing = source.Standings.FirstOrDefault(s => s.Stage == "REGULAR_SEASON" && s.Type == "TOTAL");
            if (standing != null)
            {
                var rankings = standing.Table.Select(BuildRanking);
                return await Task.FromResult(rankings);
            }

            return new List<Data.Models.Standing>();
        }

        private static Data.Models.Standing BuildRanking(Ranking ranking)
        {
            return new Data.Models.Standing {
                SourceTeamId = ranking.Team.Id,
                Played = ranking.PlayedGames,
                Wins = ranking.Won,
                Losses = ranking.Lost,
                Draws = ranking.Draw,
                GoalsFor = ranking.GoalsFor,
                GoalsAgainst = ranking.GoalsAgainst,
                Points = ranking.Points
            };
        }
    }
}