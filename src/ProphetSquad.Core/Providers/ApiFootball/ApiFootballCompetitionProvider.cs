using Newtonsoft.Json;
using ProphetSquad.Core.Data.Models.ApiFootball;
using ProphetSquad.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Providers.ApiFootball
{
    public class ApiFootballCompetitionProvider : IProvider<Data.Models.Competition>
    {
        private IHttpClientFactory httpClientFactory;
        private readonly string apiToken;
        private readonly IMapper<LeaguesResponse, IEnumerable<Data.Models.Competition>> mapper;

        public ApiFootballCompetitionProvider(IHttpClientFactory httpClientFactory, string apiToken, IMapper<LeaguesResponse, IEnumerable<Data.Models.Competition>> mapper)
        {
            this.httpClientFactory = httpClientFactory;
            this.apiToken = apiToken;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Data.Models.Competition>> RetrieveAll()
        {
            string url = $"/v2/leagues/current/";
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api-football-v1.p.rapidapi.com");
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiToken); // caeb347971msh4ece883f526b701p1aceeajsnb19f9ea5ade4
            var response = await client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<ApiResponse<LeaguesResponse>>(response);

            return await mapper.MapAsync(result.Api);
        }

        public Task<Data.Models.Competition> RetrieveBySourceId(int id)
        {
            throw new NotImplementedException();
        }
    }
}