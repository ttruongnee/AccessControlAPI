using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleFunctionController : ControllerBase
    {
        private readonly IRoleFunctionService _roleFunctionService;
        public RoleFunctionController(IRoleFunctionService roleFunctionService)
        {
            _roleFunctionService = roleFunctionService;
        }

        [HttpGet("{roleId}")]
        public IActionResult GetFunctionByRoleId(int roleId)
        {
            var functions = _roleFunctionService.GetFunctionsByRoleId(roleId);
            if (functions == null)
            {
                return NotFound(new { message = $"Vai trò với ID = {roleId} không tồn tại!" });
            }

            if (functions.Count == 0)
            {
                return NotFound(new { message = $"Vai trò với ID = {roleId} không có chức năng nào!" });
            }
            return Ok(functions);
        }

        [HttpDelete("{roleId}")]
        [Authorize]
        public IActionResult DeleteFunctionsFromRole(int roleId)
        {
            if (_roleFunctionService.DeleteFunctionsFromRole(roleId, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpPut("{roleId}")]
        [Authorize]
        public IActionResult UpdateFunctionsForRole(int roleId, [FromBody] List<string> functionIds)
        {
            if (functionIds == null)
            {
                return BadRequest(new { message = "Dữ liệu chức năng không hợp lệ" });
            }

            if (_roleFunctionService.UpdateFunctionsForRole(roleId, functionIds, out string message))
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
