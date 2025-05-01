using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ChessClient.Models;
using ChessServer.Models.DTO;

namespace ChessClient.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponse> RegisterAsync(UserRegisterDto registerDto);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://your-server-url.com/api/auth"; // Замените на ваш URL

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponse> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/users/login", loginDto);
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Error = ex.Message };
            }
        }

        public async Task<AuthResponse> RegisterAsync(UserRegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/users/register", registerDto);
                return await HandleResponse(response);
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Error = ex.Message };
            }
        }

        private async Task<AuthResponse> HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>();
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return errorResponse ?? new AuthResponse { Success = false, Error = "Unknown error occurred" };
        }
    }
}
