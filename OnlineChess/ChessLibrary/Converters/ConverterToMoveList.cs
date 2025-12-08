using ChessLibrary.Models.DTO;
using ChessEngine;

namespace ChessLibrary.Converters
{
    public static class ConverterToMoveList
    {
        public static List<ChessMove> ConvertToChessMoveList(MoveList moveList)
        {
            List<ChessMove> result = new List<ChessMove>();
            for (int i = 0; i < moveList.Size; i++)
            {
                ChessMove move = new ChessMove();
                move.FromX = moveList[i].From % 8;
                move.FromY = moveList[i].From / 8;
                move.ToX = moveList[i].To % 8;
                move.ToY = moveList[i].To / 8;
                result.Add(move);
            }
            return result;
        }
    }
}
