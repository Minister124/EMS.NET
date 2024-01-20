using BaseLibrary.DTOs;
using ClientLibrary.Services.Contracts;
using System.Net;
using System.Net.Http.Headers;

namespace ClientLibrary.Helper
{
    public class CustomHttpHandler(LocalStorageService localStorageService, GetHttpClient getHttpClient, IUserAccountServices userAccountServices) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool login = request.RequestUri!.AbsoluteUri.Contains("login");
            bool register = request.RequestUri!.AbsoluteUri.Contains("register");
            bool refreshToken = request.RequestUri!.AbsoluteUri.Contains("refreshToken");

            if (login || register || refreshToken)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var result = await base.SendAsync(request, cancellationToken);
            if (result.StatusCode == HttpStatusCode.Unauthorized) 
            {
                //Get token from localstorage
                var stringToken = await localStorageService.GetToken();
                if (stringToken == null) 
                {
                    return result;
                }

                //checking if the header contains the token
                string token = string.Empty;
                try
                {
                    token = request.Headers.Authorization!.Parameter!;
                }
                catch { }

                var deserializedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
                if (deserializedToken is null) return result;

                if(string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                    return await base.SendAsync(request, cancellationToken);
                }

                //call the refresh token from local storage
                var newJwtToken = await GetNewRefreshToken(deserializedToken.RefreshToken!);
                if (string.IsNullOrEmpty(newJwtToken)) return result;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newJwtToken);
                return await base.SendAsync(request, cancellationToken);
            }
            return result;
        }

        private async Task<string> GetNewRefreshToken(string token)
        {
            var output = await userAccountServices.RefreshTokenAsync(new RefreshToken()
            {
                Token = token
            });
            string serializedToken = Serializations.SerializeObj(new UserSession()
            {
                Token = output.Token,
                RefreshToken = output.RefreshToken
            });
            await localStorageService.SetToken(serializedToken);
            return output.Token;
        }
    }
}
