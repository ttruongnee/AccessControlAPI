using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services
{
    public interface IUserRoleService
    {
        List<RoleDTO> GetRolesByUserId(int userId);
        bool UpdateRolesForUser(int userId, List<int> roleIds, out string message);
        bool DeleteRolesFromUser(int userId, out string message);
    }
}
