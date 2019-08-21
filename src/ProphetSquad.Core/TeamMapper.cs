using ProphetSquad.Core.Data.Models.FootballDataApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public class TeamMapper : IMapper<TeamResponse, IEnumerable<Data.Models.Team>>
    {
        public async Task<IEnumerable<Data.Models.Team>> MapAsync(TeamResponse source)
        {
            var teams = new List<Data.Models.Team>();
            foreach (var c in source.Teams)
            {
                teams.Add(new Data.Models.Team
                {
                    OpenFootyId = c.Id,
                    Name = c.Name,
                    BookieName = c.ShortName,
                    Badge = c.CrestUrl
                });
            }

            return await Task.FromResult(teams);
        }
    }
}