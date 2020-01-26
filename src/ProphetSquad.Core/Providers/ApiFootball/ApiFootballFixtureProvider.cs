using Newtonsoft.Json;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.ApiFootball;
using ProphetSquad.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = new List<Fixture>();

            var days = EachDay(from, to).ToList();
            foreach (var date in days)
            {
                string url = $"/v2/fixtures/date/{date.ToString("yyyy-MM-dd")}";
                var client = httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://api-football-v1.p.rapidapi.com");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiToken);
                var response = await client.GetStringAsync(url);
                var apiResult = JsonConvert.DeserializeObject<ApiResponse<MatchResponse>>(response);

                result.AddRange(await mapper.MapAsync(apiResult.Api));
            }

            return result;
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for(var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }        
    }
}