using AccessControlAPI.DTOs;
using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFunctionController : ControllerBase
    {
        private readonly IUserFunctionService _userFunctionService;
        public UserFunctionController(IUserFunctionService userFunctionService)
        {
            _userFunctionService = userFunctionService;
        }

        [HttpGet("AllFunctions/{userId}")]
        public IActionResult GetAllFunctionsByUserId(int userId)
        {
            var functions = _userFunctionService.GetAllFunctionsByUserId(userId);
            if (functions == null)
            {
                return NotFound(new { message = $"Người dùng {userId} không tồn tại!" });
            }

            if (functions.Count == 0)
            {
                return NotFound(new { message = $"Người dùng {userId} không có chức năng nào!" });
            }
            return Ok(functions);
        }

        [HttpGet("UserFunctions/{userId}")]
        public IActionResult GetFunctionsByUserId(int userId)
        {
            var functions = _userFunctionService.GetFunctionsByUserId(userId);
            if (functions == null)
            {
                return NotFound(new { message = $"Người dùng {userId} không tồn tại!" });
            }

            if (functions.Count == 0)
            {
                return NotFound(new { message = $"Người dùng {userId} không có chức năng nào!" });    
            }
            return Ok(functions);
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public IActionResult DeleteFunctionsFromUser(int userId)
        {
            if (_userFunctionService.DeleteFunctionsFromUser(userId, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpPut("{userId}")]
        //[Authorize]
        public IActionResult UpdateFunctionsForUser(int userId, [FromBody] List<string> functionIds)
        {
            if (functionIds == null)
            {
                return BadRequest(new { message = "Dữ liệu chức năng không hợp lệ" });
            }

            if (_userFunctionService.UpdateFunctionsForUser(userId, functionIds, out string message))
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
