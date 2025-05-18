using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLibrary.Converters;
using ChessEngine;
using ChessLibrary.Models.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace ChessClient.Services
{
    public class BotService
    {
        // События для клиента
        public event Action<MatchFoundDto> MatchFound;
        public event Action<ChessMove[]> FoundAvailableMoves;
        public event Action<ChessMove> AcceptMove;
        public event Action<string, List<ChessMove>> OpponentMove;
        public event Action<int> LeaveGame;
        public event Action<EndGameDto> EndGame;
        public event Action<string> ErrorOccurred;
        public event Action<string> QueueJoined;
        public event Action<string, bool> MoveVerified;
        public event Action<int, int> UpdateTimers;

        private ChessGame _chessGame;
        private AI _chessAi;
        private PieceColor _botColor;
        private int _botRating;

        public BotService(PieceColor botColor, int botRating)
        {
            _chessGame = new ChessGame();
            _chessGame.Position = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
            _chessAi = new AI("OpeningBase.txt");
            _botColor = botColor;
            _botRating = botRating;     
        }

        /*
        private void HandleUpdateTimers(int whiteLeftTime, int blackLeftTime)
        {
            UpdateTimers?.Invoke(whiteLeftTime, blackLeftTime);
        }

        private void HandleQueueJoined(string obj)
        {
            QueueJoined?.Invoke($"prishlo {obj}");
        }
        */
        /*public async Task ConnectAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Connection failed: {ex.Message}");
            }
        }*/

        // Основные методы взаимодействия

        public void CreateGame(string username, string userRating)
        {
            string playerColor;
            if (_botColor == PieceColor.White)
            {
                playerColor = "black";
            }
            else
            {
                playerColor = "white";
            }
            var availableMoves = _chessGame.GetValidMoves(PieceColor.White);
            MatchFound?.Invoke(new MatchFoundDto() {
                Position = _chessGame.GetFen(),
                YourColor = playerColor,
                OpponentUsername = "Бот",
                OpponentRating = _botRating,
                AvailableMoves = ConverterToMoveList.ConvertToChessMoveList(availableMoves),
            });
        }

        public async Task ApplyMove(ChessMove move)
        {
            try
            {
                var callerColor = _botColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
                Move engineMove = ConverterToEngineMove.ConvertToEngineMove(move, callerColor);
                bool isMoveCorrect = _chessGame.ApplyMove(engineMove, callerColor);
                MoveVerified?.Invoke(_chessGame.GetFen(), isMoveCorrect);
                if (isMoveCorrect)
                {
                    bool isEndGame = ProcessEndGame(false);

                    if (!isEndGame)
                    {
                        _ = Task.Run(() =>
                        {
                            Move bestMove = _chessAi.FindBestMove(_chessGame.Position, _chessGame.Position.ActiveColor, _botRating - 500, _botRating);
                            _chessGame.ApplyMove(bestMove, _botColor);

                            OpponentMove?.Invoke(_chessGame.GetFen(), ConverterToMoveList.ConvertToChessMoveList(_chessGame.GetValidMoves(_chessGame.Position.ActiveColor)));
                            
                            isEndGame = ProcessEndGame(true);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Move failed: {ex.Message}");
            }
        }

        private bool ProcessEndGame(bool isPlayerMove)
        {
            bool isEndGame = true;
            if (_chessGame.IsCheckmate)
            {
                if (isPlayerMove)
                {
                    EndGame?.Invoke(new EndGameDto(EndGameType.Checkmate, false, "Бот поставил Вам мат. Надо тренироваться"));
                }
                else
                {
                    EndGame?.Invoke(new EndGameDto(EndGameType.Checkmate, true, "Объявлен мат боту"));
                }
            }
            else if (_chessGame.IsStalemate)
            {
                string message;
                if (_chessGame.AvailableMoves != null && _chessGame.AvailableMoves.Size == 0)
                {
                    message = "Объявлен пат";
                }
                else
                {
                    message = "Ничья по причине повторения позиций";
                }
                EndGame?.Invoke(new EndGameDto(EndGameType.Stalemate, false, message));
            }
            else
            {
                isEndGame = false;
            }
            return isEndGame;
        }
    }

        /*public async Task GetAvailableMoves(string position)
        {
            try
            {
                await _hubConnection.InvokeAsync("GetAvailableMoves", position);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Get moves failed: {ex.Message}");
            }
        }

        public async Task ApplyMove(ChessMove move)
        {
            try
            {
                await _hubConnection.InvokeAsync("ApplyMove", _currentGameId, move);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Move failed: {ex.Message}");
            }
        }

        public async Task LeaveCurrentGame()
        {
            try
            {
                await _hubConnection.InvokeAsync("LeaveGame", _currentGameId);
                _currentGameId = null;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Leave failed: {ex.Message}");
            }
        }

        // Обработчики событий хаба

        private void HandleMatchFound(MatchFoundDto match)
        {
            _currentGameId = match.GameId;
            MatchFound?.Invoke(match);

            // Автоматически присоединяемся к группе игры
            _ = _hubConnection.InvokeAsync("JoinGame", match.GameId);
        }

        private void HandleFoundAvailableMoves(ChessMove[] moves)
        {
            FoundAvailableMoves?.Invoke(moves);
        }

        private void HandleAcceptMove(ChessMove move)
        {
            AcceptMove?.Invoke(move);
        }

        private void HandleOpponentMove(string newFenPosition, List<ChessMove> possibleMoves)
        {
            OpponentMove?.Invoke(newFenPosition, possibleMoves);
        }

        private void HandleLeaveGame(int userId)
        {
            LeaveGame?.Invoke(userId);
            _currentGameId = null;
        }

        private void HandleEndGame(EndGameDto endGameDto)
        {
            EndGame?.Invoke(endGameDto);
            //_currentGameId = null;
        }

        // Обработка разрыва соединения
        private Task HandleConnectionClosed(Exception ex)
        {
            ErrorOccurred?.Invoke(ex?.Message ?? "Connection closed");
            return Task.CompletedTask;
        }

        private Task HandleMoveVerification(string newFenPosition, bool isCorrect)
        {
            MoveVerified.Invoke(newFenPosition, isCorrect);
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }

        public async Task EndTime()
        {
            try
            {
                await _hubConnection.InvokeAsync("EndTime", _currentGameId);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"EndTime failed: {ex.Message}");
            }
        }*/
}
