using ChessEngine;
using System.Text.Json;
using ChessLibrary.Models.DTO;

namespace ChessServer.Utilities
{
    public class MoveSerializer
    {
        public static string SerializeToJson(ChessMove move)
        {
            return JsonSerializer.Serialize(move);
        }

        public static ChessMove DeserializeFromJson(string json)
        {
            return JsonSerializer.Deserialize<ChessMove>(json);
        }
    }
}
