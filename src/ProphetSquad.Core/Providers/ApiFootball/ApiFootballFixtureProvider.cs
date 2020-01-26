using Newtonsoft.Json;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.ApiFootball;
using ProphetSquad.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Providers.FootballData
{
    public class ApiFootballFixtureProvider : IFixtureProvider
    {
        private IHttpClientFactory httpClientFactory;
        private readonly string apiToken;
        private readonly IMapper<MatchResponse, IEnumerable<Fixture>> mapper;

        public ApiFootballFixtureProvider(IHttpClientFactory httpClientFactory, string apiToken, IMapper<MatchResponse, IEnumerable<Fixture>> mapper)
        {
            this.httpClientFactory = httpClientFactory;
            this.apiToken = apiToken;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Fixture>> Retrieve(DateTime from, DateTime to)
        {
            // string url = $"/v2/matches?dateFrom={from.ToString("yyyy-MM-dd")}&dateTo={to.ToString("yyyy-MM-dd")}";
            //TODO: for each date in range - get fixtures
            string url = $"/v2/fixtures/date/{from.ToString("yyyy-MM-dd")}";
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api-football-v1.p.rapidapi.com");
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiToken);
            var response = await client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<ApiResponse<MatchResponse>>(response);

            return await mapper.MapAsync(result.Api);
        }
    }
}