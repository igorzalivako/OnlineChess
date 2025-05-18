using CommunityToolkit.Maui.Views;
using ChessClient.Models.Board;
using ChessEngine;
using System.Runtime.CompilerServices;

namespace ChessClient.Views;

public partial class PromotionPopup : Popup
{
    private readonly PieceColor _color;

    public PromotionPopup(PieceColor color) // "white" или "black"
    {
        this.CanBeDismissedByTappingOutsideOfPopup = false;
        InitializeComponent();
        _color = color;
        LoadImages();
    }

    private void LoadImages()
    {
        if (_color == PieceColor.White)
        {
            QueenButton.Source = "white_queen.png";
            RookButton.Source = "white_rook.png";
            BishopButton.Source = $"white_bishop.png";
            KnightButton.Source = $"white_knight.png";
        }
        else
        {
            QueenButton.Source = "black_queen.png";
            RookButton.Source = "black_rook.png";
            BishopButton.Source = $"black_bishop.png";
            KnightButton.Source = $"black_knight.png";
        }
    }

    private void SelectPiece(string piece)
    {
        Close($"Queen"); // Здесь нужно возвращать выбранную фигуру
    }

    private void Queen_Clicked(object sender, EventArgs e) => Close(Models.PieceType.Queen);
    private void Rook_Clicked(object sender, EventArgs e) => Close(Models.PieceType.Rook);
    private void Bishop_Clicked(object sender, EventArgs e) => Close(Models.PieceType.Bishop);
    private void Knight_Clicked(object sender, EventArgs e) => Close(Models.PieceType.Knight);
}