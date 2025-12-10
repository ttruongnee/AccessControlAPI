using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IFunctionRepository
    {
        List<Function> GetAll();
        Function GetById(string id);
        List<Function> GetChildren(string parentId);
        List<Function> GetParents();

    }
}
