using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services
{
    public interface IRoleService
    {
        RoleDTO GetById(int id);
        List<RoleDTO> GetAll();
        bool Create(RoleDTO role, out string message);
        bool Update(RoleDTO role, out string message);
        bool Delete(int id, out string message);
    }
}
