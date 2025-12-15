using AccessControlAPI.DTOs;
using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public IActionResult Login([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = _authService.Login(user, out string accessToken, out string refreshToken, out string message);

            if (!success)
            {
                return Unauthorized(new { message });
            }

            // Set refresh token vào httpOnly cookie
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                message,
                accessToken,
                username = user.Username
            });
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = _authService.Register(user, out string message);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public IActionResult RefreshToken()
        {
            // Lấy refresh token từ cookie
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token không tồn tại" });
            }

            var success = _authService.RefreshToken(
                refreshToken,
                out string newAccessToken,
                out string newRefreshToken,
                out string message
            );

            if (!success)
            {
                // Xóa cookie nếu token không hợp lệ
                Response.Cookies.Delete("refreshToken");
                return Unauthorized(new { message });
            }

            // Set refresh token mới vào cookie
            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                message,
                accessToken = newAccessToken
            });
        }


        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Đăng xuất thành công" });
        }


        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            var username = User.Identity?.Name;
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return Ok(new
            {
                message = $"Xin chào {username}!",
                userId = userId
            });
        }
    }
}