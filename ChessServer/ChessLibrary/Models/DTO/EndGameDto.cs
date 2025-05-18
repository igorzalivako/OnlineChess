using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Models.DTO
{
    public enum EndGameType { Checkmate, Stalemate, EndTime, UserLeave }
    public record EndGameDto(EndGameType EndGameType, bool YouWon, string Message);
}
