using AccessControlAPI.DTOs;
using AccessControlAPI.Models;

namespace AccessControlAPI.Utils
{
    public static class FunctionTreeHelper
    {
        /// <summary>
        /// Build tree từ flat list functions - ALL IN ONE
        /// </summary>
        public static List<FunctionDTO> BuildTree(List<Function> functions)
        {
            var tree = new Dictionary<string, FunctionDTO>();

            // Bước 1: Convert sang DTO
            foreach (var func in functions)
            {
                tree[func.Id] = new FunctionDTO
                {
                    Id = func.Id,
                    Name = func.Name,
                    Parent_id = func.Parent_id,
                    Sort_order = func.Sort_order,
                    Show_search = func.Show_search,
                    Show_add = func.Show_add,
                    Show_update = func.Show_update,
                    Show_delete = func.Show_delete,
                    Children = new List<FunctionDTO>()
                };
            }

            // Bước 2: Nối children vào parent
            foreach (var func in functions)
            {
                if (!string.IsNullOrEmpty(func.Parent_id) && tree.ContainsKey(func.Parent_id))
                {
                    tree[func.Parent_id].Children.Add(tree[func.Id]);
                }
            }

            // Bước 3: Lấy roots + sort
            var roots = tree.Values
                .Where(n => string.IsNullOrEmpty(n.Parent_id))
                .OrderBy(n => n.Sort_order)
                .ToList();

            SortChildren(roots);
            return roots;
        }

        /// <summary>
        /// Sort children đệ quy
        /// </summary>
        private static void SortChildren(List<FunctionDTO> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Children.Count > 0)
                {
                    node.Children = node.Children.OrderBy(c => c.Sort_order).ToList();
                    SortChildren(node.Children);
                }
            }
        }
    }
}