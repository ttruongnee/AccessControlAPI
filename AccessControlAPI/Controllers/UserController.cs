using AccessControlAPI.DTOs;
using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            if (users.Count == 0)
            {
                return NotFound(new { message = "Chưa có người dùng nào!" });
            }
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id) 
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = $"Không tồn tại người dùng với id {id}" });
            }
            return Ok(user);
        }

        [HttpGet("by-username/{username}")]
        public IActionResult GetByUsername(string username)
        {
            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                return NotFound(new { message = $"Không tồn tại người dùng với username {username}" });
            }
            return Ok(user);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] UserDTO user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu người dùng không hợp lệ" });
            }

            if (_userService.Create(user, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] UserDTO user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu người dùng không hợp lệ" });
            }
            if (_userService.Update(id, user, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (_userService.Delete(id, out string message))
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
