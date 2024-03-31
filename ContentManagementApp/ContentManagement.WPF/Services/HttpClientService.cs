using ContentManagement.WPF.Services.Contracts;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ContentManagement.WPF.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClient HttpClient { get => _httpClient; }

        private readonly IConfiguration _config;

        public HttpClientService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            InitialiseClient();
        }

        private void InitialiseClient()
        {
            HttpClient.BaseAddress = new Uri(_config.GetSection("ApiBaseAddress").Value!);
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
