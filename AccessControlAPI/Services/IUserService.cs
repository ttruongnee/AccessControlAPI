using AccessControlAPI.DTOs;
using AccessControlAPI.Models;

namespace AccessControlAPI.Services
{
    public interface IUserService
    {
        List<UserNoPasswordDTO> GetAll();
        UserNoPasswordDTO GetById(int id);
        UserNoPasswordDTO GetByUsername(string username);
        bool Create(UserDTO user, out string message);
        bool Update(int id, UserDTO user, out string message);
        bool Delete(int id, out string message);
        bool Login(UserDTO user, out string accessToken, out string refreshToken, out string message);
        bool Register(UserDTO user, out string message);

    }
}
