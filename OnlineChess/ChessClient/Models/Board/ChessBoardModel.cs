using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessClient.Models.Board;
using ChessEngine;
using ChessLibrary.Models.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace ChessClient.Models
{
    public partial class ChessBoardModel : ObservableObject
    {
        public PieceColor PlayerColor { get; set; }
        public BoardSquare[,] _boardSquares;
        public event Action UpdateBoard;
        // Объявление публичного свойства Squares
        public BoardSquare[,] BoardSquares
        {
            get => _boardSquares;
            private set => SetProperty(ref _boardSquares, value);
        }

        // Индексатор для работы с элементами
        public BoardSquare this[int row, int col]
        {
            get => _boardSquares[row, col];
            set
            {
                if (_boardSquares[row, col] != value)
                {
                    // Отписка от старого элемента
                    if (_boardSquares[row, col] is INotifyPropertyChanged oldSquare)
                        oldSquare.PropertyChanged -= OnSquarePropertyChanged;

                    _boardSquares[row, col] = value;

                    // Подписка на новый элемент
                    if (value is INotifyPropertyChanged newSquare)
                        newSquare.PropertyChanged += OnSquarePropertyChanged;

                    OnPropertyChanged(nameof(BoardSquares));
                    OnPropertyChanged($"Item[{row},{col}]");
                }
            }
        }

        // Уведомление о любом изменении внутри Square
        private void OnSquarePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Можно передать индекс измененного элемента, если нужно
            OnPropertyChanged(nameof(BoardSquares));
        }

        public ChessBoardModel()
        {
            BoardSquares = new BoardSquare[8, 8];
        }

        /*public void InitializeGame( тут параметры для начала игры )
        {
            string shortFen = ""; // получить Fen для партии
            BoardSquares = ParseFen(shortFen);
        }*/

        /*private BoardSquare[,] ParseFen(string shortFen)
        {
            BoardSquare[,] result = new BoardSquare[8,8];
            

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

                    BoardSquares[x, y].Piece = chessPiece;
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
        */
        public void MakeMove(ChessMove move)
        {
            var movedPiece = BoardSquares[move.FromX, move.FromY].Piece;
            BoardSquares[move.FromX, move.FromY].Piece = null;
            BoardSquares[move.ToX, move.ToY].Piece = movedPiece;
        }

        public void LoadPositionFromFen(string fen, bool needsCreateNewBoard = true)
        {
            BoardSquares = CreateBoard();
            int x = 0, y = 7;
            PieceColor pieceColor;
            PieceType pieceType;
            foreach (char c in fen)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    pieceColor = PieceColor.White;
                    pieceType = GetPieceFromFen(c.ToString());
                    BoardSquares[x, y].Piece = new ChessPiece() { Color = pieceColor, Type = pieceType};
                    BoardSquares[x, y].Position = new(x, y);
                    x++;
                }
                else if (c >= 'a' && c <= 'z')
                {
                    pieceColor = PieceColor.Black;
                    pieceType = GetPieceFromFen(c.ToString());
                    BoardSquares[x, y].Piece = new ChessPiece() { Color = pieceColor, Type = pieceType };
                    BoardSquares[x, y].Position = new(x, y);
                    x++;
                }
                else if (c >= '0' && c <= '9')
                {
                    for (int i = 0; i < int.Parse(c.ToString()); i++)
                    {
                        BoardSquares[x + i, y].Position = new(x + i, y);
                    }
                    x += int.Parse(c.ToString());
                }
                else
                {
                    y--;
                    x = 0;
                }
            }
            UpdateBoard?.Invoke();
        }

        private PieceType GetPieceFromFen(string c)
        {
            c = c.ToLower();
            return c switch
            {
                "p" => PieceType.Pawn,
                "n" => PieceType.Knight,
                "r" => PieceType.Rook,
                "b" => PieceType.Bishop,
                "q" => PieceType.Queen,
                "k" => PieceType.King
            };
        }
        private BoardSquare[,] CreateBoard()
        {
            SquareColor squareColor = SquareColor.White;  
            BoardSquare[,] boardSquares = new BoardSquare[8,8];
            for (int i = 0; i < 8; i++) 
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        squareColor = SquareColor.Black;
                    }
                    else
                    {
                        squareColor = SquareColor.White;
                    }
                    boardSquares[i, j] = new BoardSquare() { Color = squareColor };
                }
            }
            return boardSquares;
        }

        private SquareColor InverseSquareColor(SquareColor color)
        {
            return color == SquareColor.White ? SquareColor.Black : SquareColor.White;
        }

        public BoardSquare? GetFirstDragSquare()
        {
            foreach (BoardSquare boardSquare in BoardSquares)
            {
                if (boardSquare.IsDragging)
                    return boardSquare;
            }
            return null;
        }

        public void HighlightAvailableToMoveSquares(BoardSquare square, List<ChessMove> availableMoves)
        {
            foreach (ChessMove chessMove in availableMoves)
            {
                if (chessMove.FromX == square.Position.X && chessMove.FromY == square.Position.Y)
                {
                    BoardSquares[chessMove.ToX, chessMove.ToY].CanMoveTo = true;
                }
            }
        }

        public void ChearHighlighting()
        {
            foreach (BoardSquare boardSquare in BoardSquares)
            {
                boardSquare.CanMoveTo = false; 
            }
        }

        public void EndDrag()
        {
            foreach (BoardSquare boardSquare in BoardSquares)
            {
                boardSquare.IsDragging = false;
            }
        }

        public void UpdatePositionFromFen(string newFenPosition)
        {
            int x = 0, y = 7;
            PieceColor pieceColor;
            PieceType pieceType;
            foreach (char c in newFenPosition)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    pieceColor = PieceColor.White;
                    pieceType = GetPieceFromFen(c.ToString());
                    if (BoardSquares[x, y].Piece == null || BoardSquares[x, y].Piece.Type != pieceType || BoardSquares[x, y].Piece.Color != pieceColor)
                    {
                        BoardSquares[x, y].Piece = new ChessPiece() { Color = pieceColor, Type = pieceType };
                        BoardSquares[x, y].Position = new(x, y);
                    }
                    x++;
                }
                else if (c >= 'a' && c <= 'z')
                {
                    pieceColor = PieceColor.Black;
                    pieceType = GetPieceFromFen(c.ToString());
                    if (BoardSquares[x, y].Piece == null || BoardSquares[x, y].Piece.Type != pieceType || BoardSquares[x, y].Piece.Color != pieceColor)
                    {
                        BoardSquares[x, y].Piece = new ChessPiece() { Color = pieceColor, Type = pieceType };
                        BoardSquares[x, y].Position = new(x, y);
                    }
                    x++;
                }
                else if (c >= '0' && c <= '9')
                {
                    for (int i = 0; i < int.Parse(c.ToString()); i++)
                    {
                        if (BoardSquares[x + i, y].Piece != null)
                        {
                            BoardSquares[x + i, y].Piece = null;
                        }
                        BoardSquares[x + i, y].Position = new(x + i, y);
                    }
                    x += int.Parse(c.ToString());
                }
                else
                {
                    y--;
                    x = 0;
                }
            }
            UpdateBoard?.Invoke();
        }
    }
}
