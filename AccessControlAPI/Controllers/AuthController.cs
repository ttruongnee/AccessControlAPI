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
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        public AuthController(IUserService userService, IRefreshTokenService refreshTokenService)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDTO user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu người dùng không hợp lệ" });
            }

            if (_userService.Login(user, out string accessToken, out string refreshToken, out string message))
            {
                return Ok(new { message, accessToken, refreshToken });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu người dùng không hợp lệ" });
            }

            if (_userService.Register(user, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] RefreshTokenDTO request)
        {
            bool success = _refreshTokenService.RefreshAccessToken(
                request.RefreshToken,
                out string newAccessToken,
                out string newRefreshToken,
                out string message
            );

            if (!success)
            {
                return Unauthorized(new { message });
            }

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }
    }
}

