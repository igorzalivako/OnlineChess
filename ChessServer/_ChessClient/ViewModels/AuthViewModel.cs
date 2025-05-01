using ChessClient.Services;
using ChessServer.Models.DTO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isLoading;

        public AuthViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
            ShowLoginCommand = new Command(() => IsRegisterMode = false);
            ShowRegisterCommand = new Command(() => IsRegisterMode = true);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isRegisterMode;
        public bool IsRegisterMode
        {
            get => _isRegisterMode;
            set => SetProperty(ref _isRegisterMode, value);
        }

        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }
        public Command ShowLoginCommand { get; }
        public Command ShowRegisterCommand { get; }

        private async Task LoginAsync()
        {
            if (IsLoading) return;

            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password are required";
                return;
            }

            try
            {
                IsLoading = true;

                var loginDto = new UserLoginDto
                {
                    Username = Username,
                    Password = Password
                };

                var result = await _authService.LoginAsync(loginDto);

                if (result.Success)
                {
                    // Сохраняем токен и переходим на главный экран
                    await SecureStorage.SetAsync("jwt_token", result.Token);
                    await Shell.Current.GoToAsync("//main");
                }
                else
                {
                    ErrorMessage = result.Error ?? "Login failed";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RegisterAsync()
        {
            if (IsLoading) return;

            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password are required";
                return;
            }

            try
            {
                IsLoading = true;

                var registerDto = new UserRegisterDto
                {
                    Username = Username,
                    Password = Password
                };

                var result = await _authService.RegisterAsync(registerDto);

                if (result.Success)
                {
                    // После успешной регистрации автоматически входим
                    await LoginAsync();
                }
                else
                {
                    ErrorMessage = result.Error ?? "Registration failed";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
