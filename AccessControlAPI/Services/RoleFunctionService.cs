using AccessControlAPI.DTOs;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Oracle.ManagedDataAccess.Client;

namespace AccessControlAPI.Services
{
    public class RoleFunctionService : IRoleFunctionService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleFunctionRepository _roleFunctionRepository;
        private readonly LogHelper _logHelper;
        public RoleFunctionService(IRoleRepository roleRepository, IRoleFunctionRepository roleFunctionRepository, LogHelper logHelper)
        {
            _roleRepository = roleRepository;
            _roleFunctionRepository = roleFunctionRepository;
            _logHelper = logHelper;
        }
        public bool DeleteFunctionsFromRole(int roleId, out string message)
        {
            var existingRole = _roleRepository.GetById(roleId);
            if (existingRole == null)
            {
                message = $"Vai trò với ID = {roleId} không tồn tại";
                return false;
            }

            var existing = _roleFunctionRepository.GetFunctionsByRoleId(roleId);
            if (existing.Count == 0)
            {
                message = $"Vai trò {roleId} hiện tại không có chức năng nào";
                return false;
            }

            try
            {
                var result = _roleFunctionRepository.DeleteFunctionsFromRole(roleId);
                if (result)
                {
                    message = $"Xoá toàn bộ chức năng của vai trò với ID = {roleId} thành công";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, roleId, "Xoá chức năng vai trò", true, message);
                    return true;
                }
                else
                {
                    message = $"Xoá toàn bộ chức năng của vai trò với ID = {roleId} thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, roleId, "Xoá chức năng vai trò", false, message);
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
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá chức năng vai trò", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá chức năng vai trò", false, message);
                return false;
            }
        }

        public List<FunctionDTO> GetFunctionsByRoleId(int roleId)
        {
            var existingRole = _roleRepository.GetById(roleId);
            if (existingRole == null)
            {
                return null;
            }

            var functions = _roleFunctionRepository.GetFunctionsByRoleId(roleId);
            return functions.Select(f => new FunctionDTO
            {
                Id = f.Id,
                Name = f.Name,
                Sort_order = f.Sort_order,
                Parent_id = f.Parent_id,
                Show_search = f.Show_search,
                Show_add = f.Show_add,
                Show_update = f.Show_update,
                Show_delete = f.Show_delete
            }).ToList();
        }

        public bool UpdateFunctionsForRole(int roleId, List<string> functionIds, out string message)
        {
            var existingRole = _roleRepository.GetById(roleId);
            if (existingRole == null)
            {
                message = $"Vai trò với ID = {roleId} không tồn tại";
                return false;
            }

            try
            {
                var result = _roleFunctionRepository.UpdateFunctionsForRole(roleId, functionIds);
                if (result)
                {
                    message = $"Cập nhật chức năng cho vai trò thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, roleId, "Cập nhật chức năng cho vai trò", true, message);
                    return true;
                }
                else
                {
                    message = "Cập nhật chức năng cho người dùng thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, roleId, "Cập nhật chức năng cho vai trò", false, message);
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
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật chức năng cho vai trò", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật chức năng cho vai trò", false, message);
                return false;
            }
        }
    }
}
