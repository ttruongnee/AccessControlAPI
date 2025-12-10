namespace AccessControlAPI.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }

    public class CreateUpdateUserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
