using LuckyBeardAPI_Launchpad2026.Models;

namespace LuckyBeardAPI_Launchpad2026.DTOs
{
    public class AuthDto
    {
       
        public sealed record RegisterDto(string Name, string Surname, string Email, string Password);
        public sealed record LoginDto(string Email, string Password);
        public sealed record TokenResponseDto(string AccessToken);
    }
}
