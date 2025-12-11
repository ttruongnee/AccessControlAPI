using AccessControlAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;
        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet("{userId}")]
        public IActionResult GetRoleByUserId(int userId)
        {
            var functions = _userRoleService.GetRolesByUserId(userId);
            if (functions == null)
            {
                return NotFound(new { message = $"Người dùng với ID = {userId} không tồn tại!" });
            }

            if (functions.Count == 0)
            {
                return NotFound(new { message = $"Người dùng với ID = {userId} không có vai trò nào!" });
            }
            return Ok(functions);
        }


        [HttpDelete("{userId}")]
        public IActionResult DeleteRolesFromUser(int userId)
        {
            if (_userRoleService.DeleteRolesFromUser(userId, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateRolesForUser(int userId, [FromBody] List<int> roleIds)
        {
            if (roleIds == null)
            {
                return BadRequest(new { message = "Dữ liệu vai trò không hợp lệ" });
            }

            if (_userRoleService.UpdateRolesForUser(userId, roleIds, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }
    }
}
