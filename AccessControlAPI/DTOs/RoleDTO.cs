using System.ComponentModel.DataAnnotations;

namespace AccessControlAPI.DTOs
{
    public class RoleDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<FunctionDTO> Functions { get; set; } = new List<FunctionDTO>();
    }
    public class RoleNoListFunctionsDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class CreateUpdateRoleDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
