using AccessControlAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccessControlAPI.Utils
{
    public class JwtTokenHelper
    {
        private readonly string _key;
        public JwtTokenHelper(IConfiguration config)
        {
            _key = config["Jwt:Key"];
        }

        // Tạo Access Token (15 phút)
        public string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("type", "access") // Đánh dấu loại token
            };

            return GenerateToken(claims, 15); // 15 phút
        }

        // Tạo Refresh Token (7 ngày)
        public string GenerateRefreshToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("type", "refresh") // Đánh dấu loại token
            };

            return GenerateToken(claims, 7 * 24 * 60); // 7 ngày = 10080 phút
        }

        // Method dùng chung để tạo JWT
        private string GenerateToken(Claim[] claims, int expirationMinutes)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Verify JWT và trả về claims
        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_key);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,  //kiểm tra token hết hạn chưa
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}