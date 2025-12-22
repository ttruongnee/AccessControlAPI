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
        private readonly IFunctionRepository _functionRepository;
        private readonly IRoleFunctionRepository _roleFunctionRepository;
        private readonly LogHelper _logHelper;
        public RoleFunctionService(IRoleRepository roleRepository, IRoleFunctionRepository roleFunctionRepository, LogHelper logHelper, IFunctionRepository functionRepository)
        {
            _roleRepository = roleRepository;
            _roleFunctionRepository = roleFunctionRepository;
            _logHelper = logHelper;
            _functionRepository = functionRepository;
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
            return FunctionTreeHelper.BuildTree(functions);
        }

        public bool UpdateFunctionsForRole(int roleId, List<string> functionIds, out string message)
        {
            var existingRole = _roleRepository.GetById(roleId);
            if (existingRole == null)
            {
                message = $"Vai trò với ID = {roleId} không tồn tại";
                return false;
            }

            if (functionIds == null || functionIds.Count == 0)
            {
                message = "Danh sách chức năng không được rỗng.";
                return false;
            }

            //Validate EXPLICIT permissions - Phải có CHA mới được có CON
            var allFunctions = _functionRepository.GetAll();  //lấy toàn bộ functions trong hệ thống
            var functionDict = allFunctions.ToDictionary(f => f.Id);  //chuyển sang dictionary để tra cứu nhanh
            var missingParents = new List<string>();  //danh sách parent bị thiếu (tức là có con nhưng không có cha)

            foreach (var functionId in functionIds)
            {
                //kiểm tra functionId có tồn tại không
                if (!functionDict.ContainsKey(functionId))
                {
                    message = $"Function ID không tồn tại: {functionId}";
                    return false;
                }

                var function = functionDict[functionId];

                //nếu có parent_id
                if (!string.IsNullOrEmpty(function.Parent_id))
                {
                    //kiểm tra parent_id có trong functionIds (danh sách gán cho user mà người dùng truyền vào) không
                    if (!functionIds.Contains(function.Parent_id))  //nếu không có cha mà có con
                    {
                        //thêm vào danh sách parent bị thiếu (nếu chưa có)
                        if (!missingParents.Contains(function.Parent_id))
                        {
                            missingParents.Add(function.Parent_id);
                        }
                    }
                }
            }

            //nếu tồn tại parent bị thiếu thì return false với message chi tiết
            if (missingParents.Count > 0)
            {
                message = $"Thiếu parent functions: {string.Join(", ", missingParents)}. " +
                         "Phải có parent mới được chọn children.";
                return false;
            }


            //KIỂM TRA CHA PHẢI CÓ ÍT NHẤT 1 CON
            var parentsWithoutChildren = new List<string>();

            foreach (var functionId in functionIds)
            {
                var function = functionDict[functionId];

                //allFunctions đã lấy toàn bộ function trong hệ thống từ trước
                //kiểm tra functionId có con không, nếu có thì phải kiểm tra con có được chọn không
                var hasChildren = allFunctions.Any(f => f.Parent_id == functionId);

                if (hasChildren)
                {
                    // Lấy tất cả con của parent này trong hệ thống và kiểm tra xem có con nào trong số đó được chọn không
                    var selectedChildren = allFunctions
                        //kiểm tra con có functionId có trong hệ thống không VÀ kiểm tra con đó có được chọn không (con có tồn tại trong functionIds không)
                        .Where(f => f.Parent_id == functionId && functionIds.Contains(f.Id))
                        .ToList();

                    if (selectedChildren.Count == 0)
                    {
                        // Parent không có con nào được chọn
                        parentsWithoutChildren.Add(functionId);
                    }
                }
            }

            if (parentsWithoutChildren.Count > 0)
            {
                message = $"Các chức năng cha sau phải có ít nhất 1 con: {string.Join(", ", parentsWithoutChildren)}. " +
                         "Không thể chọn chỉ cha mà không có con.";
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
