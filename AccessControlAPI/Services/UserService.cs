using AccessControlAPI.DTOs;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace AccessControlAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly LogHelper _logHelper;


        public UserService(IUserRepository userRepository, LogHelper logHelper, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _logHelper = logHelper;
        }

        public bool Create(UserDTO user, out string message)
        {
            var existing = _userRepository.GetByUsername(user.Username);
            if (existing != null)
            {
                message = $"Người dùng có username = {user.Username} đã tồn tại.";
                return false;
            }
            try
            {
                var passwordHash = PasswordHelper.HashPassword(user.Password);
                var result = _userRepository.Create(new User { Username = user.Username, Password = passwordHash }, out int newUserId);
                if (result)
                {
                    message = $"Tạo người dùng thành công với ID: {newUserId}";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, newUserId, "Tạo người dùng", true, message);
                    return true;
                }
                else
                {
                    message = "Tạo người dùng thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, null, "Tạo người dùng", false, message);
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
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Tạo người dùng", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Tạo người dùng", false, message);
                return false;
            }
        }

        public bool Delete(int id, out string message)
        {
            var existing = _userRepository.GetById(id);
            if (existing == null)
            {
                message = $"Người dùng với ID = {id} không tồn tại.";
                return false;
            }

            try
            {
                var result = _userRepository.Delete(id);
                if (result)
                {
                    message = $"Xoá người dùng thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, id, "Xoá người dùng", true, message);
                    return true;
                }
                else
                {
                    message = "Xoá người dùng thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, id, "Xoá người dùng", false, message);
                    return false;
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 2292:
                        message = "Không thể xóa: lỗi liên quan ràng buộc khoá ngoại.";
                        break;

                    default:
                        message = $"Lỗi CSDL (Oracle {ex.Number}): {ex.Message}.";
                        break;
                }
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá người dùng", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá người dùng", false, message);
                return false;
            }
        }

        public List<UserNoPasswordDTO> GetAll()
        {
            var users = _userRepository.GetAll();
            return users.Select(u => new UserNoPasswordDTO
            {
                Id = u.Id,
                Username = u.Username,
                Roles = _userRoleRepository.GetRolesByUserId(u.Id).Select(r => new RoleNoListFunctionsDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                }).ToList()
            }).ToList();    
        }

        public UserNoPasswordDTO GetById(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
            {
                return null;
            }
            return new UserNoPasswordDTO
            {
                Id = user.Id,
                Username = user.Username,
                Roles = _userRoleRepository.GetRolesByUserId(user.Id).Select(r => new RoleNoListFunctionsDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                }).ToList()
            };
        }

        public UserNoPasswordDTO GetByUsername(string username)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null)
            {
                return null;
            }
            return new UserNoPasswordDTO
            {
                Id = user.Id,
                Username = user.Username,
                Roles = _userRoleRepository.GetRolesByUserId(user.Id).Select(r => new RoleNoListFunctionsDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                }).ToList()
            };
        }

        public bool Update(int id, UserDTO user, out string message)
        {
            var existing = _userRepository.GetById(id);
            if (existing == null)
            {
                message = $"Người dùng với ID = {id} không tồn tại.";
                return false;
            }

            try
            {
                var passwordHash = PasswordHelper.HashPassword(user.Password);
                var result = _userRepository.Update(id, new User { Username = user.Username, Password = passwordHash});
                if (result)
                {
                    message = $"Cập nhật thông tin người dùng thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, id, "Cập nhật thông tin người dùng", true, message);
                    return true;
                }
                else
                {
                    message = "Cập nhật thông tin vai trò thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, id, "Cập nhật thông tin người dùng", false, message);
                    return false;
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1: 
                        message = "Username đã tồn tại, vui lòng chọn username khác.";
                        break;

                    case 1407:
                        message = "Thiếu dữ liệu yêu cầu (NOT NULL).";
                        break;

                    case 904:
                        message = "Tên cột không hợp lệ.";
                        break;

                    default:
                        message = $"Lỗi CSDL (Oracle {ex.Number}): {ex.Message}.";
                        break;
                }
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật thông tin người dùng", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật thông tin người dùng", false, message);
                return false;
            }
        }
    }
}
