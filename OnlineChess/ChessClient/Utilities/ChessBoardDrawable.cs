using ChessClient.Models;
using ChessClient.Models.Board;
using ChessClient.ViewModels;
using ChessClient.Utilities.ResourcesLoaders;
using Microsoft.Maui.Graphics.Platform;

namespace ChessClient.Utilities
{
    // ChessBoardDrawable.cs
    public class ChessBoardDrawable : IDrawable
    {
        public GameViewModel? ViewModel { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (ViewModel == null || ViewModel.FlatBoard.Count != 64)
                return;

            double cellSize = ViewModel.CellSize;
            int x;
            int y;
            // Рисуем клетки и подсветку
            for (int i = 0; i < 64; i++)
            {
                var sq = ViewModel.FlatBoard[i];
                x = i % 8;
                y = i / 8;
                if (sq != null)
                {
                    // Для белых и черных — стандартная шахматная доска
                    canvas.FillColor = sq.Color == SquareColor.White ? Colors.Beige : Colors.Brown;
                    canvas.FillRectangle((float)(x * cellSize), (float)(y * cellSize), (float)cellSize, (float)cellSize);

                    // Подсветка хода
                    if (sq.CanMoveTo)
                    {
                        canvas.FillColor = Colors.Gray.WithAlpha(0.8f);
                        // Кружок в центре
                        float margin = (float)(cellSize * 0.28);
                        canvas.FillEllipse(
                            (float)(x * cellSize + margin),
                            (float)(y * cellSize + margin),
                            (float)(cellSize * 0.44),
                            (float)(cellSize * 0.44));
                    }
                }
            }

            // Рисуем НЕперетаскиваемые фигуры
            for (int i = 0; i < 64; i++)
            {
                var sq = ViewModel.FlatBoard[i];
                if (sq != null && sq.Piece != null && !sq.IsDragging)
                {
                    x = i % 8;
                    y = i / 8;
                    DrawPiece(canvas, sq.Piece, x, y, cellSize, 1.0);
                }
            }

            // Рисуем перетаскиваемую фигуру поверх всего
            var dragging = ViewModel.DraggingSquare;
            if (dragging != null && dragging.Piece != null)
            {
                var pos = ViewModel.DraggingPosition;
                // Центрируем изображение по пальцу
                DrawPiece(canvas, dragging.Piece, pos, cellSize, 1, isAbsolute: true);
            }
        }

        private void DrawPiece(ICanvas canvas, ChessPiece piece, double x, double y, double cellSize, double opacity = 1.0, bool isAbsolute = false)
        {
            // position: либо BoardPosition (x,y), либо Point (DragPosition)
            /*if (isAbsolute && position is Point pt)
            {
                x = pt.X - cellSize / 2;
                y = pt.Y - cellSize / 2;
            }
            else if (position is BoardPosition p)
            {*/
            x = x * cellSize;
            y = y * cellSize;
            /*}
            else
            {
                return;
            }*/
            
            var img = GetPieceImage(piece);
            if (!(img is null))
            {
                canvas.Alpha = (float)opacity;
                canvas.DrawImage(img, (float)x, (float)y, (float)cellSize, (float)cellSize);
                canvas.Alpha = 1;
            }
        }

        private void DrawPiece(ICanvas canvas, ChessPiece piece, Point point, double cellSize, double opacity = 1.0, bool isAbsolute = false)
        {

            double x = point.X - cellSize / 2;
            double y = point.Y - cellSize / 2;

            // Получаем изображение (пример через сервис, можно иначе)
            var img = GetPieceImage(piece);
            if (!(img is null))
            {
                canvas.Alpha = (float)opacity;
                canvas.DrawImage(img, (float)x, (float)y, (float)cellSize, (float)cellSize);
                canvas.Alpha = 1;
            }
        }

        private Microsoft.Maui.Graphics.IImage? GetPieceImage(ChessPiece piece)
        {
            // Например, через ресурс или сервис:
            // return ImageSource.FromFile($"{piece.Color}_{piece.Type}.png");
            // или через свой сервис ViewModel.GetPieceImage(piece)
            return ImageLoader.LoadImage(piece.Image); // или ViewModel.GetPieceImage(piece)
        }
    }
}
