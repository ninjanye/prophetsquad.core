using AutoFixture;
using ProphetSquad.Core.Data.Models;
using ProphetSquad.Core.Data.Models.FootballDataApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProphetSquad.Core.Tests
{
    public class FootballDataProviderTests : IHttpClientFactory, IMapper<MatchResponse, IEnumerable<Data.Models.Fixture>>
    {
        private HttpClient client;
        private readonly DateTime from, to;
        private HttpRequestMessage request;
        private readonly AutoFixture.Fixture autoFixture;
        private FootballDataFixtureProvider provider;
        private bool mapperCalled;
        private IEnumerable<Data.Models.Fixture> _expectedResult;
        private readonly IEnumerable<Data.Models.Fixture> result;

        public FootballDataProviderTests()
        {
            autoFixture = new AutoFixture.Fixture();
            _expectedResult = autoFixture.CreateMany<Data.Models.Fixture>();
            provider = new FootballDataFixtureProvider(this, "api_token", this);
            from = DateTime.Today;
            to = from.AddDays(1);
            result = provider.Retrieve(from, to).Result;
        }

        [Fact]
        public void RetrieveReturnsFixtures()
        {
            Assert.IsAssignableFrom<IEnumerable<Data.Models.Fixture>>(result);
        }

        [Fact]
        public void RetrieveCallsOutToFooballDataApi()
        {
            var expected = new Uri("http://api.football-data.org");
            Assert.Equal(expected, client.BaseAddress);
        }

        [Fact]
        public void RetrieveCallMatchesEndpoint()
        {
            Assert.Contains("/v2/matches", request.RequestUri.AbsolutePath);
        }

        [Fact]
        public void RetrieveWithDates()
        {
            Assert.Contains($"dateFrom={from.ToString("yyyy-MM-dd")}", request.RequestUri.AbsoluteUri);
            Assert.Contains($"dateTo={to.ToString("yyyy-MM-dd")}", request.RequestUri.AbsoluteUri);
        }

        [Fact]
        public void RetrieveWithAuthToken()
        {
            Assert.True(request.Headers.Contains("X-Auth-Token"));
        }

        [Fact]
        public void ReturnsMappedResponse()
        {
            Assert.True(mapperCalled);
            Assert.Equal(_expectedResult, result);
        }

        HttpClient IHttpClientFactory.CreateClient(string name)
        {
            client = new HttpClient(new MockHttpHandler(this));
            return client;
        }

        public Task<IEnumerable<Data.Models.Fixture>> MapAsync(MatchResponse source)
        {
            mapperCalled = true;
            return Task.FromResult(_expectedResult);
        }

        private class MockHttpHandler : DelegatingHandler
        {
            private readonly FootballDataProviderTests _testRunner;

            public MockHttpHandler(FootballDataProviderTests testRunner)
            {
                _testRunner = testRunner;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _testRunner.request = request;

                var result = _testRunner.autoFixture.Create<MatchResponse>();

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new JsonContent(result);
                return Task.FromResult(response);
            }
        }
    }
}