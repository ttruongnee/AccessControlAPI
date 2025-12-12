using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IAuthRepository
    {
        User Login(string username);
        bool Register(User user, out int newUserId);
    }
}
