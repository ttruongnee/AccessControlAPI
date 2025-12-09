namespace AccessControlAPI.DTOs
{
    public class FunctionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sort_order { get; set; }
        public int? Parent_id { get; set; }
        public bool Show_search { get; set; }
        public bool Show_add { get; set; }
        public bool Show_update { get; set; }
        public bool Show_delete { get; set; }
    }
}
