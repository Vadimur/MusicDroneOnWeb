using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MusicDrone.IntegrationTests.ControllerServicesTests.Helpers
{
    public static class RequestSender
    {
        public static async Task<HttpResponseMessage> SendTestRequest<T>(this HttpClient client, HttpMethod httpMethod, string url, T content) where T : class
        {
            using var requestMessage = new HttpRequestMessage(httpMethod, url)
            {
                Content = content == default ? null : new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);

            return response;
        }

        public static async Task<HttpResponseMessage> SendTestRequest(this HttpClient client, HttpMethod httpMethod, string url)
        {
            return await client.SendTestRequest<object>(httpMethod, url, default);
        }
    }
}
