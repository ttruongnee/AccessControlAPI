using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories.Interface
{
    public interface IFunctionRepository
    {
        List<Function> GetAll();
    }
}
