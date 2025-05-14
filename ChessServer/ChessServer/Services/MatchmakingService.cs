using ChessEngine;
using ChessLibrary.Models.DTO;
using ChessServer.Data;
using ChessServer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class MatchmakingService
{
    private readonly AppDbContext _db;
    private readonly IHubContext<GameHub> _hubContext;

    public MatchmakingService(AppDbContext db, IHubContext<GameHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    public async Task AddToQueue(int userId, int gameModeMinutes)
    {
        var user = await _db.Users.FindAsync(userId);
        _db.MatchmakingQueue.Add(new MatchmakingEntry
        {
            UserId = userId,
            Rating = user.Rating,
            GameModeMinutes = gameModeMinutes
        });
        await _db.SaveChangesAsync();
        await TryFindMatch(userId);
    }

    private async Task TryFindMatch(int userId)
    {
        var currentUser = await _db.MatchmakingQueue.FindAsync(userId);
        var candidates = await _db.MatchmakingQueue
            .Where(x => x.UserId != userId && x.GameModeMinutes == currentUser.GameModeMinutes)
            .OrderBy(x => Math.Abs(x.Rating - currentUser.Rating))
            .Take(1)
            .ToListAsync();

        if (candidates.Any())
        {
            var opponent = candidates.First();
            await CreateGame(opponent.UserId, currentUser.UserId, currentUser.GameModeMinutes);
        }
    }

    private async Task CreateGame(int player1, int player2, int gameModeMinutes)
    {
        var game = new Game
        {
            PlayerWhiteId = player1,
            PlayerBlackId = player2,
            Position = new ChessEngine.Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"),
            GameModeMinutes = gameModeMinutes,
        };

        _db.Games.Add(game);
        _db.MatchmakingQueue.RemoveRange(
            _db.MatchmakingQueue.Where(x => x.UserId == player1 || x.UserId == player2));
        await _db.SaveChangesAsync();

        /*// Уведомляем игроков через SignalR
        await _hubContext.Clients.Users(player1.ToString(), player2.ToString())
            .SendAsync("MatchFound", game.Id);*/

        var firstPlayer = await _db.Users.FindAsync(player1);
        var secondPlayer = await _db.Users.FindAsync(player2);

        var chessGame = new ChessGame();
        chessGame.Position = game.Position;
        MoveList moveList = chessGame.GetValidMoves(PieceColor.White);

        // Отправка уведомлений через UserId (не требуется connectionId)
        await _hubContext.Clients.Users(player1.ToString())
            .SendAsync("MatchFound", new MatchFoundDto
            {
                GameId = game.Id.ToString(),
                Position = game.GetFen(),
                YourColor = "white",
                OpponentUsername = secondPlayer.Username,
                OpponentRating = secondPlayer.Rating, 
                AvailableMoves = ConvertToChessMoveList(moveList)

            });
        await _hubContext.Clients.Users(player2.ToString())
            .SendAsync("MatchFound", new MatchFoundDto
            {
                GameId = game.Id.ToString(),
                Position = game.GetFen(),
                YourColor = "black",
                OpponentUsername = firstPlayer.Username,
                OpponentRating = secondPlayer.Rating
            });
    }

    private List<ChessMove> ConvertToChessMoveList(MoveList moveList)
    {
        List<ChessMove> result = new List<ChessMove>(); 
        for (int i = 0; i < moveList.Size; i++) 
        {
            ChessMove move = new ChessMove();
            move.FromX = moveList[i].From % 8;
            move.FromY = moveList[i].From / 8;
            move.ToX = moveList[i].To % 8;
            move.ToY = moveList[i].To / 8;
            result.Add(move);
        }
        return result;
    }
}