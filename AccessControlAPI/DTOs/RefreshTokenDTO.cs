using System.ComponentModel.DataAnnotations;

namespace AccessControlAPI.DTOs
{
    public class RefreshTokenDTO
    {
        [Required(ErrorMessage = "RefreshToken là bắt buộc")]
        public string RefreshToken { get; set; }
    }
}
