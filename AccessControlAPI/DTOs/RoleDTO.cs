using System.ComponentModel.DataAnnotations;

namespace AccessControlAPI.DTOs
{
    public class RoleDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<FunctionNoChildrenDTO> Functions { get; set; } = new List<FunctionNoChildrenDTO>();
    }
    public class CreateUpdateRoleDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
