using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Models.DTO
{
    public class ChessMove
    {
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
        public DtoPieceType PromoteType { get; set; } = DtoPieceType.None;
    }
}
