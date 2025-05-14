using ChessLibrary.Models.DTO;
using ChessServer.Data;
using ChessServer.Models;
using ChessEngine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
public class GameHub : Hub
{
    private readonly AppDbContext _db;
    private readonly ChessGame _engine;
    private readonly MatchmakingService _matchmakingService;

    public GameHub(AppDbContext db, ChessGame engine, MatchmakingService matchmakingService)
    {
        _db = db;
        _engine = engine;
        _matchmakingService = matchmakingService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await base.OnConnectedAsync();
    }

    public async Task JoinMatchmakingQueue(int gameModeMinutes)
    {
        var userId = GetCurrentUserId();

        if (await IsUserInQueueOrActiveGame(userId))
        {
            await Clients.Caller.SendAsync("ErrorOccurred", "User is already in queue or has an active game");
            return;
        }

        await _matchmakingService.AddToQueue(userId, gameModeMinutes);
    }

    public async Task LeaveGame()
    {
        var userId = GetCurrentUserId();
        var game = await GetActiveGameForUser(userId);

        if (game != null)
        {
            game.Status = GameStatus.Aborted;
            await _db.SaveChangesAsync();

            await Clients.Group(game.Id.ToString()).SendAsync("LeaveGame", userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id.ToString());
        }
    }

    public async Task GetAvailableMoves(string position)
    {
        //_engine.LoadPosition(position);
        //var moves = _engine.GetValidMoves();
        //await Clients.Caller.SendAsync("FoundAvailableMoves", moves);
    }

    public async Task ApplyMove(string gameId, ChessMove move)
    {
        var game = await _db.Games.FindAsync(gameId);
        if (game == null || game.Status != GameStatus.Active) return;

        var engineMove = ConvertToEngineMove(move);
        _engine.LoadPosition(game.Position);

        if (!_engine.IsMoveValid(engineMove))
        {
            await Clients.Caller.SendAsync("ErrorOccurred", "Invalid move");
            return;
        }

        if (game.PlayerWhiteId == Clients.Caller.

        _engine.ApplyMove(engineMove);
        game.Position = _engine.Position;
        //game.Moves.Add(MoveSerializer.SerializeToJson(move));
        await _db.SaveChangesAsync();

        await Clients.Caller.SendAsync("MoveVerified", move, );
        await Clients.OthersInGroup(gameId).SendAsync("OpponentMove", move);

        /*if (_engine.IsGameOver())
        {
            game.Status = _engine.IsCheckmate() ? GameStatus.Checkmate : GameStatus.Stalemate;
            await _db.SaveChangesAsync();
            await Clients.Group(gameId).SendAsync("EndGame", game.Status.ToString());
        }*/
    }

    /*
    // MatchmakingService будет вызывать этот метод при нахождении матча
    public async Task NotifyMatchFound(int player1, int player2, string gameId)
    {
        var game = await _db.Games.FindAsync(gameId);
        var firstPlayer = await _db.Users.FindAsync(player1);
        var secondPlayer = await _db.Users.FindAsync(player2);

        // Отправка игрокам через User ID
        await Clients.User(player1.ToString()).SendAsync("MatchFound", new MatchFoundDto
        {
            GameId = gameId,
            Position = game.GetFen(),
            YourColor = "white",
            OpponentUsername = secondPlayer.Username,
            OpponentRating = secondPlayer.Rating,
        });

        await Clients.User(player2.ToString()).SendAsync("MatchFound", new MatchFoundDto
        {
            GameId = gameId,
            Position = game.GetFen(),
            YourColor = "black",
            OpponentUsername = firstPlayer.Username,
            OpponentRating = firstPlayer.Rating,    
        });

        // Добавляем игроков в группу игры через их текущие подключения
        await Clients.User(player1.ToString()).SendAsync("JoinGameGroup", gameId);
        await Clients.User(player2.ToString()).SendAsync("JoinGameGroup", gameId);
    }
    */

    private int GetCurrentUserId()
    {
        return int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    private async Task<bool> IsUserInQueueOrActiveGame(int userId)
    {
        return await _db.MatchmakingQueue.AnyAsync(e => e.UserId == userId) ||
               await _db.Games.AnyAsync(g => (g.PlayerWhiteId == userId || g.PlayerBlackId == userId) &&
                                             g.Status == GameStatus.Active);
    }

    private async Task<Game> GetActiveGameForUser(int userId)
    {
        return await _db.Games.FirstOrDefaultAsync(g =>
            (g.PlayerWhiteId == userId || g.PlayerBlackId == userId) &&
            g.Status == GameStatus.Active);
    }

    private static Move ConvertToEngineMove(ChessMove move)
    {
        return new Move
        {
            From = (byte)(move.FromX + 7 + move.FromY * 8),
            To = (byte)(move.ToX + 7 + move.ToY * 8)
        };
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        var game = await _db.Games.FindAsync(Guid.Parse(gameId));
        await Clients.Caller.SendAsync("GameState", game.GetFen());
    }
}