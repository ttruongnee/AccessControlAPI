using System.ComponentModel.DataAnnotations;

namespace AccessControlAPI.DTOs
{
    public class UserNoPasswordDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
    }

    public class UserDTO
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        //[StringLength(50, MinimumLength = 3, ErrorMessage = "Username phải từ 3-50 ký tự")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc")]
        public string Password { get; set; }
    }
}
