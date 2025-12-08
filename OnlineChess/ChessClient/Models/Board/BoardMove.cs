using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Models.Board
{
    public class BoardMove
    {
        public BoardPosition From { get; set; }
        public BoardPosition To { get; set; }
    }
}
