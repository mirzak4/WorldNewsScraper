using Application.TheGuardianClient;
using System.Net.Http.Headers;

namespace Infrastructure.TheGuardianClient
{
    public class TheGuardianClient : ITheGuardianClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public TheGuardianClient()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
        }

        public async Task<string> GetStringAsync(string url)
        {
            return await _httpClient.GetStringAsync(url);
        }
    }
}
