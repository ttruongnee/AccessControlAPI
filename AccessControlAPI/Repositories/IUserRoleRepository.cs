using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IUserRoleRepository
    {
        List<Role> GetRolesByUserId(int userId);
        bool AddRolesForUser(int userId, List<int> roleIds);
        bool DeleteRoleFromUser(int userId);
    }
}
