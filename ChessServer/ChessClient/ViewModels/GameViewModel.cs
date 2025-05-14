using ChessEngine;
using ChessClient.Services;
using ChessClient.Models;
using ChessLibrary.Models.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using ChessClient.Models.Board;
using System.Collections.ObjectModel;
using ChessClient.Views;
using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace ChessClient.ViewModels;

[QueryProperty(nameof(Username), "username")]
[QueryProperty(nameof(Rating), "rating")]
[QueryProperty(nameof(Minutes), "minutes")]
public partial class GameViewModel : ObservableObject
{
    private GameService _gameService;
    private readonly ChessBoardModel _chessBoard;
    private string _currentGameId;
    private LoadingPopup _loadingPopup;

    [ObservableProperty]
    private PieceColor _playerColor;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ожидание начала игры";

    [ObservableProperty]
    private bool _isGameActive;

    [ObservableProperty]
    private string _username;
    
    [ObservableProperty]
    private string _rating;

    [ObservableProperty]
    private string _minutes;

    [ObservableProperty]
    private ObservableCollection<BoardSquare> _flatBoard = new();

    [ObservableProperty]
    private ObservableCollection<BoardSquare> _testFlat = new() {
        new BoardSquare() { Color = SquareColor.Black, Piece = new ChessPiece { Color = PieceColor.Black, Type = Models.PieceType.Pawn } },
        new BoardSquare() { Color = SquareColor.White, Piece = new ChessPiece { Color = PieceColor.Black, Type = Models.PieceType.Pawn } } };

    [ObservableProperty]
    private string _opponentUsername;

    [ObservableProperty]
    private int _opponentRating;

    [ObservableProperty]
    private List<ChessMove> _availableMoves;

    [ObservableProperty]
    private List<ChessMove> _activePieceAvailableMoves;

    [ObservableProperty]
    private bool _isBoardActive;

    public GameViewModel(ChessBoardModel chessBoard)
    {
        _chessBoard = chessBoard;
        _chessBoard.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ChessBoardModel.BoardSquares))
                UpdateFlatBoard();
        };
        UpdateFlatBoard();
    }

    private void UpdateFlatBoard()
    {
        ObservableCollection<BoardSquare> _newFlatBoard = new();
        if (PlayerColor == PieceColor.White)
        {
            for (int j = 7; j >= 0; j--)
            {
                for (int i = 0; i < 8; i++)
                {
                    _newFlatBoard.Add(_chessBoard[i, j]);
                }
            }
        }
        else
        {
            for (int j = 0; j < 8; j++)
            {
                for (int i = 7; i>= 0; i--)
                {
                    _newFlatBoard.Add(_chessBoard[i, j]);
                }
            }
        }
            
        FlatBoard = _newFlatBoard;
    }

    private void InitializeEventHandlers()
    {
        _gameService.MatchFound += OnMatchFound;
        _gameService.QueueJoined += OnQueueJoined;
        _gameService.MoveVerified += OnMoveVerified;
        //_gameService.GameStateUpdated += OnGameStateUpdated;
        _gameService.OpponentMove += OnOpponentMove;
        _gameService.ErrorOccurred += OnError;
    }

    private void OnMoveVerified(ChessMove move, bool isCorrect)
    {
        if (!isCorrect)
        {
            IsLoading = false;
            IsBoardActive = true;
        }
        else
        {
            _chessBoard.MakeMove(move);
        }
    }

    private void OnQueueJoined(string obj)
    {
        Debug.WriteLine($"заработало сасааааать {obj}");
    }

    private async Task JoinQueue(int minutes)
    {
        IsLoading = true;
        StatusMessage = "Поиск соперника...";
        await _gameService.JoinMatchmakingQueue(minutes);
    }

    private async Task SendMove(ChessMove move)
    {
        if (!IsGameActive) 
            return;
        IsLoading = true;
        await _gameService.ApplyMove(move);
    }

    private async void OnMatchFound(MatchFoundDto matchFoundDto)
    {
        _currentGameId = matchFoundDto.GameId;
        IsGameActive = true;
        IsLoading = false;
        _chessBoard.PlayerColor = matchFoundDto.YourColor == "white" ? PieceColor.White : PieceColor.Black;
        PlayerColor = _chessBoard.PlayerColor;
        IsBoardActive = PlayerColor == PieceColor.White;
        _chessBoard.LoadPositionFromFen(matchFoundDto.Position);
        OpponentUsername = matchFoundDto.OpponentUsername;
        OpponentRating = matchFoundDto.OpponentRating;
        AvailableMoves = matchFoundDto.AvailableMoves;
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            _loadingPopup.Close(); // Используем синхронный метод Close()
            _loadingPopup = null;
        });
    }

    private void OnGameStateUpdated(string fen)
    {
        // Обновляем модель доски
        _chessBoard.LoadPositionFromFen(fen);

        // Если нужно, можно обновить UI
        OnPropertyChanged(nameof(ChessBoard));
    }

    private void OnOpponentMove(ChessMove move, List<ChessMove> possibleMoves)
    {
        // Обновление доски через ChessBoardModel
        _chessBoard.MakeMove(move);
        AvailableMoves = possibleMoves;
    }

    private void OnError(string error)
    {
        StatusMessage = $"Ошибка: {error}";
        IsLoading = false;
    }

    public ChessBoardModel ChessBoard => _chessBoard;

    [RelayCommand]
    private async Task Surrender()
    {
        if (!IsGameActive) return;

        //await _gameService.LeaveGame();
        IsGameActive = false;
        StatusMessage = "Вы сдались";
    }
    /*
    [RelayCommand]
    private void DragStarting(BoardPosition fromPosition)
    {
        var square = _chessBoard[fromPosition.X, fromPosition.Y];
        square.IsDragging = true;
        
    }
    */
    private void UpdateActivePieceAvailableMoves(BoardSquare square)
    {
        _chessBoard.HighlightAvailableToMoveSquares(square, AvailableMoves);
    }
    /*
    [RelayCommand]
    private void DragOver(BoardPosition overPosition)
    {
        // Логика при наведении на клетку (опционально)
    }

    [RelayCommand]
    private void Drop(BoardPosition toPosition)
    {
        var fromSquare = _chessBoard.GetFirstDragSquare();
        if (fromSquare != null)
        {
            fromSquare.IsDragging = false;
            
            //var move = new ChessMove(fromSquare.Position, toPosition);
           // SendMoveCommand.Execute(move);
        }
    }
    */
    public void UnsubscribeEvents()
    {
        _gameService.MatchFound -= OnMatchFound;
        //_gameService.GameStateUpdated -= OnGameStateUpdated;
        //_gameService.OpponentMoveMade -= OnOpponentMove;
        _gameService.ErrorOccurred -= OnError;
    }

    public async Task Initialize()
    {
        string jwtToken = await SecureStorage.GetAsync("jwt_token");
        _gameService = new GameService(AppConfig.BaseUrl, jwtToken);
        _gameService.ConnectAsync();
        InitializeEventHandlers();
        _loadingPopup = new LoadingPopup();
        Shell.Current.CurrentPage.ShowPopup(_loadingPopup);
        JoinQueue(int.Parse(Minutes));
    }

    // кастомный DragAndDrop
    [ObservableProperty]
    private BoardSquare? _draggingSquare;

    [ObservableProperty]
    private Point _draggingPosition; // Пиксельная позиция для смещения

    public void StartDrag(BoardSquare square)
    {
        DraggingSquare = square;
        square.IsDragging = true;
        UpdateActivePieceAvailableMoves(square);
    }

    public void UpdateDrag(Point position)
    {
        DraggingPosition = position;
    }

    public void EndDrag(Point absolutePosition, Func<Point, BoardSquare?> getSquareAtPoint)
    {
        if (DraggingSquare is { } fromSquare)
        {
            fromSquare.IsDragging = false;
            // Определяем клетку, куда отпустили пальцем
            var targetSquare = getSquareAtPoint(absolutePosition);
            if (targetSquare != null && targetSquare != fromSquare && targetSquare.CanMoveTo)
            {
                targetSquare.Piece = fromSquare.Piece;
                fromSquare.Piece = null;
                // Здесь формируем ход — пример:
                ChessMove move = new ChessMove { FromX = fromSquare.Position.X, FromY = fromSquare.Position.Y, ToX = targetSquare.Position.X, ToY = targetSquare.Position.Y };
                SendMove(move);
            }
        }
        DraggingSquare = null;
        DraggingPosition = new Point(0, 0);
        _chessBoard.ChearHighlighting();
    }

    [ObservableProperty]
    private double _cellSize;

    [ObservableProperty]
    private Rect _draggingRect;

    partial void OnDraggingPositionChanged(Point oldValue, Point newValue)
    {
        DraggingRect = new Rect(newValue, new Size(CellSize));
    }
}