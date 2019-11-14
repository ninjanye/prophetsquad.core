using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Mappers;
using ProphetSquad.Core.Providers;

namespace ProphetSquad.Core.Providers.FootballData
{
    public class FootballDataStandingProvider : IProvider<Data.Models.Standing>
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly string _apiToken;
        private readonly IMapper<StandingResponse, IEnumerable<Data.Models.Standing>> _mapper;
        private readonly IProvider<Data.Models.Competition> _competitionProvider;
        private readonly IThrottler _throttler;

        public FootballDataStandingProvider(IHttpClientFactory httpClientFactory,
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
            client.BaseAddress = new Uri("http://api.football-data.org");
            client.DefaultRequestHeaders.Add("X-Auth-Token", _apiToken);

            var result = new List<Data.Models.Standing>();
            var competitions = await _competitionProvider.RetrieveAll();
            foreach (var competition in competitions)
            {
                string url = $"/v2/competitions/{competition.OpenFootyId}/standings";
                var response = await client.GetStringAsync(url);
                _throttler.Wait();

                var webResult = JsonConvert.DeserializeObject<StandingResponse>(response);
                var mapped = await _mapper.MapAsync(webResult);
                foreach (var map in mapped)
                {
                    map.SourceCompetitionId = competition.OpenFootyId;
                    map.CompetitionId = competition.Id;
                }
                result.AddRange(mapped);
            }

            return result;
        }
    }
}