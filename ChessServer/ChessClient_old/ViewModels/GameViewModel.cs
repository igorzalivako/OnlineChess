// ViewModels/GameViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChessClient.Models;
using ChessClient.Services;

namespace ChessClient.ViewModels;

public partial class GameViewModel : ObservableObject, IDisposable
{
    private readonly IGameHubService _gameHub;
    private readonly IApiService _apiService;
    private IDispatcherTimer _timer;
    private string _gameId;
    private bool _isWhitePlayer;

    [ObservableProperty]
    private string _whiteTime = "05:00";

    [ObservableProperty]
    private string _blackTime = "05:00";

    [ObservableProperty]
    private string _currentFen;

    [ObservableProperty]
    private bool _isMyTurn;

    [ObservableProperty]
    private string _playerColor;

    public GameViewModel(IGameHubService gameHub, IApiService apiService)
    {
        _gameHub = gameHub;
        _apiService = apiService;
        InitializeGame();
    }

    private async void InitializeGame()
    {
        try
        {
            // 1. Получаем текущую игру от сервера
            var gameState = await _apiService.GetCurrentGame();
            _gameId = gameState.GameId;
            _isWhitePlayer = gameState.IsWhitePlayer;
            PlayerColor = _isWhitePlayer ? "Белые" : "Чёрные";

            // 2. Подключаемся к игре через SignalR
            await ConnectToGameHub();

            // 3. Инициализируем состояние
            CurrentFen = gameState.CurrentFen;
            IsMyTurn = gameState.IsMyTurn;

            // 4. Запускаем таймер
            StartGameTimer();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            await Shell.Current.GoToAsync("..");
        }
    }

    private async Task ConnectToGameHub()
    {
        _gameHub.OnMoveReceived += OnMoveReceived;
        _gameHub.OnGameStateUpdated += OnGameStateUpdated;
        _gameHub.OnGameEnded += OnGameEnded;

        await _gameHub.Connect(_gameId);
    }

    private void StartGameTimer()
    {
        _timer = Application.Current.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => UpdateTimers();
        _timer.Start();
    }

    private void UpdateTimers()
    {
        // Реальная логика обновления таймеров
        // Можно получать время с сервера для синхронизации
        if (IsMyTurn)
        {
            var timeSpan = TimeSpan.Parse(_isWhitePlayer ? WhiteTime : BlackTime);
            timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(1));

            if (_isWhitePlayer)
                WhiteTime = timeSpan.ToString(@"mm\:ss");
            else
                BlackTime = timeSpan.ToString(@"mm\:ss");
        }
    }

    private void OnMoveReceived(string move)
    {
        // Временное обновление до применения хода сервером
        CurrentFen = ApplyMoveLocally(CurrentFen, move);
        IsMyTurn = true;
    }

    private void OnGameStateUpdated(GameStateDto gameState)
    {
        // Официальное обновление состояния от сервера
        CurrentFen = gameState.CurrentPosition;
        WhiteTime = gameState.WhiteTime;
        BlackTime = gameState.BlackTime;
        IsMyTurn = gameState.IsMyTurn;
    }

    private void OnGameEnded(string reason)
    {
        _timer.Stop();
        Shell.Current.DisplayAlert("Игра окончена", reason, "OK");
    }

    [RelayCommand]
    private async Task MakeMove(string move)
    {
        try
        {
            IsMyTurn = false; // Оптимистичная блокировка UI

            // 1. Локальное применение хода (оптимистичное обновление)
            CurrentFen = ApplyMoveLocally(CurrentFen, move);

            // 2. Отправка на сервер
            await _gameHub.SendMove(move, CurrentFen);
        }
        catch
        {
            IsMyTurn = true;
            // Восстановление состояния при ошибке
            var gameState = await _apiService.GetGameState(_gameId);
            CurrentFen = gameState.CurrentFen;
        }
    }

    [RelayCommand]
    private async Task Resign()
    {
        bool success = await _apiService.ResignGame(_gameId);
        if (success) OnGameEnded("Вы сдались");
    }

    private string ApplyMoveLocally(string fen, string move)
    {
        // Упрощенная локальная логика применения хода
        // В реальном проекте используйте ChessEngine
        return fen; // Заглушка - должен быть реальный код
    }

    public void Dispose()
    {
        _timer?.Stop();
        _gameHub.OnMoveReceived -= OnMoveReceived;
        _gameHub.OnGameEnded -= OnGameEnded;
    }
}