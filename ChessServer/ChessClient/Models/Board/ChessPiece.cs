using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;

namespace ChessClient.Models
{
    public enum PieceType { Pawn, Rook, Knight, King, Queen, Bishop };
    public class ChessPiece
    {
        private static readonly Dictionary<PieceColor, Dictionary<PieceType, string>> piecesImages = new Dictionary<PieceColor, Dictionary<PieceType, string>>()
        {
            {
                PieceColor.White, new Dictionary<PieceType, string>
                {
                    { PieceType.Pawn, "white_pawn.png" },
                    { PieceType.Knight, "white_knight.png" },
                    { PieceType.Bishop, "white_bishop.png" },
                    { PieceType.Queen, "white_queen.png" },
                    { PieceType.King, "white_king.png" },
                    { PieceType.Rook, "white_rook.png" }
                }
            },
            {
                PieceColor.Black, new Dictionary<PieceType, string>
                {
                    { PieceType.Pawn, "black_pawn.png" },
                    { PieceType.Knight, "black_knight.png" },
                    { PieceType.Bishop, "black_bishop.png" },
                    { PieceType.Queen, "black_queen.png" },
                    { PieceType.King, "black_king.png" },
                    { PieceType.Rook, "black_rook.png" }
                }
            },
            {
                PieceColor.None, new Dictionary<PieceType, string>
                {
                    { PieceType.Pawn, "white_pawn.png" },
                    { PieceType.Knight, "white_knight.png" },
                    { PieceType.Bishop, "white_bishop.png" },
                    { PieceType.Queen, "white_queen.png" },
                    { PieceType.King, "white_king.png" },
                    { PieceType.Rook, "white_rook.png" }
                }
            },
        };

        private PieceType _type;
        public PieceType Type { get { return GetChessPiece(); } set { SetChessPiece(value); } }

        private PieceType GetChessPiece()
        {
            return _type;
        }

        private void SetChessPiece(PieceType value)
        {
            _type = value;
            Image = piecesImages[Color][value];
        }

        public PieceColor Color { get; set; }
        public string Image { get; private set; }
    }
}
