﻿using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using ClientLibrary.Helper;
using ClientLibrary.Services.Contracts;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementation
{
    public class UserAccountServices(GetHttpClient getHttpClient) : IUserAccountServices
    {
        public const string AuthUrl = "api/Authentication";
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error Occured");
            return await result.Content.ReadFromJsonAsync<GeneralResponse>()??
                new GeneralResponse(false, "Deserialization Failed");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error Occured");
            return await result.Content.ReadFromJsonAsync<LoginResponse>() ??
                new LoginResponse(false, "Deserialiization Failed");
        }
        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken refreshToken)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/refreshToken", refreshToken);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error Occured");
            return await result.Content.ReadFromJsonAsync<LoginResponse>() ??
                new LoginResponse(false, "Deserialization Failed");
        }

        public async Task<WeatherForecast[]> GetWeatherForecasts()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast");
            return result!;
            
        }
    }
}
