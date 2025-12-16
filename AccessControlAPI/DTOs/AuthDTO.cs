using System.ComponentModel.DataAnnotations;

namespace AccessControlAPI.DTOs
{
    public class TokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password là bắt buộc")]

        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password là bắt buộc")]
        public string Password { get; set; }
    }
}