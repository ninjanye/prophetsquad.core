using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.ApiFootball;
using ProphetSquad.Core.Databases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Mappers.ApiFootball
{
    public class CompetitionMapper : IMapper<LeaguesResponse, IEnumerable<Data.Models.Competition>>
    {
        private readonly IRegionDatabase regionDb;

        public CompetitionMapper(IRegionDatabase regionDb)
        {
            this.regionDb = regionDb;
        }

        public async Task<IEnumerable<Data.Models.Competition>> MapAsync(LeaguesResponse source)
        {
            var competitions = new List<Data.Models.Competition>();
            foreach (var c in source.Leagues)
            {
                competitions.Add(new Data.Models.Competition
                {
                    OpenFootyId = c.LeagueId,
                    Name = c.Name,
                    RegionId = await GetRegionId(c)
                });
            }

            return competitions;
        }

        private async Task<int> GetRegionId(League league)
        {
            regionDb.Save(new Region { Name = league.Country, Code = league.CountryCode });
            var region = await regionDb.Retrieve(league.Country);
            return region.Id;
        }
    }    
}