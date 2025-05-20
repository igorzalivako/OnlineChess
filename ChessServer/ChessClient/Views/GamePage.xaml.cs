using ChessClient.ViewModels;
using ChessClient.Utilities.Converters;
using ChessClient.Models.Board;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using ChessClient.Controls;
using CommunityToolkit.Maui.Core;

namespace ChessClient.Views;

public partial class GamePage : ContentPage, INotifyPropertyChanged
{
    // Для простоты — размер клетки
    private const int BoardSize = 8;

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public GamePage(GameViewModel gameViewModel)
    {
        InitializeComponent();
        BindingContext = gameViewModel;

        AspectContainer.SizeChanged += (s, e) =>
        {
            double size = Math.Min(AspectContainer.Width, AspectContainer.Height) / 8.0;
            gameViewModel.CellSize = size;
        };

        gameViewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(gameViewModel.FlatBoard))
            {
                ChessBoardGraphicsView.Invalidate();
            }
        };
    }

    /*protected override async void OnAppearing()
    {
        base.OnAppearing();
        ((GameViewModel)BindingContext).Initialize();
    }*/
    /*
    private BoardSquare? GetSquareAtPoint(Point absPos)
    {
        // Определяем индекс клетки по координатам пальца
        int col = (int)((absPos.X + ViewModel.CellSize / 2) / ViewModel.CellSize);
        int row = (int)((absPos.Y + ViewModel.CellSize / 2) / ViewModel.CellSize);

        // Преобразуем в индекс FlatBoard (зависит от порядка FlatBoard!)
        int index = row * BoardSize + col;
        if (index >= 0 && index < ViewModel.FlatBoard.Count)
            return ViewModel.FlatBoard[index];
        return null;
    }
    */
    public BoardSquare? GetSquareAtPoint(Point pos)
    {
        int x = (int)(pos.X / ViewModel.CellSize);
        int y = (int)(pos.Y / ViewModel.CellSize);
        if (x < 0 || x > 7 || y < 0 || y > 7)
            return null;
        // В твоем FlatBoard порядок зависит от цвета игрока!
        return ViewModel.FlatBoard[y * 8 + x];
    }
    private Point GetAbsolutePositionForCell(BoardSquare square)
    {
        var index = ViewModel.FlatBoard.IndexOf(square);
        if (index < 0) return new Point(0, 0);
        int row = index / BoardSize;
        int col = index % BoardSize;
        return new Point(col * ViewModel.CellSize, row * ViewModel.CellSize);
    }

    void OnBoardTouchStart(object? sender, TouchEventArgs e)
    {
        if (!(BindingContext is GameViewModel vm) || !vm.IsBoardActive)
            return;
        var touch = e.Touches.FirstOrDefault();
        if (touch == null) return;
        var pos = new Point(touch.X, touch.Y);

        var sq = GetSquareAtPoint(pos);
        vm.StartDrag(sq, pos);
        ChessBoardGraphicsView.Invalidate();
    }

    void OnBoardTouchDrag(object? sender, TouchEventArgs e)
    {
        if (!(BindingContext is GameViewModel vm) || !vm.IsBoardActive)
            return;
        var touch = e.Touches.FirstOrDefault();
        if (touch == null) return;
        var pos = new Point(touch.X, touch.Y);

        vm.UpdateDrag(pos);
        ChessBoardGraphicsView.Invalidate();
    }

    void OnBoardTouchEnd(object? sender, TouchEventArgs e)
    {
        if (!(BindingContext is GameViewModel vm) || !vm.IsBoardActive)
            return;
        var touch = e.Touches.FirstOrDefault();
        if (touch == null) return;
        var pos = new Point(touch.X, touch.Y);

        vm.EndDrag(pos, GetSquareAtPoint);
        ChessBoardGraphicsView.Invalidate();
    }

    /*private void OnPiecePanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is Image image && image.BindingContext is BoardSquare square && ViewModel.IsBoardActive)
        {
            //var absLayout = BoardAbsoluteLayout;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    // Получаем позицию клетки
                    var start = GetAbsolutePositionForCell(square);
                    Debug.WriteLine($"Начальная позиция: {start}");
                    ViewModel.StartDrag(square);
                    ViewModel.UpdateDrag(start);
                    break;

                case GestureStatus.Running:
                    // Смещаем фигуру
                    var startPos = GetAbsolutePositionForCell(square);
                    var newPos = new Point(startPos.X + e.TotalX, startPos.Y + e.TotalY);
                    Debug.WriteLine($"Новая позиция: {newPos}");
                    ViewModel.UpdateDrag(newPos);
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    // Отпустили — определяем, на какую клетку попали
                    var dropPos = ViewModel.DraggingPosition;
                    Debug.WriteLine($"Конечная позиция: {dropPos}");
                    ViewModel.EndDrag(dropPos, GetSquareAtPoint);
                    break;
            }
        }
    }*/

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.OnDisappearing();   
    }

    private GameViewModel ViewModel => BindingContext as GameViewModel;

    private void Button_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Нажата кнопка");
    }

    private void OnChessBoardSizeChanged(object sender, EventArgs e)
    {
        ViewModel.CellSize = Math.Min(
            ChessBoardGraphicsView.Width,
            ChessBoardGraphicsView.Height
        ) / 8;
        ChessBoardGraphicsView.Invalidate(); // Форсируем перерисовку
    }

}