using System.ComponentModel.DataAnnotations;

namespace AccessControlAPI.DTOs
{
    public class UserNoPasswordDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
        public List<FunctionNoChildrenDTO> Functions { get; set; } = new List<FunctionNoChildrenDTO>(); 
    }

    public class UserDTO
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc")]
        public string Password { get; set; }
    }
}
