using ChessEngine;
using System.ComponentModel.DataAnnotations.Schema;
using ChessServer.Utilities;

namespace ChessServer.Models
{
    public class Game
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int PlayerWhiteId { get; set; }
        public int PlayerBlackId { get; set; }

        [Column(TypeName = "BLOB")]  // Для MySQL используем LONGBLOB
        public byte[] PositionBytes { get; set; }  // Вместо CurrentFEN

        [NotMapped]  // Не хранить в БД
        public Position Position
        {
            get => PositionSerializer.DeserializeFromBytes(PositionBytes);
            set => PositionBytes = PositionSerializer.SerializeToBytes(value);
        }
        public List<string> Moves { get; set; } = new();
        public GameStatus Status { get; set; } = GameStatus.Waiting;
    }

    public enum GameStatus { Waiting, Active, Finished }
}
