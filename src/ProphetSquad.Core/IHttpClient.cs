using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetSquad.Core
{
    public interface IHttpClient
    {
        Task<T> Get<T>(string authToken, string endpoint) where T : class, new();
        Task<T> Post<T>(string endpoint, HttpContent httpContent) where T : class, new();
    }
}