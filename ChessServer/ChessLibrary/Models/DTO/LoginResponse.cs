using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Models.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
    }
}