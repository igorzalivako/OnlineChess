using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
