using ChessEngine;
using System.Text.Json;

namespace ChessServer.Utilities
{
    public class MoveSerializer
    {
        public static string SerializeToJson(Move move)
        {
            return JsonSerializer.Serialize(move);
        }

        public static Move DeserializeFromJson(string json)
        {
            return JsonSerializer.Deserialize<Move>(json);
        }
    }
}
