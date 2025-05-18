using ChessEngine;
using ChessClient.Models;
using ChessLibrary.Models.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChessClient.Models.Board;
using System.Collections.ObjectModel;
using ChessClient.Views;
using CommunityToolkit.Maui.Views;
using System.Timers;
using CommunityToolkit.Maui.Core;
using ChessClient.Services;

namespace ChessClient.ViewModels;

[QueryProperty(nameof(Username), "username")]
[QueryProperty(nameof(Rating), "rating")]
[QueryProperty(nameof(Minutes), "minutes")]
[QueryProperty(nameof(GameModeName), "game_mode")]
[QueryProperty(nameof(BotComplexity), "complexity")]
[QueryProperty(nameof(BotColor), "bot_color")]
public partial class GameViewModel : ObservableObject
{
    private GameService _gameService;
    private BotService _botService;
    private readonly ChessBoardModel _chessBoard;
    private string _currentGameId;
    private LoadingPopup _loadingPopup;
    private EndGamePopup _endGamePopup;

    private bool _isInitialized;
    partial void OnUsernameChanged(string oldValue, string newValue) => TryInitialize();
    partial void OnRatingChanged(string oldValue, string newValue) => TryInitialize();
    partial void OnMinutesChanged(string oldValue, string newValue) => TryInitialize();
    partial void OnGameModeNameChanged(string oldValue, string newValue) => TryInitialize();
    partial void OnBotComplexityChanged(string oldValue, string newValue) => TryInitialize();
    partial void OnBotColorChanged(string oldValue, string newValue) => TryInitialize();

    private void TryInitialize()
    {
        if (_isInitialized) return;
        if (!(Username is null) && !(Rating is null) && !(Minutes is null)
            && !(GameModeName is null) && !(BotComplexity is null) && !(BotColor is null))
        {
            _isInitialized = true;
            Initialize();
        }
    }

    private GameMode Mode { get; set; }

    [ObservableProperty]
    private string _botComplexity;

    [ObservableProperty]
    private string _botColor;

    [ObservableProperty]
    private bool _isBotGame;

    [ObservableProperty]
    private string _gameModeName;

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
    private string _opponentRating;
    
    [ObservableProperty]
    private List<ChessMove> _availableMoves;

    [ObservableProperty]
    private List<ChessMove> _activePieceAvailableMoves;

    [ObservableProperty]
    private bool _isBoardActive;

    [ObservableProperty]
    private PieceColor _activePlayerColor;

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
        _gameService.EndGame += OnEndGame;
        _gameService.UpdateTimers += OnUpdateTimers;
        _gameService.OpponentMove += OnOpponentMove;
        _gameService.ErrorOccurred += OnError;
    }

    private void OnUpdateTimers(int whiteLeftTime, int blackLeftTime)
    {
        SetTimers(whiteLeftTime, blackLeftTime);
    }

    private void OnEndGame(EndGameDto endGameDto)
    {
        if (endGameDto.EndGameType == EndGameType.Checkmate)
        {
            if (endGameDto.YouWon)
            {
                _endGamePopup = new EndGamePopup($"Объявлен мат игроку {OpponentUsername}",
                                                 "Вы победили!", 
                                                 "", 
                                                 300, 
                                                 200, 
                                                 endGameDto.EndGameType, 
                                                 endGameDto.YouWon);
            }
            else
            {
                _endGamePopup = new EndGamePopup($"Вам объявлен мат",
                                                 "Вы проиграли",
                                                 "",
                                                 300,
                                                 200,
                                                 endGameDto.EndGameType,
                                                 endGameDto.YouWon);
            }
        }
        else if (endGameDto.EndGameType == EndGameType.Stalemate)
        {
            _endGamePopup = new EndGamePopup(endGameDto.Message,
                                 "Ничья",
                                 "",
                                 300,
                                 200,
                                 endGameDto.EndGameType,
                                 endGameDto.YouWon);
        }
        else if (endGameDto.EndGameType == EndGameType.EndTime)
        {
            _endGamePopup = new EndGamePopup(endGameDto.Message,
                                 "Закончилось время",
                                 "",
                                 300,
                                 200,
                                 endGameDto.EndGameType,
                                 endGameDto.YouWon);
        }
        else
        {
            _endGamePopup = new EndGamePopup(endGameDto.Message,
                                             endGameDto.YouWon ? "Противник сдался" : "Вы сдались",
                                             "",
                                             300,
                                             200,
                                             endGameDto.EndGameType,
                                             endGameDto.YouWon);
        }
            _endGamePopup.Closed += OnEndGamePopupClosed;
        IsGameActive = false;
        Thread.Sleep(1000);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Shell.Current.CurrentPage.ShowPopup(_endGamePopup);
        });
    }

    private async void OnEndGamePopupClosed(object? sender, PopupClosedEventArgs e)
    {
        _endGamePopup.Closed -= OnEndGamePopupClosed;
        if (IsBotGame)
        {
            UnsubscribeBotEvents();
            await Shell.Current.GoToAsync($"///MainPage");
        }
        else
        {
            UnsubscribeEvents();
            await _gameService.DisposeAsync();
            await Shell.Current.GoToAsync($"///MainPage?username={Username}&rating={Rating}");
        }

        // тут еще добавить обновление рейтинга!!!
        
        
    }

    private void UnsubscribeBotEvents()
    {
        _botService.MatchFound -= OnMatchFound;
        _botService.OpponentMove -= OnOpponentMove;
        _botService.EndGame -= OnEndGame;
        _botService.MoveVerified -= OnMoveVerified;
    }

    private void OnMoveVerified(string newFenPosition, bool isCorrect)
    {
        if (!isCorrect)
        {
            IsLoading = false;
            IsBoardActive = true;
        }
        else
        {
            _chessBoard.UpdatePositionFromFen(newFenPosition);
            ActivePlayerColor = InvertColor(ActivePlayerColor);
        }
        _chessBoard.EndDrag();
    }

    private void OnQueueJoined(string obj)
    {
        //Debug.WriteLine($"заработало сасааааать {obj}");
    }

    private async Task JoinQueue(int minutes)
    {
        IsLoading = true;
        await _gameService.JoinMatchmakingQueue(minutes);
    }

    private async Task SendMove(ChessMove move)
    {
        if (!IsGameActive) 
            return;
        IsLoading = true;
        IsBoardActive = false;
        await ProcessPromotion(move);
        if (IsBotGame)
        {
            _botService.ApplyMove(move);
        }
        else
        {
            await _gameService.ApplyMove(move);
        }
    }

    private async Task LeaveGame()
    {
        await _gameService.LeaveCurrentGame();
    }

    private async Task<ChessMove> ProcessPromotion(ChessMove move)
    {
        BoardSquare fromSquare = _chessBoard[move.FromX, move.FromY];
        if (fromSquare.Piece != null && fromSquare.Piece.Type == Models.PieceType.Pawn)
        {
            if (PlayerColor == PieceColor.White && move.ToY == 7
                || PlayerColor == PieceColor.Black && move.ToY == 0)
            {
                Models.PieceType promoteType = await SelectPromotePiece();
                move.PromoteType = (DtoPieceType)promoteType;
            }
        }
        return move;
    }

    private async Task<Models.PieceType> SelectPromotePiece()
    {
        var popup = new PromotionPopup(PlayerColor);
        var page = Application.Current.MainPage;

        Models.PieceType result = Models.PieceType.Queen;
        await page.Dispatcher.DispatchAsync(async () =>
        {
            result = (Models.PieceType)await page.ShowPopupAsync(popup);
        });
        return result;
    }

    private async void OnMatchFound(MatchFoundDto matchFoundDto)
    {
        

        _currentGameId = matchFoundDto.GameId;
        IsGameActive = true;
        IsLoading = false;
        _chessBoard.PlayerColor = matchFoundDto.YourColor == "white" ? PieceColor.White : PieceColor.Black;
        PlayerColor = _chessBoard.PlayerColor;
        ActivePlayerColor = PieceColor.White;
        IsBoardActive = PlayerColor == PieceColor.White;
        _chessBoard.LoadPositionFromFen(matchFoundDto.Position);
        OpponentUsername = matchFoundDto.OpponentUsername;
        OpponentRating = matchFoundDto.OpponentRating.ToString();
        AvailableMoves = matchFoundDto.AvailableMoves;

        if (!IsBotGame)
        {
            SetTimers(matchFoundDto.WhiteLeftTime, matchFoundDto.BlackLeftTime);
            StartTimer();

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (_loadingPopup != null)
                {
                    await _loadingPopup.CloseAsync(); // Асинхронное закрытие
                }
                _loadingPopup = null;
            });
        }
    }

    private void SetTimers(int whiteLeftTime, int blackLeftTime)
    {
        if (PlayerColor == PieceColor.White)
        {
            PlayerTimeLeft = whiteLeftTime;
            OpponentTimeLeft = blackLeftTime;
        }
        else
        {
            PlayerTimeLeft = blackLeftTime;
            OpponentTimeLeft = whiteLeftTime;   
        }
    }

    /*
private void OnGameStateUpdated(string fen)
{
   // Обновляем модель доски
   _chessBoard.LoadPositionFromFen(fen);

   // Если нужно, можно обновить UI
   OnPropertyChanged(nameof(ChessBoard));
}
*/
    private void OnOpponentMove(string newFenPosition, List<ChessMove> possibleMoves)
    {
        // Обновление доски через ChessBoardModel
        _chessBoard.UpdatePositionFromFen(newFenPosition);
        AvailableMoves = possibleMoves;
        IsBoardActive = true;
        IsLoading = false;
        ActivePlayerColor = InvertColor(ActivePlayerColor);
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
        if (!IsGameActive) 
            return;

        await _gameService.LeaveCurrentGame();
    }

    private void UpdateActivePieceAvailableMoves(BoardSquare square)
    {
        _chessBoard.HighlightAvailableToMoveSquares(square, AvailableMoves);
    }

    public void UnsubscribeEvents()
    {
        _gameService.MatchFound -= OnMatchFound;
        _gameService.QueueJoined -= OnQueueJoined;
        _gameService.MoveVerified -= OnMoveVerified;
        _gameService.EndGame -= OnEndGame;
        _gameService.UpdateTimers -= OnUpdateTimers;
        _gameService.OpponentMove -= OnOpponentMove;
        _gameService.ErrorOccurred -= OnError;
    }

    public async Task Initialize()
    {
        Mode = GetCurrentGameMode();
        if (Mode == GameMode.Online)
        {
            string jwtToken = await SecureStorage.GetAsync("jwt_token");
            _gameService = new GameService(AppConfig.BaseUrl, jwtToken);
            _gameService.ConnectAsync();
            InitializeEventHandlers();
            _loadingPopup = new LoadingPopup();
            Shell.Current.CurrentPage.ShowPopup(_loadingPopup);
            JoinQueue(int.Parse(Minutes));
        }
        else
        {
            InitializeBotGame();
        }
    }

    private void InitializeBotGame()
    {
        IsBotGame = true;
        PieceColor botColor;
        int botRating;
        (botColor, botRating) = GetBotSettings();
        _botService = new BotService(botColor, botRating);
        InitializeBotEventHandlers();
        _botService.CreateGame(Username, Rating);       
    }

    private void InitializeBotEventHandlers()
    {
        _botService.MatchFound += OnMatchFound;
        _botService.OpponentMove += OnOpponentMove;
        _botService.EndGame += OnEndGame;
        _botService.MoveVerified += OnMoveVerified;
    }

    private (PieceColor, int) GetBotSettings()
    {
        //OpponentUsername = "Бот";
        //OpponentRating = BotComplexity;
        var botColor = BotColor == "white" ? PieceColor.White : PieceColor.Black;
        int botRating = int.Parse(BotComplexity);
        //PlayerColor = InvertColor(botColor);
        return (botColor, botRating);
    }

    private GameMode GetCurrentGameMode()
    {
        if (GameModeName == "online")
        {
            return GameMode.Online;
        }
        else
        {
            return GameMode.Bot;
        }
    }

    // кастомный DragAndDrop
    [ObservableProperty]
    private BoardSquare? _draggingSquare;

    [ObservableProperty]
    private Point _draggingPosition; // Пиксельная позиция для смещения

    public void StartDrag(BoardSquare square)
    {
        if (square.Piece != null && square.Piece.Color == PlayerColor)
        {
            DraggingSquare = square;
            square.IsDragging = true;
            UpdateActivePieceAvailableMoves(square);
        }
    }

    public void UpdateDrag(Point position)
    {
        DraggingPosition = position;
    }

    public void EndDrag(Point absolutePosition, Func<Point, BoardSquare?> getSquareAtPoint)
    {
        if (DraggingSquare is { } fromSquare)
        {
            // fromSquare.IsDragging = false;
            // Определяем клетку, куда отпустили пальцем
            var targetSquare = getSquareAtPoint(absolutePosition);
            if (targetSquare != null && targetSquare != fromSquare && targetSquare.CanMoveTo)
            {
                // Здесь формируем ход
                ChessMove move = new ChessMove { FromX = fromSquare.Position.X, FromY = fromSquare.Position.Y, ToX = targetSquare.Position.X, ToY = targetSquare.Position.Y };
                SendMove(move);
            }
            else
            {
                fromSquare.IsDragging = false;
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

    // Cистема для работы со временем
    private System.Timers.Timer? _timer;

    [ObservableProperty]
    private int _playerTimeLeft;

    [ObservableProperty]
    private int _opponentTimeLeft;

    // Форматированное отображение времени для биндинга
    public string PlayerTimeFormatted => FormatTime(PlayerTimeLeft);
    public string OpponentTimeFormatted => FormatTime(OpponentTimeLeft);

    partial void OnPlayerTimeLeftChanged(int value)
    {
        OnPropertyChanged(nameof(PlayerTimeFormatted));
    }

    partial void OnOpponentTimeLeftChanged(int value)
    {
        OnPropertyChanged(nameof(OpponentTimeFormatted));
    }

    // Запуск таймера
    [RelayCommand]
    public void StartTimer()
    {
        StopTimer();
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerTick;
        _timer.Start();
    }

    // Остановка таймера
    [RelayCommand]
    public void StopTimer()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }
    }

    private async void OnTimerTick(object? sender, ElapsedEventArgs e)
    {
        if (ActivePlayerColor == PlayerColor)
        {
            if (PlayerTimeLeft > 0)
            {
                PlayerTimeLeft--;
                if (PlayerTimeLeft == 0)
                {
                    StopTimer();
                    await OnTimeOut();
                }
            }
        }
        else
        {
            if (OpponentTimeLeft > 0)
            {
                OpponentTimeLeft--;
                if (OpponentTimeLeft == 0)
                {
                    StopTimer();
                }
            }
        }
    }

    // Этот метод вызывается при истечении времени у игрока
    private async Task OnTimeOut()
    {
        try
        {
            await _gameService.EndTime();
        }
        catch (Exception ex)
        {
            
        }
    }

    // Форматирование времени в MM:SS
    public static string FormatTime(int seconds)
    {
        int m = seconds / 60;
        int s = seconds % 60;
        return $"{m:D2}:{s:D2}";
    }


    private PieceColor InvertColor(PieceColor color)
    {
        return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    public void OnDisappearing()
    {
        //LeaveGame();
        if (!IsBotGame)
        {
            UnsubscribeEvents();
        }
    }
}