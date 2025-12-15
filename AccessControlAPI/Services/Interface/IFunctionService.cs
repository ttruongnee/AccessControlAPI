using AccessControlAPI.DTOs;

namespace AccessControlAPI.Services.Interface
{
    public interface IFunctionService
    {
        List<FunctionDTO> GetAll();
        FunctionDTO GetById(string id);
        public List<FunctionDTO> GetChildren(string parentId);
        List<FunctionDTO> GetParents();
    }
}
