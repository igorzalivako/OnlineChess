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
        public bool ApplyMove(Move move, PieceColor attackerSide)
        {
            MoveList availableMoves = LegalMovesGenerator.Generate(Position, attackerSide, false);
            if (availableMoves.FirstOrDefault((Move m) => m.From == move.From && m.To == move.To) != null)
            {
                Position.MakeMove(move);
                return true;
            }
            else
            {
                return false;
            }
        }
        public MoveList GetValidMoves(PieceColor attackerSide)
        {
            MoveList availableMoves = LegalMovesGenerator.Generate(Position, attackerSide, false);
            // lifted operator
            return availableMoves;
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
