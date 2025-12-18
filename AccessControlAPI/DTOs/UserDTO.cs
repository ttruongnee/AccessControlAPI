using System.ComponentModel.DataAnnotations;

namespace AccessControlAPI.DTOs
{
    public class UserNoPasswordDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        public List<RoleNoListFunctionsDTO> Roles { get; set; } = new List<RoleNoListFunctionsDTO>();
    }

    public class UserDTO
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc")]
        public string Password { get; set; }
    }
}
