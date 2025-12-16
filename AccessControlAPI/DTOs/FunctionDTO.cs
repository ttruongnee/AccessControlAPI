using AccessControlAPI.Models;

namespace AccessControlAPI.DTOs
{
    public class FunctionDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Sort_order { get; set; }
        public string Parent_id { get; set; }
        public bool Show_search { get; set; }
        public bool Show_add { get; set; }
        public bool Show_update { get; set; }
        public bool Show_delete { get; set; }
        public List<FunctionDTO> Children { get; set; } = new List<FunctionDTO>();
    }
    public class FunctionNoChildrenDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Sort_order { get; set; }
        public string Parent_id { get; set; }
        public bool Show_search { get; set; }
        public bool Show_add { get; set; }
        public bool Show_update { get; set; }
        public bool Show_delete { get; set; }
    }
}
