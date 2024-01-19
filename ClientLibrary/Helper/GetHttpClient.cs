using BaseLibrary.DTOs;
using System.Net.Http.Headers;

namespace ClientLibrary.Helper
{
    public class GetHttpClient(IHttpClientFactory httpFactory, LocalStorageService localStorageService)
    {
        private const string HeaderKey = "Authorization";

        public async Task<HttpClient> GetPrivateHttpClient()
        {
            var Client = httpFactory.CreateClient("ApplicationApiClient");
            var stringToken = await localStorageService.GetToken();
            if (string.IsNullOrEmpty(stringToken)) return Client;
            var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
            if (deserializeToken == null) return Client;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deserializeToken.Token);
            return Client;
        }

        public HttpClient GetPublicHttpClient()
        {
            var Client = httpFactory.CreateClient("ApplicationApiClient");
            Client.DefaultRequestHeaders.Remove(HeaderKey);
            return Client;
        }
    }
}
