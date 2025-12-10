namespace AccessControlAPI.DTOs
{
    public class CreateRoleFunctionDTO
    {
        public int RoleId { get; set; }
        public List<int> FunctionIds { get; set; }
    }
    public class RoleWithFunctionsDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<FunctionDTO> Functions { get; set; }
    }
}
