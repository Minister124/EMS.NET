using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using ClientLibrary.Helper;
using ClientLibrary.Services.Contracts;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementation
{
    public class UserAccountServices(GetHttpClient getHttpClient) : IUserAccountServices
    {
        public const string AuthUrl = "api/authentication";
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error Occured");
            return await result.Content.ReadFromJsonAsync<GeneralResponse>()??
                new GeneralResponse(false, "Deserialization Failed");
        }

        public Task<LoginResponse> SignInAsync(Login user)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> RefreshTokenAsync(RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<WeatherForecast[]> GetWeatherForecasts()
        {
            throw new NotImplementedException();
        }
    }
}
