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

    public async Task AddToQueue(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        _db.MatchmakingQueue.Add(new MatchmakingEntry
        {
            UserId = userId,
            Rating = user.Rating
        });
        await _db.SaveChangesAsync();
        await TryFindMatch(userId);
    }

    private async Task TryFindMatch(int userId)
    {
        var currentUser = await _db.MatchmakingQueue.FindAsync(userId);
        var candidates = await _db.MatchmakingQueue
            .Where(x => x.UserId != userId)
            .OrderBy(x => Math.Abs(x.Rating - currentUser.Rating))
            .Take(1)
            .ToListAsync();

        if (candidates.Any())
        {
            var opponent = candidates.First();
            await CreateGame(currentUser.UserId, opponent.UserId);
        }
    }

    private async Task CreateGame(int player1, int player2)
    {
        var game = new Game
        {
            PlayerWhiteId = player1,
            PlayerBlackId = player2,
            CurrentFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
        };

        _db.Games.Add(game);
        _db.MatchmakingQueue.RemoveRange(
            _db.MatchmakingQueue.Where(x => x.UserId == player1 || x.UserId == player2));
        await _db.SaveChangesAsync();

        // Уведомляем игроков через SignalR
        await _hubContext.Clients.Users(player1.ToString(), player2.ToString())
            .SendAsync("MatchFound", game.Id);
    }
}