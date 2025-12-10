namespace AccessControlAPI.Models
{
    public class UserFunction
    {
        public int UserId { get; set; }
        public string FunctionId { get; set; }

        public UserFunction() { }

        public UserFunction(int userId, string functionId)
        {
            UserId = userId;
            FunctionId = functionId;
        }
    }
}
