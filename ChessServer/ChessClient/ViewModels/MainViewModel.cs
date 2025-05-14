using ChessClient.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessClient.ViewModels;

[QueryProperty(nameof(Username), "username")]
[QueryProperty(nameof(Rating), "rating")]
public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        SelectedTime = AvailableTimes[0];
        SelectedGameMode = "Bot";
    }

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

    partial void OnSelectedTimeChanged(GameTimeOption oldValue, GameTimeOption newValue)
    {
        // Сбрасываем выделение у всех элементов
        foreach (var time in AvailableTimes)
        {
            time.IsSelected = time == newValue;
        }
        IsTimePickerOpen = false;
    }

    [ObservableProperty]
    private bool _isLogged;

    public ObservableCollection<GameTimeOption> AvailableTimes { get; } = new()
    {
        new("Пуля 1 минута", "bullet.png"),
        new("Блиц 3 минуты", "blitz.png"),
        new("Блиц 5 минут", "blitz.png"),
        new("Рапид 10 минут", "rapid.png"),
        new("Рапид 30 минут", "rapid.png")
    };


    [RelayCommand]
    private void ToggleTimePicker()
    {
        IsTimePickerOpen = !IsTimePickerOpen;
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
        if (IsOnlineGameSelected)
        {
            if (IsLogged)
            {
                try
                {
                    using var client = new HttpClient
                    {
                        Timeout = TimeSpan.FromSeconds(3) // Таймаут 3 секунды
                    };

                    // Запрос к специальному эндпоинту для проверки
                    var response = await client.GetAsync($"{AppConfig.BaseUrl}/api/health");
                    if (!response.IsSuccessStatusCode)
                    {
                        var popup = new ErrorPopup("Проверьте подключение к интернету и повторите попытку", "Нет соединения с сервером", "", 300, 250);
                        await Application.Current.MainPage.ShowPopupAsync(popup);
                    }
                    else
                    {
                        // тут потом сделать передачу разного времени
                        await Shell.Current.GoToAsync($"///GamePage?username={Username}&rating={Rating}&game_mode=online&minutes=5");
                    }
                }
                catch
                {
                    var popup = new ErrorPopup("Проверьте подключение к интернету и повторите попытку", "Нет соединения с сервером", "", 300, 250);
                    await Application.Current.MainPage.ShowPopupAsync(popup);
                }
            }
            else
            {
                var popup = new ErrorPopup("Для онлайн игры войдите в профиль", "Выполните вход", "", 300, 200);
                await Application.Current.MainPage.ShowPopupAsync(popup);
            }
        }
        
    }

    [RelayCommand]
    private async Task Login()
    {
        await Shell.Current.GoToAsync("///AuthPage?mode=login");
    }
}