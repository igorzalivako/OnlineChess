using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;
using ChessLibrary.Models.DTO;

namespace ChessLibrary.Converters
{
    public static class ConverterToEngineMove
    {
        public static Move ConvertToEngineMove(ChessMove move, PieceColor attackerColor)
        {
            return new Move
            {
                From = (byte)(move.FromX + move.FromY * 8),
                To = (byte)(move.ToX + move.ToY * 8),
                AttackerSide = attackerColor,
                Flag = move.PromoteType switch
                {
                    DtoPieceType.Queen => MoveFlag.PromoteToQueen,
                    DtoPieceType.Knight => MoveFlag.PromoteToKnight,
                    DtoPieceType.Bishop => MoveFlag.PromoteToBishop,
                    DtoPieceType.Rook => MoveFlag.PromoteToRook,
                    _ => MoveFlag.Default,
                }
            };
        }
    }
}
