using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessClient.ViewModels;

[QueryProperty(nameof(Username), "username")]
[QueryProperty(nameof(Rating), "rating")]
public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    private async Task ShowLogin()
    {
        await Shell.Current.GoToAsync("//AuthPage?mode=login");
    }

    [RelayCommand]
    private async Task ShowRegister()
    {
        await Shell.Current.GoToAsync("//AuthPage?mode=register");
    }

    [ObservableProperty]
    private bool _isTimePickerOpen;

    [ObservableProperty]
    private GameTimeOption _selectedTime;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _rating;

    // Этот метод будет вызван автоматически после изменения Username
    partial void OnUsernameChanged(string oldValue, string newValue)
    {
        if (newValue != "") 
        {
            IsLogged = true;
        }
    }

    [ObservableProperty]
    private bool _isLogged;

    public ObservableCollection<GameTimeOption> AvailableTimes { get; } = new()
    {
        new("1 минута", "dotnet_bot.png"),
        new("3 минуты", "dotnet_bot.png"),
        new("5 минут", "dotnet_bot.png"),
        new("10 минут", "dotnet_bot.png"),
        new("15 минут", "dotnet_bot.png")
    };


    [RelayCommand]
    private void ToggleTimePicker()
    {
        IsTimePickerOpen = !IsTimePickerOpen;
    }

    partial void OnSelectedTimeChanged(GameTimeOption value)
    {
        IsTimePickerOpen = false; // Закрыть список после выбора
        OnPropertyChanged(nameof(AvailableTimes)); // Обновить галочки
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBotGameSelected))]
    [NotifyPropertyChangedFor(nameof(IsOnlineGameSelected))]
    [NotifyPropertyChangedFor(nameof(IsLocalGameSelected))]
    private string _selectedGameMode = "None";

    [ObservableProperty]
    private int _botDifficulty = 5;

    public bool IsBotGameSelected => SelectedGameMode == "Bot";
    public bool IsOnlineGameSelected => SelectedGameMode == "Online";
    public bool IsLocalGameSelected => SelectedGameMode == "Local";

    [RelayCommand]
    private void SelectGameMode(string mode)
    {
        SelectedGameMode = mode;
    }

    [RelayCommand]
    private void Logout()
    {
        IsLogged = false;
    }

    [RelayCommand]
    private async Task NavigateToGame()
    {
        await Shell.Current.GoToAsync("///GamePage");
    }

    [RelayCommand]
    private async Task Login()
    {
        await Shell.Current.GoToAsync("///AuthPage?mode=login");
    }
}