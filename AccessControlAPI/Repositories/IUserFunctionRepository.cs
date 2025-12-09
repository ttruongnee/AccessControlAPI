using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IUserFunctionRepository
    {
        List<Function> GetFunctionsByUserId(int userId);
        bool AddFunctionsForUser(int userId, List<string> functionIds);
        bool DeleteFunctionsFromUser(int userId);
    }
}
