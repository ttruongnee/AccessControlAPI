using AccessControlAPI.DTOs;
using AccessControlAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthDTO user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu người dùng không hợp lệ" });
            }

            if (_authService.Login(user, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthDTO user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu người dùng không hợp lệ" });
            }

            if (_authService.Register(user, out string message))
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
