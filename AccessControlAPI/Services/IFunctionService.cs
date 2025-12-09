using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services
{
    public interface IFunctionService
    {
        List<FunctionDTO> GetAll();
        FunctionDTO GetById(string id);
    }
}
