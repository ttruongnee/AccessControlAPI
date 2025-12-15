using AccessControlAPI.DTOs;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Oracle.ManagedDataAccess.Client;

namespace AccessControlAPI.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly LogHelper _logHelper;
        public UserRoleService(IUserRoleRepository userRoleRepository, IUserRepository userRepository, LogHelper logHelper)
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
            _logHelper = logHelper;
        }

        public bool DeleteRolesFromUser(int userId, out string message)
        {
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                message = $"Người dùng với ID = {userId} không tồn tại";
                return false;
            }

            var existing = _userRoleRepository.GetRolesByUserId(userId);
            if (existing.Count == 0)
            {
                message = $"Người dùng với ID = {userId} hiện tại không có vai trò nào";
                return false;
            }

            try
            {
                var result = _userRoleRepository.DeleteRolesFromUser(userId);
                if (result)
                {
                    message = $"Xoá toàn bộ vai trò của tài khoản {userId} thành công";
                    _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Xoá vai trò người dùng", true, message);
                    return true;
                }
                else
                {
                    message = $"Xoá toàn bộ vai trò của tài khoản {userId} thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Xoá vai trò người dùng", false, message);
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
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá vai trò người dùng", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá vai trò người dùng", false, message);
                return false;
            }
        }

        public List<RoleDTO> GetRolesByUserId(int userId)
        {
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                return null;
            }

            var functions = _userRoleRepository.GetRolesByUserId(userId);
            return functions.Select(f => new RoleDTO
            {
                Id = f.Id,
                Name = f.Name
            }).ToList();
        }

        public bool UpdateRolesForUser(int userId, List<int> roleIds, out string message)
        {
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                message = $"Người dùng với ID = {userId} không tồn tại.";
                return false;
            }

            try
            {
                var result = _userRoleRepository.UpdateRolesForUser(userId, roleIds);
                if (result)
                {
                    message = $"Cập nhật vai trò cho người dùng thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Cập nhật vai trò cho người dùng", true, message);
                    return true;
                }
                else
                {
                    message = "Cập nhật vai trò cho người dùng thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Cập nhật vai trò cho người dùng", false, message);
                    return false;
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1407:
                        message = "Thiếu dữ liệu yêu cầu (NOT NULL).";
                        break;

                    case 904:
                        message = "Tên cột không hợp lệ.";
                        break;

                    case 2291:
                        message = "Không tìm thấy dữ liệu tham chiếu từ bảng cha.";
                        break;

                    default:
                        message = $"Lỗi CSDL (Oracle {ex.Number}): {ex.Message}.";
                        break;
                }
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật vai trò cho người dùng", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật vai trò cho người dùng", false, message);
                return false;
            }
        }
    }
}
