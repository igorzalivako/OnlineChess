using ChessEngine;
using ChessServer.Data;
using ChessServer.Utilities;
using Microsoft.AspNetCore.SignalR;
using ChessLibrary.Models.DTO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
/*
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
        await Clients.Caller.SendAsync("GameState", game.Position);
    }

    public async Task MakeMove(string gameId, string moveJson)
    {
        var game = await _db.Games.FindAsync(gameId);
        if (game == null || game.Status != ChessServer.Models.GameStatus.Active) return;

        ChessMove move = MoveSerializer.DeserializeFromJson(moveJson);

        Move engineMove = ConvertToEngineMove(move);

        // Валидация хода через движок
        _engine.LoadPosition(game.Position);
        if (!_engine.IsMoveValid(engineMove))
        {
            await Clients.Caller.SendAsync("InvalidMove", move);
            return;
        }

        // Обновление состояния
        _engine.ApplyMove(engineMove);
        game.Position = _engine.Position;
        game.Moves.Add(moveJson);
        await _db.SaveChangesAsync();

        // Рассылка хода
        await Clients.OthersInGroup(gameId).SendAsync("MoveMade", move);
    }

    private static Move ConvertToEngineMove(ChessMove move)
    {
        Move result = default;
        result.From = (byte)(move.FromX + 7 + move.FromY * 8);
        result.To = (byte)(move.ToX + 7 + move.ToY * 8);
        return result;
    }
}
*/