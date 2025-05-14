using ChessLibrary.Models.DTO;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
/*
public class GameService : IAsyncDisposable
{
    public event Action<string> MatchFound;
    public event Action<string> GameState;
    public event Action<ChessMove> MoveReceived;
    public event Action<string> ErrorOccurred;

    private HubConnection _hubConnection;
    private HttpClient _httpClient;
    private string _currentGameId;

    public GameService(string serverUrl, string jwtToken)
    {
        // Настройка HTTP-клиента для REST API
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(serverUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        
        // Для SignalR
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/gamehub?access_token={jwtToken}", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(jwtToken);
                options.SkipNegotiation = true;
                options.Transports = HttpTransportType.WebSockets;
            })
            .WithAutomaticReconnect()
            .Build();
        

        // Подписка на события хаба
        _hubConnection.On<string>("MatchFound", HandleMatchFound);
        _hubConnection.On<string>("GameState", HandleGameState);
        _hubConnection.On<ChessMove>("MoveMade", HandleMoveReceived);
        
    }

    public async Task ConnectAsync()
    {
        await _hubConnection.StartAsync();
    }

    // Вход в очередь через REST API
    public async Task JoinQueue(int gameModeMinutes)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/games/join-queue",
                new JoinQueueRequestDto { GameModeMinutes = gameModeMinutes }
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ErrorOccurred?.Invoke($"Join queue failed: {error}");
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Join queue error: {ex.Message}");
        }
    }

    // Отправка хода через SignalR
    public async Task SendMove(ChessMove move)
    {
        if (string.IsNullOrEmpty(_currentGameId)) return;

        try
        {
            //var moveJson = MoveSerializer.SerializeToJson(move);
            //await _hubConnection.InvokeAsync("MakeMove", _currentGameId, moveJson);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke($"Move failed: {ex.Message}");
        }
    }

    private void HandleMatchFound(string gameId)
    {
        _currentGameId = gameId;
        MatchFound?.Invoke(gameId);
        _ = JoinGame(gameId);
    }

    private async Task JoinGame(string gameId)
    {
        await _hubConnection.InvokeAsync("JoinGame", gameId);
    }

    private void HandleGameState(string fen)
    {
        GameState?.Invoke(fen);
    }

    private void HandleMoveReceived(ChessMove move)
    {
        MoveReceived?.Invoke(move);
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient?.Dispose();
        await _hubConnection.DisposeAsync();
    }
}
*/