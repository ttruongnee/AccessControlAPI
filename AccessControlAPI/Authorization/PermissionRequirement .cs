using Microsoft.AspNetCore.Authorization;

namespace AccessControlAPI.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string FunctionId { get; }

        public PermissionRequirement(string functionId)
        {
            FunctionId = functionId;
        }
    }
}
