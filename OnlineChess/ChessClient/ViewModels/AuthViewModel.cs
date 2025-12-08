using ChessClient.Services;
using ChessServer.Models.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.ViewModels
{
    [QueryProperty(nameof(Mode), "mode")]
    public partial class AuthViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isLoading;
        private string _repeatedPassword;

        public AuthViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
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

        public string RepeatedPassword
        {
            get => _repeatedPassword;
            set => SetProperty(ref _repeatedPassword, value); 
        }

        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }

        private string _mode;
        public string Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                IsRegisterMode = value == "register"; // Обновляем состояние
            }
        }

        [RelayCommand]        
        private async Task ShowLogin()
        {
            ClearSession();
            await Shell.Current.GoToAsync("///AuthPage?mode=login");
        }

        [RelayCommand]
        private async Task ShowRegister()
        {
            ClearSession();
            await Shell.Current.GoToAsync("///AuthPage?mode=register");
        }

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
                    await Shell.Current.GoToAsync($"///MainPage?username={Username}&rating={result.Rating}");
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
            
            IsLoading = true;

            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Необходимо ввести пароль и логин";
                return;
            }
            else if (string.IsNullOrWhiteSpace(RepeatedPassword) || RepeatedPassword != Password)
            {
                ErrorMessage = "Пароли не совпадают";
            }
            else
            {
                try
                {
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
                        ErrorMessage = result.Error ?? "Ошибка регистрации";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Произошла ошибка: {ex.Message}";
                }
                finally
                {
                    IsLoading = false;
                }
            }
            IsLoading = false;
        }

        private void ClearSession()
        {
            Username = "";
            Password = "";
            RepeatedPassword = "";
            ErrorMessage = "";
        }
    }
}
