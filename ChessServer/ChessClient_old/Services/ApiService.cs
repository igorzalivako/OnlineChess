using System.Text.Json;
using ChessClient.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChessClient.Services;

public interface IApiService
{
    public string AuthToken { get; set; }
    Task<UserDto?> Login(string username, string password);
    Task<UserDto?> Register(string username, string password);
    Task<GameDto?> FindMatch();
    Task<GameStateDto?> GetGameState(string gameId);
}

public partial class ApiService : ObservableObject, IApiService
{
    private readonly IChessApi _api;
    private readonly IConnectivity _connectivity;

    [ObservableProperty]
    private string _authToken;

    public ApiService(IChessApi api, IConnectivity connectivity)
    {
        _api = api;
        _connectivity = connectivity;
    }

    public async Task<UserDto?> Login(string username, string password)
    {
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            throw new Exception("No internet connection");

        var response = await _api.Login(new UserLoginDto(username, password));

        if (response.IsSuccessStatusCode)
        {
            AuthToken = response.Content.Token;
            await SecureStorage.SetAsync("auth_token", AuthToken);
            return response.Content;
        }

        throw new Exception(response.Error?.Content ?? "Login failed");
    }

    public async Task<GameDto?> FindMatch()
    {
        if (string.IsNullOrEmpty(AuthToken))
            throw new Exception("Not authenticated");

        var response = await _api.FindMatch($"Bearer {AuthToken}");
        return response.IsSuccessStatusCode ? response.Content : null;
    }
    public async Task<UserDto?> Register(string username, string password)
    {
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            throw new Exception("No internet connection");

        var response = await _api.Register(new UserRegisterDto(username, password));

        if (response.IsSuccessStatusCode)
        {
            AuthToken = response.Content.Token;
            await SecureStorage.SetAsync("auth_token", AuthToken);
            return response.Content;
        }

        throw new Exception(response.Error?.Content ?? "Registration failed");
    }

    public async Task<GameStateDto?> GetGameState(string gameId)
    {
        if (string.IsNullOrEmpty(AuthToken))
            throw new Exception("Not authenticated");

        var response = await _api.GetGameState(gameId, $"Bearer {AuthToken}");
        return response.IsSuccessStatusCode ? response.Content : null;
    }
    public async Task<bool> ResignGame(string gameId)
    {
        try
        {
            var response = await _api.Resign(gameId, $"Bearer {AuthToken}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}