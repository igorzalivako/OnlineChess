using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Models
{
    public record UserDto(string Id, string Username, int Rating, string Token);

    public record UserLoginDto(string Username, string Password);

    public record UserRegisterDto(string Username, string Password);
}