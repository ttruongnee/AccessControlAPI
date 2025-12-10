using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services
{
    public interface IUserFunctionService
    {
        List<FunctionDTO> GetFunctionsByUserId(int userId);
        //bool AddFunctionsForUser(int userId, List<string> functionIds, out string message);
        //create or update
        bool UpdateFunctionsForUser(int userId, List<string> functionIds, out string message);
        bool DeleteFunctionsFromUser(int userId, out string message);
    }
}
