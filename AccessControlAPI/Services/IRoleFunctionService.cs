using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services
{
    public interface IRoleFunctionService
    {
        List<FunctionDTO> GetFunctionsByRoleId(int roleId);
        bool UpdateFunctionsForRole(int roleId, List<string> functionIds, out string message);
        bool DeleteFunctionsFromRole(int roleId, out string message);
    }
}
