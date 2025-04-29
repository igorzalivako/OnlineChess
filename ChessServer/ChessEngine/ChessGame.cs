using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class ChessGame
    {
        public Position Position { get; set; }
        public void ApplyMove(Move move)
        {
            Position.MakeMove(move);
        }

        public bool IsMoveValid(Move move)
        {
            MoveList availableMoves = LegalMovesGenerator.Generate(Position, move.AttackerSide, false);
            // lifted operator
            return availableMoves.FirstOrDefault((Move m) => m == move) != null;
        }

        // Реализовать логику движка
        public void LoadPosition(Position position)
        {
            Position = position;
        }
    }
}
