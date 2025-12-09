namespace AccessControlAPI.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Role() { }
        public Role(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
