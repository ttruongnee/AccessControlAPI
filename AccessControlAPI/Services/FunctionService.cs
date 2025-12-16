using AccessControlAPI.DTOs;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Routing;

namespace AccessControlAPI.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly IFunctionRepository _functionRepository;
        public FunctionService(IFunctionRepository functionRepository, Utils.LogHelper _logHelper)
        {
            _functionRepository = functionRepository;
        }        
        
        public List<FunctionDTO> GetAll()
        {
            //lấy toàn bộ functions đã build tree
            var tree = BuildCompleteTree();

            //chỉ lấy những node gốc (không có parent_id)
            var roots = tree.Values
                .Where(node => string.IsNullOrEmpty(node.Parent_id))  //lấy những node không có parent_id
                .OrderBy(node => node.Sort_order)  //sắp xếp theo sort_order
                .ToList();

            //sort tất cả children theo sort_order
            SortAllChildren(roots);

            return roots;
        }

        public FunctionDTO GetById(string id)
        {
            //lấy toàn bộ functions đã build tree
            var tree = BuildCompleteTree();

            //kiểm tra node có tồn tại không
            if (!tree.ContainsKey(id))
            {
                return null;
            }

            //lấy ra node đó (đã có sẵn children)
            var node = tree[id];

            //sort tất cả children của node này
            //FunctionDTO là class -> kiểu tham chiếu nên mọi thứ thay đổi trên node.Childen đều tác động trực tiếp đến obj gốc
            SortAllChildren(new List<FunctionDTO> { node });  

            return node;
        }

        //xây dựng cây đã gán con cho toàn bộ cha
        private Dictionary<string, FunctionDTO> BuildCompleteTree()
        {
            //lấy ra toàn bộ function
            var allFunctions = _functionRepository.GetAll();

            //convert tất cả sang DTO và cho vào Dictionary
            var tree = new Dictionary<string, FunctionDTO>();
            foreach (var func in allFunctions)
            {
                tree[func.Id] = ConvertToDTO(func);
            }

            //nối children vào parent
            foreach (var func in allFunctions)
            {
                //nếu có parent
                if (!string.IsNullOrEmpty(func.Parent_id))
                {
                    //và parent tồn tại trong tree
                    if (tree.ContainsKey(func.Parent_id))
                    {
                        //thêm child vào parent
                        var parent = tree[func.Parent_id];
                        var child = tree[func.Id];
                        parent.Children.Add(child);
                    }
                }
            }

            return tree;
        }

        //truyền vào model func trả về func dto
        private FunctionDTO ConvertToDTO(Function function)
        {
            return new FunctionDTO
            {
                Id = function.Id,
                Name = function.Name,
                Parent_id = function.Parent_id,
                Sort_order = function.Sort_order,
                Show_search = function.Show_search,
                Show_add = function.Show_add,
                Show_update = function.Show_update,
                Show_delete = function.Show_delete
            };
        }

        private void SortAllChildren(List<FunctionDTO> nodes)
        {
            foreach (var node in nodes)
            {
                //nếu node có con thì sắp xếp theo sort_order
                if (node.Children.Count > 0)
                {
                    node.Children = node.Children
                                    .OrderBy(child => child.Sort_order)
                                    .ToList();

                    //đệ quy sắp xếp con của con đến khi hết các node (nào check node.Children.Count < 0 thì nó k gọi nữa)
                    SortAllChildren(node.Children);
                }
            }
        }
    }
}
