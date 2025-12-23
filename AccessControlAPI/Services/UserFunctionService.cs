using AccessControlAPI.Database;
using AccessControlAPI.DTOs;
using AccessControlAPI.Models;
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
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleFunctionRepository _roleFunctionRepository;
        private readonly IFunctionRepository _functionRepository;

        public UserFunctionService(
            LogHelper logHelper,
            IUserFunctionRepository userFunctionRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IRoleFunctionRepository roleFunctionRepository,
            IFunctionRepository functionRepository) 
        {
            _logHelper = logHelper;
            _userFunctionRepository = userFunctionRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleFunctionRepository = roleFunctionRepository;
            _functionRepository = functionRepository; 
        }

        //lấy tất cả func (từ roles + user) dạng tree
        public List<FunctionDTO> GetAllFunctionsByUserId(int userId)
        {
            //kiểm tra user tồn tại
            var user = _userRepository.GetById(userId);
            if (user == null)
                return null;

            var functionList = new List<Function>();  //dùng để lưu tất cả functions thu thập được và build tree
            var functionIds = new HashSet<string>();  //.Add trả về false nếu đã tồn tại, true nếu thêm mới, dùng để tránh duplicate

            //lấy functions từ roles
            var roles = _userRoleRepository.GetRolesByUserId(userId);
            foreach (var role in roles)  //lấy từng role nên vẫn phải check duplicate vì có thể nhiều role có cùng function
            {
                var roleFunctions = _roleFunctionRepository.GetFunctionsByRoleId(role.Id);
                foreach (var func in roleFunctions)
                {
                    if (functionIds.Add(func.Id)) // Tránh duplicate 
                    {
                        functionList.Add(func);
                    }
                }
            }

            //lấy functions từ user
            var userFunctions = _userFunctionRepository.GetFunctionsByUserId(userId);
            foreach (var func in userFunctions)
            {
                if (functionIds.Add(func.Id)) // Tránh duplicate
                {
                    functionList.Add(func);
                }
            }

            // Build tree và return
            return FunctionTreeHelper.BuildTree(functionList);
        }

        //lấy functions gán trực tiếp cho user dạng tree
        public List<FunctionDTO> GetFunctionsByUserId(int userId)
        {
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                return null;
            }

            var functions = _userFunctionRepository.GetFunctionsByUserId(userId);
            // return tree thay vì flat list
            return FunctionTreeHelper.BuildTree(functions);
        }


        //cập nhật chức năng cho user với validate EXPLICIT permissions (phải có CHA mới được có CON)
        public bool UpdateFunctionsForUser(int userId, List<string> functionIds, out string message)
        {
            //KIỂM TRA TỒN TẠI NGƯỜI DÙNG
            var existingUser = _userRepository.GetById(userId);
            if (existingUser == null)
            {
                message = $"Người dùng với ID = {userId} không tồn tại.";
                return false;
            }

            //KIỂM TRA DANH SÁCH functionIds TRUYỀN VÀO KHÔNG RỖNG
            if (functionIds == null || functionIds.Count == 0)
            {
                message = "Hàm gán cho user không được rỗng.";
                return false;
            }


            //Validate EXPLICIT permissions - KIỂM TRA PHẢI CÓ CHA MỚI ĐƯỢC CÓ CON 
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

            //nếu tồn tại parent không có con được chọn thì return false với message chi tiết
            if (parentsWithoutChildren.Count > 0)
            {
                message = $"Các chức năng cha sau phải có ít nhất 1 con: {string.Join(", ", parentsWithoutChildren)}. " +
                         "Không thể chọn chỉ cha mà không có con.";
                return false;
            }

            //cập nhật chức năng cho user
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
                message = $"Người dùng {userId} hiện tại không có chức năng riêng nào";
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
    }
}