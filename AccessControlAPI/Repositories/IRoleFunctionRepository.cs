using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IRoleFunctionRepository
    {
        List<Function> GetFunctionsByRoleId(int roleId);
        //bool AddFunctionsForRole(int roleId, List<string> functionIds);
        //create or update 
        bool UpdateFunctionsForRole(int roleId, List<string> functionIds);
        bool DeleteFunctionsFromRole(int roleId);
    }
}
