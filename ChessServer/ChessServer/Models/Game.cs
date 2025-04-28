namespace ChessServer.Models
{
    public class Game
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int PlayerWhiteId { get; set; }
        public int PlayerBlackId { get; set; }
        public string CurrentFEN { get; set; }
        public List<string> Moves { get; set; } = new();
        public GameStatus Status { get; set; } = GameStatus.Waiting;
    }

    public enum GameStatus { Waiting, Active, Finished }
}
