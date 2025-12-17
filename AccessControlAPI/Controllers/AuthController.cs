using AccessControlAPI.DTOs;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly int _refreshTokenDays;

        public AuthController(IAuthService authService, ConfigurationHelper configurationHelper)
        {
            _authService = authService;
            _refreshTokenDays = configurationHelper.GetRefreshTokenDays();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDTO user)
        {
            var success = _authService.Login(user, out string accessToken, out string refreshToken, out string message);

            if (!success)
            {
                //ném lỗi 401
                return Unauthorized(new { message });
            }

            //set refresh token vào httpOnly cookie
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,  //js không thể đọc cookie này
                Secure = true, //chỉ gửi qua kết nối HTTPS để chống MITM (man-in-the-middle) tấn công
                SameSite = SameSiteMode.Strict, //Cookie CHỈ gửi khi request từ CÙNG DOMAIN, chống CSRF (Cross-Site Request Forgery)
                Expires = DateTime.UtcNow.AddDays(_refreshTokenDays)
            });

            return Ok(new
            {
                message,
                username = user.Username,
                accessToken
            });
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO user)
        {
            var success = _authService.Register(user, out string message);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }


        [HttpPost("refresh")]
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
    }
}