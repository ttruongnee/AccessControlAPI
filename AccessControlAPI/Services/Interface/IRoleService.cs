using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services.Interface
{
    public interface IRoleService
    {
        RoleDTO GetById(int id);
        List<RoleDTO> GetAll();
        bool Create(CreateUpdateRoleDTO role, out string message);
        bool Update(int id, CreateUpdateRoleDTO role, out string message);
        bool Delete(int id, out string message);
    }
}
