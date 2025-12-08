using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLibrary.Converters;
using ChessEngine;
using ChessLibrary.Models.DTO;
using ChessClient.Utilities.ResourcesLoaders;
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
            _botColor = botColor;
            _botRating = botRating;     
        }

        public async Task CreateAsync()
        {
            string openingBookContent = await OpeningBookLoader.LoadAsync("OpeningBase.txt");
            _chessAi = new AI(openingBookContent, true);
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
            if (_botColor == PieceColor.White)
            {
                Move bestMove = _chessAi.FindBestMove(_chessGame.Position, _chessGame.Position.ActiveColor, _botRating - 500, _botRating);
                _chessGame.ApplyMove(bestMove, _botColor);

                OpponentMove?.Invoke(_chessGame.GetFen(), ConverterToMoveList.ConvertToChessMoveList(_chessGame.GetValidMoves(_chessGame.Position.ActiveColor)));
            }
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
                    EndGame?.Invoke(new EndGameDto(EndGameType.Checkmate, false, "Бот поставил Вам мат. Надо тренироваться", -1));
                }
                else
                {
                    EndGame?.Invoke(new EndGameDto(EndGameType.Checkmate, true, "Объявлен мат боту", -1));
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
                EndGame?.Invoke(new EndGameDto(EndGameType.Stalemate, false, message, -1));
            }
            else
            {
                isEndGame = false;
            }
            return isEndGame;
        }

        public void LeaveCurrentGame()
        {
            string message = "Поражение";
            EndGame?.Invoke(new EndGameDto(EndGameType.UserLeave, false, message, -1));
        }
    }
}
