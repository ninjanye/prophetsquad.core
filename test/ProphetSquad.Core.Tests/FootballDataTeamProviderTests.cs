using AutoFixture;
using ProphetSquad.Core;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Mappers;
using ProphetSquad.Core.Providers;
using ProphetSquad.Core.Providers.FootballData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Matcher.Tests
{
    public class TeamDataProviderFixture : IHttpClientFactory, IMapper<TeamResponse, IEnumerable<Core.Data.Models.Team>>, IProvider<Core.Data.Models.Competition>
    {
        private readonly AutoFixture.Fixture _autoFixture;
        private readonly FootballDataTeamProvider _provider;

        public TeamDataProviderFixture()
        {
            _autoFixture = new AutoFixture.Fixture();
            Competitions = _autoFixture.CreateMany<Core.Data.Models.Competition>();
            ExpectedResult = _autoFixture.CreateMany<Core.Data.Models.Team>();
            ApiCallTimes = new List<DateTime>();
            _provider = new FootballDataTeamProvider(this, "api_token", this, this);
            Client = new HttpClient(new MockHttpHandler(this));
            Result = _provider.RetrieveAll().Result;
        }

        public HttpRequestMessage Request { get; private set; }
        public IEnumerable<Core.Data.Models.Team> Result { get; }
        public IEnumerable<Core.Data.Models.Competition> Competitions { get; }
        public HttpClient Client { get; }
        public bool MapperCalled { get; private set; }
        public IEnumerable<Core.Data.Models.Team> ExpectedResult { get; }
        public bool CompetitionProviderCalled { get; private set; }
        public List<DateTime> ApiCallTimes { get; }

        HttpClient IHttpClientFactory.CreateClient(string name)
        {
            return Client;
        }

        async Task<IEnumerable<Core.Data.Models.Team>> IMapper<TeamResponse, IEnumerable<Core.Data.Models.Team>>.MapAsync(TeamResponse source)
        {
            MapperCalled = true;
            return await Task.FromResult(ExpectedResult);
        }

        async Task<IEnumerable<Core.Data.Models.Competition>> IProvider<Core.Data.Models.Competition>.RetrieveAll()
        {
            CompetitionProviderCalled = true;
            return await Task.FromResult(Competitions);
        }

        Task<Core.Data.Models.Competition> IProvider<Core.Data.Models.Competition>.RetrieveBySourceId(int id)
        {
            throw new NotImplementedException();
        }

        private class MockHttpHandler : DelegatingHandler
        {
            private readonly TeamDataProviderFixture _testRunner;

            public MockHttpHandler(TeamDataProviderFixture testRunner)
            {
                _testRunner = testRunner;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _testRunner.Request = request;
                _testRunner.ApiCallTimes.Add(DateTime.UtcNow);

                var result = _testRunner._autoFixture.Create<TeamResponse>();

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new JsonContent(result);
                return Task.FromResult(response);
            }
        }
    }

    public class FootballDataTeamProviderTests : IClassFixture<TeamDataProviderFixture>
    {
        private readonly TeamDataProviderFixture _fixture;

        public FootballDataTeamProviderTests(TeamDataProviderFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void RetrieveReturnsTeams()
        {
            Assert.IsAssignableFrom<IEnumerable<Core.Data.Models.Team>>(_fixture.Result);
        }

        [Fact]
        public void RetrieveCallsOutToFooballDataApi()
        {
            var expected = new Uri("http://api.football-data.org");
            Assert.Equal(expected, _fixture.Client.BaseAddress);
        }

        [Fact]
        public void RetrieveCallMatchesEndpoint()
        {
            Assert.Contains("/v2/competitions", _fixture.Request.RequestUri.AbsolutePath);
            Assert.Contains("/teams", _fixture.Request.RequestUri.AbsolutePath);
        }

        [Fact]
        public void RetrieveWithAuthToken()
        {
            Assert.True(_fixture.Request.Headers.Contains("X-Auth-Token"));
        }

        [Fact]
        public void ReturnsAllMappedResponses()
        {
            Assert.True(_fixture.MapperCalled);
            Assert.Equal(_fixture.ExpectedResult.Concat(_fixture.ExpectedResult).Concat(_fixture.ExpectedResult), _fixture.Result);
        }

        [Fact]
        public void CompetitionProviderCalled()
        {
            Assert.True(_fixture.CompetitionProviderCalled);
        }

        [Fact]
        public void GetTeamsForEachCompetition()
        {
            Assert.Equal(_fixture.Competitions.Count(), _fixture.ApiCallTimes.Count);
        }

        [Fact]
        public void AllowsSufficentTimeBetweenApiCalls()
        {
            var sixSeconds = new TimeSpan(0, 0, 8);
            var minDifference = GetSmallestDiff(_fixture.ApiCallTimes);

            Assert.True(sixSeconds < minDifference, $"Minimum difference: {minDifference.TotalSeconds} seconds");
        }

        private TimeSpan GetSmallestDiff(List<DateTime> times)
        {
            TimeSpan minDifference = TimeSpan.MaxValue;
            for (int i = 1; i < times.Count; i++)
            {
                minDifference = new TimeSpan(Math.Min(minDifference.Ticks, Math.Abs(times[i - 1].Ticks - times[i].Ticks)));
            }

            return minDifference;
        }
    }
}