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
        private readonly int _accessTokenMinutes;
        private readonly int _refreshTokenDays;
        public JwtTokenHelper(ConfigurationHelper configurationHelper)
        {
            _key = configurationHelper.GetJwtKey();
            _accessTokenMinutes = configurationHelper.GetAccessTokenMinutes();
            _refreshTokenDays = configurationHelper.GetRefreshTokenDays();

        }

        public string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Username", user.Username),
                new Claim("Type", "Access") // Đánh dấu loại token
            };

            return GenerateToken(claims, _accessTokenMinutes); 
        }


        public string GenerateRefreshToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Type", "Refresh") // Đánh dấu loại token
            };

            return GenerateToken(claims, _refreshTokenDays * 24 * 60); 
        }


        private string GenerateToken(Claim[] claims, int expirationMinutes)
        {
            //Encoding.UTF8.GetBytes dùng để converts string thành mảng byte (vì SymmetricSecurityKey cần byte[])
            //key đối xứng (Symmetric) này dùng để ký và xác thực token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            //creds chứa thông tin về khoá ký và thuật toán ký 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),  //thời gian hết hạn
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token); //trả về token dưới dạng chuỗi từ JwtSecurityToken
        }

        // Verify JWT và trả về claims
        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler(); //Helper class để đọc và validate JWT
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters 
                {
                    ValidateIssuerSigningKey = true,  //kiểm tra khoá ký có hợp lệ không 
                    IssuerSigningKey = key, //khóa ký dùng để xác thực token
                    ValidateLifetime = true,  //kiểm tra token hết hạn chưa
                    ClockSkew = TimeSpan.Zero, //không cho phép sai lệch thời gian
                    ValidateIssuer = false, //không kiểm tra nhà phát hành
                    ValidateAudience = false  //không kiểm tra người nhận
                }, out _); //out _ dùng để bỏ qua giá trị trả về thứ hai (SecurityToken)

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}