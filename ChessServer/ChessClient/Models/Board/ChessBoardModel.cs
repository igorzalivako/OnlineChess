using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessClient.Models.Board;
using ChessEngine;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChessClient.Models
{
    public class ChessBoardModel
    {
        public ChessPiece[,] BoardSquares { get; set; }

        [ObservableProperty]
        public List<BoardMove> _possibleMoves;

        public ChessBoardModel()
        {

        }

        public void InitializeGame(/* тут параметры для начала игры*/ )
        {
            string shortFen = ""; // получить Fen для партии
            BoardSquares = ParseFen(shortFen);
        }

        private ChessPiece[,] ParseFen(string shortFen)
        {
            ChessPiece[,] result = new ChessPiece[8,8];
            

            int x = 0;
            int y = 7;

            foreach (char c in shortFen)
            {
                if (c == '/')
                {
                    x = 0;
                    y--;
                }
                else if (char.IsDigit(c))
                {
                    result[x, y] = null;
                }
                else
                {
                    PieceColor side = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                    char pieceChar = char.ToLower(c);

                    ChessPiece chessPiece = new ChessPiece() { Type =
                    pieceChar switch
                    {
                        'p' => PieceType.Pawn,
                        'n' => PieceType.Knight,
                        'b' => PieceType.Bishop,
                        'r' => PieceType.Rook,
                        'q' => PieceType.Queen,
                        'k' => PieceType.King,
                        _ => throw new ArgumentException("Invalid FEN character")
                    }};

                    BoardSquares[x, y] = chessPiece;
                    x++;
                }
            }

            return result;
        }

        // получаем список возможных ходов для фигуры с сервера и присваиваем его _possibleMoves
        public static List<BoardMove> GetPossibleMoves(BoardPosition from)
        {
            // PossibleMoves = ...
            throw new NotImplementedException();    
        }

        public void MakeMove(BoardMove move)
        {
            GetPossibleMoves(move.From);
            if (_possibleMoves.Contains(move))
            {
                // отправка хода на сервер и затем применение и обновление позиции
            }
        }
        /*private BoardSquare[,] CreateBoard()
        {
            BoardSquare[,] boardSquares = new BoardSquare[8,8];
            for (int i = 0; i < 8; i++) 
            {
                for (int j = 0; j < 8; j++)
                {
                    boardSquares[i, j] = new BoardSquare();
                }
            }
        }*/
    }
}
