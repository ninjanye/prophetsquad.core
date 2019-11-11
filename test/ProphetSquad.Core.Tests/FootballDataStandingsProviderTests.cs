using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using ProphetSquad.Core;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using ProphetSquad.Core.Mappers;
using Xunit;

namespace ProphetSquad.Matcher.Tests
{
    public class FootballDataStandingsProviderTests : IHttpClientFactory, IMapper<StandingResponse, IEnumerable<Core.Data.Models.Standing>>, IProvider<Core.Data.Models.Competition>, IThrottler
    {
        const int REQUEST_DELAY = 50;
        private readonly AutoFixture.Fixture _autoFixture;
        private readonly HttpClient _client;
        private readonly IEnumerable<Core.Data.Models.Standing> _result;
        private readonly List<DateTime> _apiCallTimes = new List<DateTime>();
        private readonly IEnumerable<Core.Data.Models.Competition> _competitions;
        private readonly IEnumerable<StandingResponse> _standings = new List<StandingResponse>();
        private readonly List<HttpRequestMessage> _requests = new List<HttpRequestMessage>();

        public FootballDataStandingsProviderTests()
        {
            _autoFixture = new AutoFixture.Fixture();
            _competitions = _autoFixture.CreateMany<Core.Data.Models.Competition>();
            _client = new HttpClient(new MockHttpHandler(this));
            var provider = new FootballDataStandingProvider(this, "api_token", this, this, this);
            _result = provider.RetrieveAll().Result;
        }

        [Fact]
        public void RetrieveReturnsStandings()
        {
            Assert.IsAssignableFrom<IEnumerable<Core.Data.Models.Standing>>(_result);
        }

        [Fact]
        public void RetrieveCallsOutToFooballDataApi()
        {
            var expected = new Uri("http://api.football-data.org");
            Assert.Equal(expected, _client.BaseAddress);
        }

        [Fact]
        public void RetrieveWithAuthToken()
        {
            Assert.True(_requests.First().Headers.Contains("X-Auth-Token"));
        }

        [Fact]
        public void RetrieveCallMatchesEndpoint()
        {
            Assert.Contains("/v2/competitions", _requests.First().RequestUri.AbsolutePath);
            Assert.Contains("/standings", _requests.First().RequestUri.AbsolutePath);
        }

        [Fact]
        public void MakesOneCallPerCompetition()
        {
            Assert.Equal(_competitions.Count(), _apiCallTimes.Count);
        }

        [Fact]
        public void AllCompetitionIdsUsedInUrls()
        {
            var urls = _requests.Select(x => x.RequestUri.AbsolutePath).ToList();
            foreach (var competition in _competitions)
            {
                var expectedUrl = $"v2/competitions/{competition.OpenFootyId}/standings";
                Assert.Contains(urls, url => url.Contains(expectedUrl));
            }
        }

        [Fact]
        public void AllowsSufficentTimeBetweenApiCalls()
        {
            var minDifference = GetSmallestDiff(_apiCallTimes);

            Assert.True(REQUEST_DELAY < minDifference.TotalMilliseconds, $"Minimum difference: {minDifference.TotalSeconds} seconds");
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
        HttpClient IHttpClientFactory.CreateClient(string name)
        {
            return _client;
        }

        async Task<IEnumerable<Core.Data.Models.Standing>> IMapper<StandingResponse, IEnumerable<Core.Data.Models.Standing>>.MapAsync(StandingResponse source)
        {

            return await Task.FromResult(_autoFixture.CreateMany<Core.Data.Models.Standing>());
        }

        async Task<IEnumerable<Core.Data.Models.Competition>> IProvider<Core.Data.Models.Competition>.RetrieveAll()
        {
            return await Task.FromResult(_competitions);
        }

        void IThrottler.Wait()
        {
            Thread.Sleep(REQUEST_DELAY);
        }

        private class MockHttpHandler : DelegatingHandler
        {
            private readonly FootballDataStandingsProviderTests _testRunner;

            public MockHttpHandler(FootballDataStandingsProviderTests testRunner)
            {
                _testRunner = testRunner;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _testRunner._requests.Add(request);
                _testRunner._apiCallTimes.Add(DateTime.UtcNow);

                var result = _testRunner._autoFixture.Create<StandingResponse>();

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new JsonContent(result);
                return Task.FromResult(response);
            }
        }
    }
}