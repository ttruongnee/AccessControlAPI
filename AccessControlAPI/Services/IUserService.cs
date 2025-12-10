using AccessControlAPI.DTOs;
using AccessControlAPI.Models;

namespace AccessControlAPI.Services
{
    public interface IUserService
    {
        List<UserDTO> GetAll();
        UserDTO GetById(int id);
        UserDTO GetByUsername(string username);
        bool Create(CreateUpdateUserDTO user, out string message);
        bool Update(int id, CreateUpdateUserDTO user, out string message);
        bool Delete(int id, out string message);
    }
}
