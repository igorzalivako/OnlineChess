using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Models
{
    public enum PieceType { Pawn, Rook, Knight, King, Queen, Bishop };
    public class ChessPiece
    {
        public PieceType Type { get; set; }
    }
}
