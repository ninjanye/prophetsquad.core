using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ProphetSquad.Core
{
    internal class JsonContent : StringContent
    {
        private static JsonSerializerSettings _serializerSettings  = new JsonSerializerSettings{
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public JsonContent(object data) 
            : base(JsonConvert.SerializeObject(data, _serializerSettings), Encoding.UTF8, "application/json")
        {
        }
    }
}