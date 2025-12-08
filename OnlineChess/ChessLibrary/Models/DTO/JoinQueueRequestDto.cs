using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Models.DTO
{
    public class JoinQueueRequestDto
    {
        public int GameModeMinutes { get; set; }
    }
}
