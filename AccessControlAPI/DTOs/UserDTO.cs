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
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
