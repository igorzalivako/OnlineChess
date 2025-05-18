using ChessClient.ViewModels;
using ChessClient.Utilities.Converters;
using ChessClient.Models.Board;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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

        BoardAbsoluteLayout.SizeChanged += (s, e) =>
        {
            ViewModel.CellSize = BoardAbsoluteLayout.Width / BoardSize;
        };
    }

    /*protected override async void OnAppearing()
    {
        base.OnAppearing();
        ((GameViewModel)BindingContext).Initialize();
    }*/

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

    private Point GetAbsolutePositionForCell(BoardSquare square)
    {
        var index = ViewModel.FlatBoard.IndexOf(square);
        if (index < 0) return new Point(0, 0);
        int row = index / BoardSize;
        int col = index % BoardSize;
        return new Point(col * ViewModel.CellSize, row * ViewModel.CellSize);
    }

    private void OnPiecePanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is Image image && image.BindingContext is BoardSquare square && ViewModel.IsBoardActive)
        {
            var absLayout = BoardAbsoluteLayout;

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
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.OnDisappearing();   
    }

    private GameViewModel ViewModel => BindingContext as GameViewModel;

}