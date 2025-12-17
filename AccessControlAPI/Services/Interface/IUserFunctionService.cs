using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services.Interface
{
    public interface IUserFunctionService
    {
        List<FunctionDTO> GetAllFunctionsByUserId(int userId);
        List<FunctionDTO> GetFunctionsByUserId(int userId);
        bool UpdateFunctionsForUser(int userId, List<string> functionIds, out string message);
        bool DeleteFunctionsFromUser(int userId, out string message);
    }
}
