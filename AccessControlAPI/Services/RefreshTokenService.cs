using AccessControlAPI.Repositories;
using AccessControlAPI.Utils;

namespace AccessControlAPI.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenHelper _jwtTokenHelper;
        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository,
                                   IUserRepository userRepository,
                                   JwtTokenHelper jwtTokenHelper)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _jwtTokenHelper = jwtTokenHelper;
        }
        public bool RefreshAccessToken(string refreshToken, out string newAccessToken, out string newRefreshToken, out string message)
        {
            newAccessToken = null;
            newRefreshToken = null;

            var token = _refreshTokenRepository.GetByToken(refreshToken);

            if (token == null || token.Is_Revoked == true || token.Expires_At < DateTime.UtcNow)
            {
                message = "Refresh token không hợp lệ";
                return false;
            }

            var user = _userRepository.GetById(token.User_Id);
            if (user == null)
            {
                message = "Người dùng không tồn tại";
                return false;
            }

            // 1️ revoke token cũ
            _refreshTokenRepository.Revoke(refreshToken);

            // 2️ tạo token mới
            newAccessToken = _jwtTokenHelper.GenerateAccessToken(user);
            newRefreshToken = _jwtTokenHelper.GenerateRefreshToken();

            var expires = DateTime.UtcNow.AddDays(7);
            _refreshTokenRepository.Save(user.Id, newRefreshToken, expires);

            message = "Refresh token thành công";
            return true;
        }

        public void Logout(string refreshToken)
        {
            _refreshTokenRepository.Revoke(refreshToken);
        }
    }
}
