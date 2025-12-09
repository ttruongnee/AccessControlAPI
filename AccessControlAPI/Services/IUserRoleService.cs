using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services
{
    public interface IUserRoleService
    {
        List<RoleDTO> GetRolesByUserId(int userId);
        bool AddRolesForUser(int userId, List<int> roleIds, out string message);
        bool DeleteRoleFromUser(int userId, out string message);
    }
}
