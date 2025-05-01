using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Models
{
    public record GameDto(string Id, string PlayerWhite, string PlayerBlack);

    public record GameStateDto(
        string CurrentPosition,
        List<string> Moves,
        string Status);
}
