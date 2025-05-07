using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ChessClient.Models;
using ChessLibrary.Models.DTO;
using ChessServer.Models.DTO;

namespace ChessClient.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponse> RegisterAsync(UserRegisterDto registerDto);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5054"; 

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/users/login", loginDto);
                return await HandleLoginResponse(response);
            }
            catch (Exception ex)
            {
                return new LoginResponse { Success = false, Error = ex.Message };
            }
        }

        private async Task<LoginResponse> HandleLoginResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response);
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }
            var errorResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return errorResponse ?? new LoginResponse { Success = false, Error = "Unknown error occurred" };
        }

        public async Task<AuthResponse> RegisterAsync(UserRegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/users/register", registerDto);
                return await HandleRegisterResponse(response);
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Error = ex.Message };
            }
        }

        private async Task<AuthResponse> HandleRegisterResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response);
                return await response.Content.ReadFromJsonAsync<AuthResponse>();
            }
            var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return errorResponse ?? new AuthResponse { Success = false, Error = "Unknown error occurred" };
        }
    }
}
