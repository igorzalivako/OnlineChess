using ChessClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChessClient.ViewModels;
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    string _username;

    [ObservableProperty]
    string _password;

    private readonly IApiService _apiService;

    public LoginViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    async Task Login()
    {
        var user = await _apiService.Login(Username, Password);
        if (user != null)
        {
            await Shell.Current.GoToAsync("//GamePage");
        }
    }
}