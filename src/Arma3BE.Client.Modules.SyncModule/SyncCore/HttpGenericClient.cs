using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Arma3BE.Client.Modules.SyncModule.SyncCore
{
    public class HttpGenericClient : IHttpGenericClient
    {
        private readonly HttpClient _httpClient;

        public HttpGenericClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = new HttpClient();
        }

        public void SetHeader(string name, string value)
        {
            var headers = _httpClient.DefaultRequestHeaders;

            if (headers.Contains(name))
                headers.Remove(name);

            headers.Add(name, value);
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var str = await _httpClient.GetStringAsync(uri);
            return JsonConvert.DeserializeObject<T>(str);
        }

        public async Task<TK> PostAsync<T, TK>(string uri, T payload)
        {
            var content = new StringContent(JsonConvert.SerializeObject(payload),
                Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync(uri, content);
            resp.EnsureSuccessStatusCode();
            var str = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TK>(str);
        }

    }
}