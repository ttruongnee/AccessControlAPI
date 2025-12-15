namespace AccessControlAPI.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires_At { get; set; }
        public bool Is_Revoked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
