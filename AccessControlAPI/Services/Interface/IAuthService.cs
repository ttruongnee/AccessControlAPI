using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services.Interface
{
    public interface IAuthService
    {
        bool Login(UserDTO user, out string accessToken, out string refreshToken, out string message);
        bool Register(UserDTO user, out string message);
        bool RefreshToken(string refreshToken, out string newAccessToken, out string newRefreshToken, out string message);
    }
}