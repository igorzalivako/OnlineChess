using ChessEngine;
using ChessLibrary.Models.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

public class GameService : IAsyncDisposable
{
    // События для клиента
    public event Action<MatchFoundDto> MatchFound;
    public event Action<ChessMove[]> FoundAvailableMoves;
    public event Action<ChessMove> AcceptMove;
    public event Action<string, List<ChessMove>> OpponentMove;
    public event Action<int> LeaveGame;
    public event Action<EndGameDto> EndGame;
    public event Action<string> ErrorOccurred;
    public event Action<string> QueueJoined;
    public event Action<string, bool> MoveVerified;
    public event Action<int, int> UpdateTimers;

    private HubConnection _hubConnection;
    private string _currentGameId;


    public GameService(string serverUrl, string jwtToken)
    {
        // Настройка подключения к хабу
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/gamehub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(jwtToken);
            })
            .WithAutomaticReconnect()
            .Build();

        // Подписка на события хаба
        _hubConnection.On<MatchFoundDto>("MatchFound", HandleMatchFound);
        _hubConnection.On<ChessMove[]>("FoundAvailableMoves", HandleFoundAvailableMoves);
        _hubConnection.On<ChessMove>("AcceptMove", HandleAcceptMove);
        _hubConnection.On<string, List<ChessMove>>("OpponentMove", HandleOpponentMove);
        _hubConnection.On<int>("LeaveGame", HandleLeaveGame);
        _hubConnection.On<EndGameDto>("EndGame", HandleEndGame);
        _hubConnection.On<string>("QueueJoined", HandleQueueJoined);
        _hubConnection.On<string, bool>("MoveVerified", HandleMoveVerification);
        _hubConnection.On<int, int>("UpdateTimers", HandleUpdateTimers);

        _hubConnection.Closed += HandleConnectionClosed;
    }

    private void HandleUpdateTimers(int whiteLeftTime, int blackLeftTime)
    {
        UpdateTimers?.Invoke(whiteLeftTime, blackLeftTime);
    }

    private void HandleQueueJoined(string obj)
    {
        QueueJoined?.Invoke($"prishlo {obj}");
    }

    public async Task ConnectAsync()
    {
        try
        {
            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Connection failed: {ex.Message}");
        }
    }

    // Основные методы взаимодействия

    public async Task JoinMatchmakingQueue(int gameModeMinutes)
    {
        try
        {
            await _hubConnection.InvokeAsync("JoinMatchmakingQueue", gameModeMinutes);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Join queue failed: {ex.Message}");
        }
    }

    public async Task GetAvailableMoves(string position)
    {
        try
        {
            await _hubConnection.InvokeAsync("GetAvailableMoves", position);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Get moves failed: {ex.Message}");
        }
    }

    public async Task ApplyMove(ChessMove move)
    {
        try
        {
            await _hubConnection.InvokeAsync("ApplyMove", _currentGameId, move);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Move failed: {ex.Message}");
        }
    }

    public async Task LeaveCurrentGame()
    {
        try
        {
            await _hubConnection.InvokeAsync("LeaveGame", _currentGameId);
            _currentGameId = null;
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Leave failed: {ex.Message}");
        }
    }

    // Обработчики событий хаба

    private void HandleMatchFound(MatchFoundDto match)
    {
        _currentGameId = match.GameId;
        MatchFound?.Invoke(match);

        // Автоматически присоединяемся к группе игры
        _ = _hubConnection.InvokeAsync("JoinGame", match.GameId);
    }

    private void HandleFoundAvailableMoves(ChessMove[] moves)
    {
        FoundAvailableMoves?.Invoke(moves);
    }

    private void HandleAcceptMove(ChessMove move)
    {
        AcceptMove?.Invoke(move);
    }

    private void HandleOpponentMove(string newFenPosition, List<ChessMove> possibleMoves)
    {
        OpponentMove?.Invoke(newFenPosition, possibleMoves);
    }

    private void HandleLeaveGame(int userId)
    {
        LeaveGame?.Invoke(userId);
        _currentGameId = null;
    }

    private void HandleEndGame(EndGameDto endGameDto)
    {
        EndGame?.Invoke(endGameDto);
        //_currentGameId = null;
    }

    // Обработка разрыва соединения
    private Task HandleConnectionClosed(Exception ex)
    {
        ErrorOccurred?.Invoke(ex?.Message ?? "Connection closed");
        return Task.CompletedTask;
    }

    private Task HandleMoveVerification(string newFenPosition, bool isCorrect)
    {
        MoveVerified.Invoke(newFenPosition, isCorrect);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
    }

    public async Task EndTime()
    {
        try
        {
            await _hubConnection.InvokeAsync("EndTime", _currentGameId);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"EndTime failed: {ex.Message}");
        }
    }
}