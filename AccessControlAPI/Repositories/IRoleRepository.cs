using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IRoleRepository
    {
        Role GetById(int id);
        List<Role> GetAll();
        int GetNextRoleId();
        bool Create(Role role, out int newRoleId);
        bool Update(Role role);
        bool Delete(int id);
    }
}
