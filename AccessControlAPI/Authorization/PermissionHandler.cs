using AccessControlAPI.DTOs;
using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;

namespace AccessControlAPI.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserFunctionService _userFunctionService; //dùng để lấy quyền của user

        public PermissionHandler(IUserFunctionService userFunctionService)
        {
            _userFunctionService = userFunctionService;
        }

        protected override Task HandleRequirementAsync(  //xử lý yêu cầu ủy quyền (chạy khi có [Authorize] với policy tương ứng)
            AuthorizationHandlerContext context,  
            PermissionRequirement requirement)  //đối tượng yêu cầu chứa FunctionId
        {
            //nếu chưa đăng nhập (context.User lấy ra ClaimsPrincipal từ token)
            if (context.User.Identity == null || context.User.Identity.IsAuthenticated == false)
            {
                return Task.CompletedTask;
            }

            //lấy userId từ claim trong token
            var userIdClaim = context.User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Task.CompletedTask;
            }

            //lấy toàn bộ quyền của user 
            var userFunctions = _userFunctionService.GetAllFunctionsByUserId(userId);

            //kiểm tra quyền
            if (userFunctions != null &&
                HasFunction(userFunctions, requirement.FunctionId))
            {
                context.Succeed(requirement); //requirement được chấp nhận
            }

            return Task.CompletedTask;
        }

        //đệ quy kiểm tra xem trong danh sách functions có functionId không
        private bool HasFunction(List<FunctionDTO> functions, string functionId)
        {
            foreach (var func in functions)
            {
                if (func.Id == functionId)
                    return true;

                if (func.Children != null && HasFunction(func.Children, functionId))
                    return true;
            }
            return false;
        }
    }
}
