using AccessControlAPI.Models;
using System.Runtime.CompilerServices;

namespace AccessControlAPI.Services
{
    public interface IUserService
    {
        List<User> GetAll();
        User GetById(int id);
        User GetByUsername(string username);
        bool Create(User user, out string message);
        bool Update(User user, out string message);
        bool Delete(int id, out string message);
    }
}
