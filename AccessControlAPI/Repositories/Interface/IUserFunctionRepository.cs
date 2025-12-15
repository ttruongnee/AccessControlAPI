using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories.Interface
{
    public interface IUserFunctionRepository
    {
        List<Function> GetFunctionsByUserId(int userId);
        //bool AddFunctionsForUser(int userId, List<string> functionIds);
        //create or update
        bool UpdateFunctionsForUser(int userId, List<string> functionIds);
        bool DeleteFunctionsFromUser(int userId);
    }
}
