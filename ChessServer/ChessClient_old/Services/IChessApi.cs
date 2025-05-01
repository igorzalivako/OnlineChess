using Refit;
using ChessClient.Models;

namespace ChessClient.Services;

public interface IChessApi
{
    [Post("/api/users/register")]
    Task<ApiResponse<UserDto>> Register(UserRegisterDto dto);

    [Post("/api/users/login")]
    Task<ApiResponse<UserDto>> Login(UserLoginDto dto);

    [Post("/api/games/find-match")]
    Task<ApiResponse<GameDto>> FindMatch([Header("Authorization")] string token);

    [Get("/api/games/{gameId}")]
    Task<ApiResponse<GameStateDto>> GetGameState(
        string gameId,
        [Header("Authorization")] string token);

    [Post("/api/games/{gameId}/resign")]
    Task<ApiResponse<bool>> Resign(
    string gameId,
    [Header("Authorization")] string token);
}