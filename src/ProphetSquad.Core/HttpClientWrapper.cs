using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProphetSquad.Core
{
    public class HttpClientWrapper : IHttpClient
    {
        private static HttpClient httpClient = new HttpClient();
        const string AppKey = "4DN5h4OFpu7806Xa";

        public HttpClientWrapper()
        {
        }

        public Task<T> Get<T>(string authToken, string endpoint) where T : class, new()
        {
            return Task.FromResult(new T());
        }

        public async Task<T> Post<T>(string endpoint, HttpContent httpContent) where T : class, new()
        {
            httpContent.Headers.Add("X-Application", AppKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = await httpContent.ReadAsStringAsync();
            var response = await httpClient.PostAsync(endpoint, httpContent);
            var returnedData = await response.Content.ReadAsStringAsync();
            if(response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"REQUEST: {request}");
                Console.WriteLine($"STATUS: {response.StatusCode} RESPONSE: {returnedData}");                
                return null;
            }

            return JsonConvert.DeserializeObject<T>(returnedData);
        }
    }
}