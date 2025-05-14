using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Models.DTO
{
    public class MatchFoundDto
    {
        public string GameId { get; set; }
        public string Position { get; set; }    
        public string YourColor { get; set; }   
        public string OpponentUsername { get; set; }
        public int OpponentRating { get; set; } 
        public List<ChessMove> AvailableMoves { get; set; }
    }
}
