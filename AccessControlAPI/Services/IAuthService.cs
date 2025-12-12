using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services
{
    public interface IAuthService
    {
        bool Login(AuthDTO user, out string message);
        bool Register(AuthDTO user, out string message);
    }
}
