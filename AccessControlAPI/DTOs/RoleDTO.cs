namespace AccessControlAPI.DTOs
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CreateUpdateRoleDTO
    {
        public string Name { get; set; }
    }
}
