using ChessClient.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class GameViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ChessPiece> squares;

    public GameViewModel()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        Squares = new ObservableCollection<ChessPiece>();

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                bool isWhite = (row + col) % 2 == 0;
                /*Squares.Add(new ChessSquare
                {
                    Color = isWhite ? Colors.White : Colors.Black,
                    Piece = PieceType.Pawn, // Здесь можно добавить начальную расстановку фигур
                    Row = row,
                    Column = col
                });*/
            }
        }
    }
}