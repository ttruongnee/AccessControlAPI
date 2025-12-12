using AccessControlAPI.DTOs;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories;
using AccessControlAPI.Utils;
using Oracle.ManagedDataAccess.Client;

namespace AccessControlAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly LogHelper _logHelper;
        public AuthService(IAuthRepository authRepository, IUserRepository userRepository, LogHelper logHelper)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _logHelper = logHelper;
        }

        public bool Login(AuthDTO user, out string message)
        {
            try
            {
                var existing = _userRepository.GetByUsername(user.Username);
                if (existing == null)
                {
                    message = $"Người dùng có username = {user.Username} không tồn tại.";
                    return false;
                }

                bool passwordMatch = PasswordHelper.VerifyPassword(user.Password, existing.Password);
                if (passwordMatch)
                {
                    message = "Đăng nhập thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, existing.Id, "Đăng nhập", true, message);
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

        public bool Register(AuthDTO user, out string message)
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
                var result = _authRepository.Register(new User { Username = user.Username, Password = passwordHash }, out int newUserId);
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
    }
}
