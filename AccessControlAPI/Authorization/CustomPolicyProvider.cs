using Microsoft.AspNetCore.Authorization; 

namespace AccessControlAPI.Authorization
{
    //class này tạo và cung cấp Policy
    public class CustomPolicyProvider : IAuthorizationPolicyProvider  //yêu cầu async ở interface này nên phải return task
    {
        //dùng để trả về Policy mặc định khi không chỉ định Policy nào -> khi [Authorize] (không truyền tên Policy)
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());
        }


        //dùng để trả về Policy fallback cho endpoint KHÔNG có (không có [Authorize])
        //trả null -> endpoint không có [Authorize] sẽ là public 
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy?>(null);
        }


        //dùng để trả về Policy theo tên (bất kỳ policyName nào)
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Tự động tạo Policy cho mọi tên
            return Task.FromResult<AuthorizationPolicy?>(
                new AuthorizationPolicyBuilder()  //Tạo bộ xây dựng policy
                    .RequireAuthenticatedUser() //kiểm tra người dùng đã xác thực (token hợp lệ)
                    .AddRequirements(  //gửi PremissionRequirement đến PermissionHandler để xử lý
                        new PermissionRequirement(policyName)   //AddRequirements chỉ nhận một IAuthorizationRequirement nên phải tạo PermissionRequirement
                    )
                    .Build()  
            );
        }
    }
}