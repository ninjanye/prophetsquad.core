    using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Databases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Mappers
{
    public class CompetitionMapper : IMapper<CompetitionResponse, IEnumerable<Data.Models.Competition>>
    {
        private readonly IRegionDatabase regionDb;

        public CompetitionMapper(IRegionDatabase regionDb)
        {
            this.regionDb = regionDb;
        }

        public async Task<IEnumerable<Data.Models.Competition>> MapAsync(CompetitionResponse source)
        {
            var competitions = new List<Data.Models.Competition>();
            foreach (var c in source.Competitions)
            {
                competitions.Add(new Data.Models.Competition
                {
                    OpenFootyId = c.Id,
                    Name = c.Name,
                    RegionId = await GetRegionId(c.Area)
                });
            }

            return competitions;
        }

        private async Task<int> GetRegionId(Area area)
        {
            regionDb.Save(new Region { Name = area.Name, Code = area.Id.ToString() });
            var region = await regionDb.Retrieve(area.Name);
            return region.Id;
        }
    }
}