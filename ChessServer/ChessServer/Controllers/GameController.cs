using ChessLibrary.Models.DTO;
using ChessServer.Data;
using ChessServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/*[ApiController]
[Route("api/games")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly MatchmakingService _matchmakingService;

    public GamesController(AppDbContext db, MatchmakingService matchmakingService)
    {
        _db = db;
        _matchmakingService = matchmakingService;
    }

    [HttpPost("join-queue")]
    [Authorize]
    public async Task<IActionResult> JoinMatchmakingQueue([FromBody] JoinQueueRequestDto request)
    {
        var userId = GetCurrentUserId();
        if (await IsUserInQueueOrActiveGame(userId))
        {
            return BadRequest(new { Success = false, Error = "User is already in queue or has an active game" });
        }

        await _matchmakingService.AddToQueue(userId, request.GameModeMinutes);
        return Ok(new { Success = true });
    }

    [HttpPost("leave-queue")]
    public async Task<IActionResult> LeaveMatchmakingQueue()
    {
        var userId = GetCurrentUserId();
        var entry = await _db.MatchmakingQueue.FindAsync(userId);

        if (entry == null)
            return BadRequest(new { Success = false, Error = "User is not in queue" });

        _db.MatchmakingQueue.Remove(entry);
        await _db.SaveChangesAsync();
        return Ok(new { Success = true });
    }

    [HttpGet("current")]
    public async Task<ActionResult<GameResponseDto>> GetCurrentGame()
    {
        var userId = GetCurrentUserId();
        var game = await _db.Games
            .FirstOrDefaultAsync(g =>
                (g.PlayerWhiteId == userId || g.PlayerBlackId == userId) &&
                g.Status == GameStatus.Active);

        if (game == null)
            return NotFound(new { Success = false, Error = "No active game found" });

        return Ok(new GameResponseDto
        {
            Id = game.Id.ToString(),
            PlayerWhite = game.PlayerWhiteId,
            PlayerBlack = game.PlayerBlackId,
            Status = game.Status.ToString(),
            Position = game.GetFen() // Предполагается метод получения FEN
        });
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    private async Task<bool> IsUserInQueueOrActiveGame(int userId)
    {
        bool inQueue = await _db.MatchmakingQueue.AnyAsync(e => e.UserId == userId);
        bool inGame = await _db.Games.AnyAsync(g =>
            (g.PlayerWhiteId == userId || g.PlayerBlackId == userId) &&
            g.Status == GameStatus.Active);

        return inQueue || inGame;
    }
}
*/