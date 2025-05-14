using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Models.DTO
{
    public class GameResponseDto
    {
        public string Id { get; set; }
        public int PlayerWhite { get; set; }
        public int PlayerBlack { get; set; }
        public string Status { get; set; }
        public string Position { get; set; }
        public int GameModeMinutes { get; set; }
    }
}
