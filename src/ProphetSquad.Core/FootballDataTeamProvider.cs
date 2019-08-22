using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Mappers;

namespace ProphetSquad.Core
{
    public class FootballDataTeamProvider : IProvider<Data.Models.Team>
    {
        private IHttpClientFactory httpClientFactory;
        private readonly string apiToken;
        private readonly IMapper<TeamResponse, IEnumerable<Data.Models.Team>> mapper;
        private readonly IProvider<Data.Models.Competition> competitionProvider;

        public FootballDataTeamProvider(IHttpClientFactory httpClientFactory,
                                        string apiToken,
                                        IMapper<TeamResponse, IEnumerable<Data.Models.Team>> mapper,
                                        IProvider<Data.Models.Competition> competitionProvider)
        {
            this.httpClientFactory = httpClientFactory;
            this.apiToken = apiToken;
            this.mapper = mapper;
            this.competitionProvider = competitionProvider;
        }

        public async Task<IEnumerable<Data.Models.Team>> RetrieveAll()
        {
            var competitions = await competitionProvider.RetrieveAll();

            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://api.football-data.org");
            client.DefaultRequestHeaders.Add("X-Auth-Token", apiToken);

            HttpResponseMessage response;
            var result = new List<Data.Models.Team>();
            foreach (var competition in competitions)
            {
                var url = $"/v2/competitions/{competition.OpenFootyId}/teams";

                response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var deserialized = JsonConvert.DeserializeObject<TeamResponse>(await response.Content.ReadAsStringAsync());
                    result.AddRange(await mapper.MapAsync(deserialized));
                    //Football api only allow 10 requests per minute
                    //Conservatively set a wait of 10 seconds to allow for other calls
                }
                await Task.Delay(10000);
            }

            return result;
        }
    }
}