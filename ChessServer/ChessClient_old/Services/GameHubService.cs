// Services/GameHubService.cs
using Microsoft.AspNetCore.SignalR.Client;
using ChessClient.Models;

namespace ChessClient.Services;

public interface IGameHubService
{
    Task Connect(string gameId);
    Task SendMove(string move);
    event Action<string> OnMoveReceived;
    event Action<string> OnGameEnded;
}

public class GameHubService : IGameHubService
{
    private HubConnection _hubConnection;
    private readonly IApiService _apiService;

    public event Action<string> OnMoveReceived;
    public event Action<string> OnGameEnded;

    public GameHubService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task Connect(string gameId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
            return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{AppSettings.SignalRHubUrl}?gameId={gameId}", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_apiService.AuthToken);
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string>("ReceiveMove", move => OnMoveReceived?.Invoke(move));
        _hubConnection.On<string>("GameEnded", reason => OnGameEnded?.Invoke(reason));

        await _hubConnection.StartAsync();
    }

    public async Task SendMove(string move)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
            await _hubConnection.SendAsync("MakeMove", move);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
            await _hubConnection.DisposeAsync();
    }
}