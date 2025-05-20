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
        Task<LoginResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponse> RegisterAsync(UserRegisterDto registerDto);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{AppConfig.BaseUrl}/api/users/login", loginDto);
                return await HandleLoginResponse(response);
            }
            catch (Exception ex)
            {
                return new LoginResponseDto { Success = false, Error = ex.Message };
            }
        }

        private async Task<LoginResponseDto> HandleLoginResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response);
                return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            }
            var errorResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            return errorResponse ?? new LoginResponseDto { Success = false, Error = "Unknown error occurred" };
        }

        public async Task<AuthResponse> RegisterAsync(UserRegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{AppConfig.BaseUrl}/api/users/register", registerDto);
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

        ~AuthService()
        {
            _httpClient.Dispose();
        }
    }
}
