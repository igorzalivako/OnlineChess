using ChessEngine;
using System.ComponentModel.DataAnnotations.Schema;
using ChessServer.Utilities;
using System.Text;

namespace ChessServer.Models
{
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int PlayerWhiteId { get; set; }
        public int PlayerBlackId { get; set; }
        public int GameModeMinutes { get; set; } // Добавлено новое свойство

        [Column(TypeName = "BLOB")]  // Для MySQL используем LONGBLOB
        public byte[] PositionBytes { get; set; }  // Вместо CurrentFEN

        [NotMapped]  // Не хранить в БД
        public Position Position
        {
            get => PositionSerializer.DeserializeFromBytes(PositionBytes);
            set => PositionBytes = PositionSerializer.SerializeToBytes(value);
        }
        public List<string> Moves { get; set; } = new();
        public GameStatus Status { get; set; } = GameStatus.Waiting;

        public string GetFen()
        {
            StringBuilder fen = new StringBuilder();
            int freeSquaresCount = 0;
            for (int j = 7; j >= 0; j--)   
            {
                for (int i = 0; i < 8; i++)

                {
                    PieceType currentPiece = PieceType.None;
                    PieceColor currentPieceColor = PieceColor.None;
                    // проверяем все фигуры на данной позиции
                    for (PieceType pieceType = PieceType.Pawn; pieceType < PieceType.None; pieceType++)
                    {
                        for (PieceColor pieceColor = 0; pieceColor < PieceColor.None; pieceColor++)
                        {
                            if ((Position.Pieces.PieceBitboards[(int)pieceColor, (int)pieceType].Value & (1ul << GetBitboardPosition(i, j))) != 0)
                            {
                                currentPiece = pieceType;
                                currentPieceColor = pieceColor;
                                break;
                            }
                        }
                        if (currentPiece != PieceType.None)
                        {
                            break;
                        }
                    }
                    if (currentPiece == PieceType.None)
                    {
                        freeSquaresCount++;
                    }
                    else
                    {
                        if (freeSquaresCount > 0)
                        {
                            fen.Append(freeSquaresCount.ToString());
                            freeSquaresCount = 0;
                        }
                        char piece = currentPiece switch
                        {
                            PieceType.Pawn => currentPieceColor switch { PieceColor.White => 'P', PieceColor.Black => 'p' },
                            PieceType.Knight => currentPieceColor switch { PieceColor.White => 'N', PieceColor.Black => 'n' },
                            PieceType.Rook => currentPieceColor switch { PieceColor.White => 'R', PieceColor.Black => 'r' },
                            PieceType.Bishop => currentPieceColor switch { PieceColor.White => 'B', PieceColor.Black => 'b' },
                            PieceType.Queen => currentPieceColor switch { PieceColor.White => 'Q', PieceColor.Black => 'q' },
                            PieceType.King => currentPieceColor switch { PieceColor.White => 'K', PieceColor.Black => 'k' }
                        };
                        fen.Append(piece);
                    }
                }
                if (freeSquaresCount > 0)
                {
                    fen.Append(freeSquaresCount.ToString());
                    freeSquaresCount = 0;
                }
                fen.Append('/');
            }

            return fen.Remove(fen.Length - 1, 1).ToString();
        }



        private int GetBitboardPosition(int x, int y) => y * 8 + x;
    }

    public enum GameStatus { Waiting, Active, Finished, Aborted }
}
