using AccessControlAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AccessControlAPI.Utils
{
    public class JwtTokenHelper
    {
        private readonly ConfigurationHelper _config;

        public JwtTokenHelper(ConfigurationHelper config)
        {
            _config = config;
        }

        public string GenerateAccessToken(User user)
        {
            //payload của jwt
            var claims = new[]
            {
                new Claim("Username", user.Username),
            };

            //tạo khoá đối xứng (vừa dùng để ký và xác thực) từ jwt key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetJwtKey())
            );

            //SigningCredentials(dùng khoá nào, thuật toán nào để ký)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //tạo thông tin ký token
            var token = new JwtSecurityToken(
                claims: claims,
                //thời điểm token hết hạn
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_config.GetAccessTokenMinutes())
                ),
                //thông tin ký token
                signingCredentials: creds
            );

            //trả về token đã được ký dưới dạng chuỗi
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
