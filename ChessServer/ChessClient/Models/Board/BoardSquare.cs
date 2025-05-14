using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChessClient.Models.Board
{
    public enum SquareColor { Black,  White }
    public partial class BoardSquare : ObservableObject
    {
        [ObservableProperty]
        public SquareColor _color;
        [ObservableProperty]
        public ChessPiece _piece;

        [ObservableProperty]

        private bool _isDragging;

        [ObservableProperty]
        private double _dragX;

        [ObservableProperty]
        private double _dragY;

        [ObservableProperty]
        private BoardPosition _position;

        [ObservableProperty]
        private bool _canMoveTo;

    }
}
