using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IUserRoleRepository
    {
        List<Role> GetRolesByUserId(int userId);
        bool UpdateRolesForUser(int userId, List<int> roleIds);
        bool DeleteRolesFromUser(int userId);
    }
}
