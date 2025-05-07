using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Models.Board
{
    public class BoardSquare
    {
        public BoardPosition Position { get; init; }
        public ChessPiece Piece { get; set; }
    }
}
