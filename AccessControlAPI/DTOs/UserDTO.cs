namespace AccessControlAPI.DTOs
{
    public class UserNoPasswordDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }

    public class UserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
