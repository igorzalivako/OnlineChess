using System.ComponentModel.DataAnnotations;

namespace ChessServer.Models
{
    public class MatchmakingEntry
    {
        [Key]
        public int UserId { get; set; }
        public int Rating { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}