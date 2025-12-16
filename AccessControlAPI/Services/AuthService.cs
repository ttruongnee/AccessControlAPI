using AccessControlAPI.DTOs;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Oracle.ManagedDataAccess.Client;
using System.Security.Claims;

namespace AccessControlAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenHelper _jwtHelper;
        private readonly LogHelper _logHelper;

        public AuthService(IUserRepository userRepository, JwtTokenHelper jwtHelper, LogHelper logHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _logHelper = logHelper;
        }

        public bool Login(UserDTO user, out string accessToken, out string refreshToken, out string message)
        {
            accessToken = null;
            refreshToken = null;
            try
            {
                //kiểm tra tồn tại người dùng
                var existing = _userRepository.GetByUsername(user.Username);
                if (existing == null)
                {
                    message = $"Người dùng có username = {user.Username} không tồn tại.";
                    return false;
                }

                //kiểm tra mật khẩu 
                bool passwordMatch = PasswordHelper.VerifyPassword(user.Password, existing.Password);
                if (passwordMatch)
                {
                    message = "Đăng nhập thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, existing.Id, "Đăng nhập", true, message);

                    //tạo token
                    accessToken = _jwtHelper.GenerateAccessToken(existing);
                    refreshToken = _jwtHelper.GenerateRefreshToken(existing);

                    return true;
                }
                else
                {
                    message = "Mật khẩu không đúng.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, existing.Id, "Đăng nhập", false, message);
                    return false;
                }

            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Đăng nhập", false, message);
                return false;
            }
        }

        public bool Register(UserDTO user, out string message)
        {
            try
            {
                var existing = _userRepository.GetByUsername(user.Username);
                if (existing != null)
                {
                    message = $"Người dùng có username = {user.Username} đã tồn tại.";
                    return false;
                }

                var passwordHash = PasswordHelper.HashPassword(user.Password);
                var result = _userRepository.Create(new User { Username = user.Username, Password = passwordHash }, out int newUserId);
                if (result)
                {
                    message = $"Đăng ký người dùng thành công với ID: {newUserId}";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, newUserId, "Đăng ký", true, message);
                    return true;
                }
                else
                {
                    message = "Đăng ký người dùng thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, null, "Đăng ký", false, message);
                    return false;
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1:
                        message = "Người dùng đã tồn tại.";
                        break;

                    case 1400:
                        message = "Thiếu dữ liệu yêu cầu (NOT NULL).";
                        break;

                    case 904:
                        message = "Tên cột không hợp lệ.";
                        break;

                    default:
                        message = $"Lỗi CSDL (Oracle {ex.Number}): {ex.Message}.";
                        break;
                }
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Đăng ký", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Đăng ký", false, message);
                return false;
            }
        }

        public bool RefreshToken(string refreshToken, out string newAccessToken, out string newRefreshToken, out string message)
        {
            newAccessToken = null;
            newRefreshToken = null;

            try
            {
                //xác thực refresh token
                var principal = _jwtHelper.ValidateToken(refreshToken);
                if (principal == null)
                {
                    message = "Refresh token không hợp lệ hoặc đã hết hạn";
                    _logHelper.WriteLog(NLog.LogLevel.Warn, null, null, "Refresh Token", false, message);
                    return false;
                }

                //kiểm tra loại token từ claims
                var tokenType = principal.FindFirst("Type")?.Value;
                if (tokenType != "Refresh")
                {
                    message = "Token type không đúng";
                    _logHelper.WriteLog(NLog.LogLevel.Warn, null, null, "Refresh Token", false, message);
                    return false;
                }

                //lấy userId từ claims
                var userIdClaim = principal.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    message = "Token không hợp lệ - Không tìm thấy mã người dùng";
                    _logHelper.WriteLog(NLog.LogLevel.Warn, null, null, "Refresh Token", false, message);
                    return false;
                }

                //tìm user vừa lấy được trong database
                var user = _userRepository.GetById(userId);
                if (user == null)
                {
                    message = "Người dùng không tồn tại";
                    _logHelper.WriteLog(NLog.LogLevel.Error, null, userId, "Refresh Token", false, message);
                    return false;
                }

                //tạo token mới
                newAccessToken = _jwtHelper.GenerateAccessToken(user);
                newRefreshToken = _jwtHelper.GenerateRefreshToken(user);

                message = "Refresh token thành công";
                _logHelper.WriteLog(NLog.LogLevel.Info, null, user.Id, "Refresh Token", true, message);

                return true;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Refresh Token", false, message);
                return false;
            }
        }
    }
}