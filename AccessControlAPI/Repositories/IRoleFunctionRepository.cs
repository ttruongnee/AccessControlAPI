using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IRoleFunctionRepository
    {
        List<Function> GetFunctionIdsByRoleId(int roleId);
        bool AddFunctionsForRole(int roleId, List<string> functionIds);
        bool DeleteFunctionsFromRole(int roleId);
    }
}
