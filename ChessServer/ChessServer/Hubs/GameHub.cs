using ChessEngine;
using ChessServer.Data;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

public class GameHub : Hub
{
    private readonly AppDbContext _db;
    private readonly ChessGame _engine;

    public GameHub(AppDbContext db, ChessGame engine)
    {
        _db = db;
        _engine = engine;
    }

    public async Task JoinGame(string gameId)
    {
        var game = await _db.Games.FindAsync(gameId);
        if (game == null) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Caller.SendAsync("GameState", game.CurrentFEN);
    }

    public async Task MakeMove(string gameId, string move)
    {
        var game = await _db.Games.FindAsync(gameId);
        if (game == null || game.Status != ChessServer.Models.GameStatus.Active) return;

        // Валидация хода через движок
        _engine.LoadPosition(game.CurrentFEN);
        if (!_engine.IsMoveValid(move))
        {
            await Clients.Caller.SendAsync("InvalidMove", move);
            return;
        }

        // Обновление состояния
        _engine.ApplyMove(move);
        game.CurrentFEN = _engine.GetFEN();
        game.Moves.Add(move);
        await _db.SaveChangesAsync();

        // Рассылка хода
        await Clients.OthersInGroup(gameId).SendAsync("MoveMade", move);
    }
}