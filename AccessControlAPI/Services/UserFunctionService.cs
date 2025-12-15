using AccessControlAPI.Database;
using AccessControlAPI.DTOs;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services.Interface;
using AccessControlAPI.Utils;
using Oracle.ManagedDataAccess.Client;

namespace AccessControlAPI.Services
{
    public class UserFunctionService : IUserFunctionService
    {
        private readonly LogHelper _logHelper;
        private readonly IUserRepository _userRepository;
        private readonly IUserFunctionRepository _userFunctionRepository;
        public UserFunctionService(LogHelper logHelper, IUserFunctionRepository userFunctionRepository, IUserRepository userRepository)
        {
            _logHelper = logHelper;
            _userFunctionRepository = userFunctionRepository;
            _userRepository = userRepository;
        }

        //Tạo chức năng cho người dùng mới - không sử dụng nữa, thay bằng việc gán chức năng trong UpdateFunctionsForUser
        //public bool AddFunctionsForUser(int userId, List<string> functionIds, out string message)
        //{
        //    try
        //    {
        //        var existingUser = _userRepository.GetById(userId);
        //        if (existingUser == null)
        //        {
        //            message = $"Người dùng với ID = {userId} không tồn tại.";
        //            return false;
        //        }
        //        var result = _userFunctionRepository.AddFunctionsForUser(userId, functionIds);
        //        if (result)
        //        {
        //            message = $"Gán chức năng cho người dùng {userId} thành công";
        //            _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Gán chức năng cho người dùng", true, message);
        //            return true;
        //        }
        //        else
        //        {
        //            message = $"Gán chức năng cho người dùng {userId} thất bại";
        //            _logHelper.WriteLog(NLog.LogLevel.Info, null, null, "Gán chức năng cho người dùng", false, message);
        //            return false;
        //        }
        //    }
        //    catch (OracleException ex)
        //    {
        //        switch (ex.Number)
        //        {
        //            case 1:
        //                message = "Chức năng thêm vào người dùng đã tồn tại.";
        //                break;

        //            case 1400:
        //                message = "Thiếu dữ liệu yêu cầu (NOT NULL).";
        //                break;

        //            case 904:
        //                message = "Tên cột không hợp lệ.";
        //                break;

        //            case 2291:
        //                message = "Không tìm thấy dữ liệu tham chiếu từ bảng cha.";
        //                break;

        //            default:
        //                message = $"Lỗi CSDL (Oracle {ex.Number}): {ex.Message}.";
        //                break;
        //        }
        //        _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Gán chức năng cho người dùng", false, message);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        message = $"Lỗi hệ thống: {ex.Message}";
        //        _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Gán chức năng cho người dùng", false, message);
        //        return false;
        //    }
        //}

        public bool DeleteFunctionsFromUser(int userId, out string message)
        {
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                message = $"Người dùng với ID = {userId} không tồn tại";
                return false;
            }

            var existing = _userFunctionRepository.GetFunctionsByUserId(userId);
            if (existing.Count == 0)
            {
                message = $"Người dùng {userId} hiện tại không có chức năng nào";
                return false;
            }

            try
            {
                var result = _userFunctionRepository.DeleteFunctionsFromUser(userId);
                if (result)
                {
                    message = $"Xoá toàn bộ chức năng của tài khoản {userId} thành công";
                    _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Xoá chức năng người dùng", true, message);
                    return true;
                }
                else
                {
                    message = $"Xoá toàn bộ chức năng của tài khoản {userId} thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Xoá chức năng người dùng", false, message);
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
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá chức năng người dùng", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Xoá chức năng người dùng", false, message);
                return false;
            }
        }

        public List<FunctionDTO> GetFunctionsByUserId(int userId)
        {
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                return null;
            }

            var functions = _userFunctionRepository.GetFunctionsByUserId(userId);
            return functions.Select(f => new FunctionDTO {
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

        public bool UpdateFunctionsForUser(int userId, List<string> functionIds, out string message)
        {
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                message = $"Người dùng với ID = {userId} không tồn tại.";
                return false;
            }

            //var existing = _userFunctionRepository.GetFunctionsByUserId(userId);
            //if (existing.Count == 0)
            //{
            //    message = $"Người dùng {userId} hiện tại không có chức năng nào";
            //    return false;
            //}
        
            try
            {
                var result = _userFunctionRepository.UpdateFunctionsForUser(userId, functionIds);
                if (result)
                {
                    message = $"Cập nhật chức năng cho người dùng thành công.";
                    _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Cập nhật chức năng cho người dùng", true, message);
                    return true;
                }
                else
                {
                    message = "Cập nhật chức năng cho người dùng thất bại";
                    _logHelper.WriteLog(NLog.LogLevel.Info, userId, null, "Cập nhật chức năng cho người dùng", false, message);
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
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật chức năng cho người dùng", false, message);
                return false;
            }
            catch (Exception ex)
            {
                message = $"Lỗi hệ thống: {ex.Message}";
                _logHelper.WriteLog(NLog.LogLevel.Error, null, null, "Cập nhật chức năng cho người dùng", false, message);
                return false;
            }
        }
    }
}
