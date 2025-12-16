using AccessControlAPI.DTOs;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Oracle.ManagedDataAccess.Client;

namespace AccessControlAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleFunctionRepository _roleFunctionRepository;
        private readonly LogHelper _logHelper;
        public RoleService(IRoleRepository roleRepository, LogHelper logHelper, IRoleFunctionRepository roleFunctionRepository)
        {
            _roleRepository = roleRepository;
            _logHelper = logHelper;
            _roleFunctionRepository = roleFunctionRepository;
        }

        public bool Create(CreateUpdateRoleDTO role, out string message)
        {
            try
            {
                var result = _roleRepository.Create( new Role { Name = role.Name }, out int newRoleId);
                if (result)
                {
                    message = $"Tạo vai trò thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, newRoleId, "Tạo vai trò", true, message);
                    return true;
                }
                else
                {
                    message = "Tạo vai trò thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, null, "Tạo vai trò", false, message);
                    return false;
                }
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 1:
                        message = "Vai trò đã tồn tại.";
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
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Tạo vai trò", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Tạo vai trò", false, message);
                return false;
            }
        }


        public bool Delete(int id, out string message)
        {
            var existing = _roleRepository.GetById(id);
            if (existing == null)
            {
                message = $"Vai trò với ID = {id} không tồn tại.";
                return false;
            }

            try
            {
                var result = _roleRepository.Delete(id);
                if (result)
                {
                    message = $"Xoá vai trò thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, id, "Xoá vai trò", true, message);
                    return true;
                }
                else
                {
                    message = "Xoá vai trò thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, id, "Xoá vai trò", false, message);
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
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá vai trò", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá vai trò", false, message);
                return false;
            }
        }

        public List<RoleDTO> GetAll()
        {
            var roles = _roleRepository.GetAll();
            return roles.Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name,
                Functions = _roleFunctionRepository.GetFunctionsByRoleId(r.Id).Select(f => new FunctionDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Sort_order = f.Sort_order,
                    Parent_id = f.Parent_id,
                    Show_search = f.Show_search,
                    Show_add = f.Show_add,
                    Show_update = f.Show_update,
                    Show_delete = f.Show_delete
                }).ToList()
            }).ToList();
        }

        public RoleDTO GetById(int id)
        {
            var role = _roleRepository.GetById(id);
            if (role == null)
            {
                return null;
            }
            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Functions = _roleFunctionRepository.GetFunctionsByRoleId(role.Id).Select(f => new FunctionDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Sort_order = f.Sort_order,
                    Parent_id = f.Parent_id,
                    Show_search = f.Show_search,
                    Show_add = f.Show_add,
                    Show_update = f.Show_update,
                    Show_delete = f.Show_delete
                }).ToList()
            };
        }

        public bool Update(int id, CreateUpdateRoleDTO role, out string message)
        {
            var existing = _roleRepository.GetById(id);
            if (existing == null)
            {
                message = $"Vai trò với ID = {id} không tồn tại.";
                return false;
            }

            try
            {
                var result = _roleRepository.Update(id, new Role { Name = role.Name });
                if (result)
                {
                    message = $"Cập nhật thông tin vai trò thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, id, "Cập nhật thông tin vai trò", true, message);
                    return true;
                }
                else
                {
                    message = "Cập nhật thông tin vai trò thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, null, id, "Cập nhật thông tin vai trò", false, message);
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

                    default:
                        message = $"Lỗi CSDL (Oracle {ex.Number}): {ex.Message}.";
                        break;
                }
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật thông tin vai trò", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật thông tin vai trò", false, message);
                return false;
            }
        }
    }
}
