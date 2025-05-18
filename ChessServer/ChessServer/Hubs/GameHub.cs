using ChessLibrary.Models.DTO;
using ChessServer.Data;
using ChessServer.Models;
using ChessEngine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using ChessServer.Utilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ChessLibrary.Converters;
public class GameHub : Hub
{
    private readonly AppDbContext _db;
    private readonly ChessGame _engine;
    private readonly MatchmakingService _matchmakingService;
    private const int ERROR = 3;

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

    public async Task LeaveGame(string gameId)
    {
        var game = await _db.Games.FindAsync(Guid.Parse(gameId));
        if (game == null || game.Status != GameStatus.Active) return;

        if (game != null)
        {
            game.Status = GameStatus.Finished;
            await _db.SaveChangesAsync();

            await Clients.Caller.SendAsync("EndGame", new EndGameDto(EndGameType.UserLeave, false, "Поражение"));
            await Clients.OthersInGroup(gameId).SendAsync("EndGame", new EndGameDto(EndGameType.UserLeave, true, "Победа"));
            //await Clients.Group(game.Id.ToString()).SendAsync("EndGame", new EndGameDto());
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id.ToString());
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
        var game = await _db.Games.FindAsync(Guid.Parse(gameId));
        if (game == null || game.Status != GameStatus.Active) return;

        _engine.LoadPosition(game.Position);

        var callerIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        int callerId;
        if (callerIdClaim == null || !int.TryParse(callerIdClaim.Value, out callerId) || callerId != game.ActivePlayerId)
        {
            await Clients.Caller.SendAsync("MoveVerified", _engine.GetFen(), false);
        }
        else
        {
            var callerColor = game.PlayerWhiteId == callerId ? PieceColor.White : PieceColor.Black; 
            Move engineMove = ConverterToEngineMove.ConvertToEngineMove(move, callerColor);
            bool isMoveCorrect = _engine.ApplyMove(engineMove, callerColor);
            await Clients.Caller.SendAsync("MoveVerified", _engine.GetFen(), isMoveCorrect);
            if (isMoveCorrect) 
            {
                // вычислить актуальное время для активного игрока
                var now = DateTime.UtcNow;
                var timeSpent = (int)(now - game.LastMoveTime).TotalSeconds;
                if (game.Status == GameStatus.Active)
                {
                    if (game.ActivePlayerId == game.PlayerWhiteId)
                        game.TimeLeftWhite -= timeSpent;
                    else
                        game.TimeLeftBlack -= timeSpent;
                }

                game.ActivePlayerId = game.ActivePlayerId == game.PlayerWhiteId ? game.PlayerBlackId : game.PlayerWhiteId;
                game.LastMoveTime = now;
                game.Position = _engine.Position;
                game.Moves.Add(JsonSerializer.Serialize(move));
                await _db.SaveChangesAsync();
                List<ChessMove> availableMoves = ConverterToMoveList.ConvertToChessMoveList(_engine.GetValidMoves(_engine.InverseColor(callerColor)));
                await Clients.OthersInGroup(gameId).SendAsync("OpponentMove", _engine.GetFen(), availableMoves);
                await Clients.Group(gameId).SendAsync("UpdateTimers", game.TimeLeftWhite, game.TimeLeftBlack);
                await ProcessEndGame(gameId, game);
            }
        }

        /*if (_engine.IsGameOver())
        {
            game.Status = _engine.IsCheckmate() ? GameStatus.Checkmate : GameStatus.Stalemate;
            await _db.SaveChangesAsync();
            await Clients.Group(gameId).SendAsync("EndGame", game.Status.ToString());
        }*/
    }

    public async Task EndTime(string gameId)
    {
        var game = await _db.Games.FindAsync(Guid.Parse(gameId));
        if (game == null || game.Status != GameStatus.Active) return;

        var callerIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        int callerId;
        if (callerIdClaim == null || !int.TryParse(callerIdClaim.Value, out callerId))
        {
            await Clients.Caller.SendAsync("MoveVerified", _engine.GetFen(), false);
        }
        else
        {
            // --- ЛОГИКА КОНТРОЛЯ ВРЕМЕНИ ---
            var now = DateTime.UtcNow;
            var timeSpent = (int)(now - game.LastMoveTime).TotalSeconds;

            var timeLeftWhite = game.TimeLeftWhite;
            var timeLeftBlack = game.TimeLeftBlack;
            if (game.ActivePlayerId == game.PlayerWhiteId)
            {
                timeLeftWhite -= timeSpent;
                if (timeLeftWhite < ERROR)
                {
                    game.Status = GameStatus.Finished;
                    await _db.SaveChangesAsync();
                    await SendEndGameMessage(gameId);
                }
            }
            else
            {
                timeLeftBlack -= timeSpent;
                if (timeLeftBlack < ERROR)
                {
                    game.Status = GameStatus.Finished;
                    await _db.SaveChangesAsync();    
                    await SendEndGameMessage(gameId);   
                }
            }
            // --------------------------------
        }
    }

    private async Task SendEndGameMessage(string gameId)
    {
        await Clients.Caller.SendAsync("EndGame", new EndGameDto(EndGameType.EndTime, false, "У вас закончилось время!\nВы проиграли"));
        await Clients.OthersInGroup(gameId).SendAsync("EndGame", new EndGameDto(EndGameType.EndTime, true, "У противника закончилось время!\nВы победили"));
    }

    private async Task ProcessEndGame(string gameId, Game game)
    {
        if (_engine.IsCheckmate)
        {
            await Clients.OthersInGroup(gameId).SendAsync("EndGame", new EndGameDto(EndGameType.Checkmate, false, ""));
            await Clients.Caller.SendAsync("EndGame", new EndGameDto(EndGameType.Checkmate, true, ""));
            game.Status = GameStatus.Finished;
            await _db.SaveChangesAsync();
        }
        else if (_engine.IsStalemate)
        {
            string message;
            if (_engine.AvailableMoves != null && _engine.AvailableMoves.Size == 0)
            {
                message = "Объявлен пат";
            }
            else 
            {
                message = "Ничья по причине повторения позиций";
            }
            await Clients.OthersInGroup(gameId).SendAsync("EndGame", new EndGameDto(EndGameType.Stalemate, false, message));
            await Clients.Caller.SendAsync("EndGame", new EndGameDto(EndGameType.Stalemate, false, message));
            game.Status = GameStatus.Finished;
            await _db.SaveChangesAsync();
        }
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

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        var game = await _db.Games.FindAsync(Guid.Parse(gameId));
        await Clients.Caller.SendAsync("GameState", game.GetFen());
    }
}