using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories.Interface
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User GetById(int id);
        User GetByUsername(string username);
        int GetNextUserId();
        bool Create(User user, out int newUserId);
        bool Update(int id, User user);
        bool Delete(int id);
    }
}
