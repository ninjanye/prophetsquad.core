using Newtonsoft.Json;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Providers.FootballData
{
    public class FootballDataFixtureProvider : IFixtureProvider
    {
        private IHttpClientFactory httpClientFactory;
        private readonly string apiToken;
        private readonly IMapper<MatchResponse, IEnumerable<Fixture>> mapper;

        public FootballDataFixtureProvider(IHttpClientFactory httpClientFactory, string apiToken, IMapper<MatchResponse, IEnumerable<Fixture>> mapper)
        {
            this.httpClientFactory = httpClientFactory;
            this.apiToken = apiToken;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to)
        {
            string url = $"/v2/matches?dateFrom={from.ToString("yyyy-MM-dd")}&dateTo={to.ToString("yyyy-MM-dd")}";
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://api.football-data.org");
            client.DefaultRequestHeaders.Add("X-Auth-Token", apiToken);
            var response = await client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<MatchResponse>(response);

            return await mapper.MapAsync(result);
        }
    }
}