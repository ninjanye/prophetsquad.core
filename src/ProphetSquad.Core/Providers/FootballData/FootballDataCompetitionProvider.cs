using Newtonsoft.Json;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Providers.FootballData
{
    public class FootballDataCompetitionProvider : IProvider<Data.Models.Competition>
    {
        private IHttpClientFactory httpClientFactory;
        private readonly string apiToken;
        private readonly IMapper<CompetitionResponse, IEnumerable<Data.Models.Competition>> mapper;

        public FootballDataCompetitionProvider(IHttpClientFactory httpClientFactory, string apiToken, IMapper<CompetitionResponse, IEnumerable<Data.Models.Competition>> mapper)
        {
            this.httpClientFactory = httpClientFactory;
            this.apiToken = apiToken;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Data.Models.Competition>> RetrieveAll()
        {
            string url = $"/v2/competitions?plan=TIER_ONE";
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://api.football-data.org");
            client.DefaultRequestHeaders.Add("X-Auth-Token", apiToken);
            var response = await client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<CompetitionResponse>(response);

            return await mapper.MapAsync(result);
        }

        public Task<Data.Models.Competition> RetrieveBySourceId(int id)
        {
            throw new NotImplementedException();
        }
    }
}