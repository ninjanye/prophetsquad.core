using Newtonsoft.Json;
using ProphetSquad.Core.Data.Models.ApiFootball;
using ProphetSquad.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Providers.ApiFootball
{
    public class ApiFootballStandingProvider : IProvider<Data.Models.Standing>
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly string _apiToken;
        private readonly IMapper<StandingResponse, IEnumerable<Data.Models.Standing>> _mapper;
        private readonly IProvider<Data.Models.Competition> _competitionProvider;
        private readonly IThrottler _throttler;

        public ApiFootballStandingProvider(IHttpClientFactory httpClientFactory,
                                            string apiToken,
                                            IMapper<StandingResponse, IEnumerable<Data.Models.Standing>> mapper,
                                            IProvider<Data.Models.Competition> competitionProvider,
                                            IThrottler throttler)
        {
            _httpClientFactory = httpClientFactory;
            _apiToken = apiToken;
            _mapper = mapper;
            _competitionProvider = competitionProvider;
            _throttler = throttler;
        }

        public async Task<IEnumerable<Data.Models.Standing>> RetrieveAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api-football-v1.p.rapidapi.com");
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", _apiToken);

            var result = new List<Data.Models.Standing>();
            var competitions = await _competitionProvider.RetrieveAll();
            foreach (var competition in competitions)
            {
                string url = $"/v2/leagueTable/{competition.OpenFootyId}";
                var response = await client.GetStringAsync(url);
                _throttler.Wait();

                var webResult = JsonConvert.DeserializeObject<ApiResponse<StandingResponse>>(response);
                var mapped = await _mapper.MapAsync(webResult.Api);
                foreach (var map in mapped)
                {
                    map.SourceCompetitionId = competition.OpenFootyId;
                    map.CompetitionId = competition.Id;
                }
                result.AddRange(mapped);
            }

            return result;
        }

        public Task<Data.Models.Standing> RetrieveBySourceId(int id)
        {
            throw new NotImplementedException();
        }
    }
}