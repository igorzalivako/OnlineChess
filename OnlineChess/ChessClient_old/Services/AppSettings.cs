using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Services
{
    public static class AppSettings
    {
        public static string ApiBaseUrl { get; set; }
        public static string SignalRHubUrl { get; set; }
    }
}
