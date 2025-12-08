using ChessClient.ViewModels;

namespace ChessClient.Views;

// Views/GamePage.xaml.cs
public partial class GamePage : ContentPage
{
    private readonly GameViewModel _vm;

    public GamePage(GameViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        InitializeChessBoard();
    }

    private void InitializeChessBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var frame = new Frame
                {
                    BackgroundColor = (row + col) % 2 == 0 ? Colors.White : Colors.Black,
                    Padding = 0,
                    CornerRadius = 0
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => OnSquareTapped(row, col);
                frame.GestureRecognizers.Add(tapGesture);

                Grid.SetRow(frame, row);
                Grid.SetColumn(frame, col);
                ChessBoard.Children.Add(frame);
            }
        }
    }

    private void OnSquareTapped(int row, int col)
    {
        string square = $"{(char)('a' + col)}{8 - row}";
        _vm.MakeMoveCommand.Execute(square); // В реальном коде нужно обрабатывать выбор фигуры
    }
}