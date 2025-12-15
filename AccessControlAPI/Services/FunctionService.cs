using AccessControlAPI.DTOs;
using AccessControlAPI.Repositories.Interface;
using AccessControlAPI.Services.Interface;

namespace AccessControlAPI.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly IFunctionRepository _functionRepository;
        public FunctionService(IFunctionRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public List<FunctionDTO> GetAll()
        {
            var functions = _functionRepository.GetAll();
            return functions.Select(f => new FunctionDTO
            {
                Id = f.Id,
                Name = f.Name,
                Sort_order = f.Sort_order,
                Parent_id = f.Parent_id,
                Show_search = f.Show_search,
                Show_add = f.Show_add,
                Show_update = f.Show_update,
                Show_delete = f.Show_delete
            }).ToList();
        }
        public FunctionDTO GetById(string id)
        {
            var function = _functionRepository.GetById(id);
            if (function == null) return null;

            return new FunctionDTO
            {
                Id = function.Id,
                Name = function.Name,
                Sort_order = function.Sort_order,
                Parent_id = function.Parent_id,
                Show_search = function.Show_search,
                Show_add = function.Show_add,
                Show_update = function.Show_update,
                Show_delete = function.Show_delete
            };
        }

        public List<FunctionDTO> GetChildren(string parentId)
        {
            var functions = _functionRepository.GetChildren(parentId);
            return functions.Select(f => new FunctionDTO
            {
                Id = f.Id,
                Name = f.Name,
                Sort_order = f.Sort_order,
                Parent_id = f.Parent_id,
                Show_search = f.Show_search,
                Show_add = f.Show_add,
                Show_update = f.Show_update,
                Show_delete = f.Show_delete
            }).ToList();
        }

        public List<FunctionDTO> GetParents()
        {
            var functions = _functionRepository.GetParents();
            return functions.Select(f => new FunctionDTO
            {
                Id = f.Id,
                Name = f.Name,
                Sort_order = f.Sort_order,
                Parent_id = f.Parent_id,
                Show_search = f.Show_search,
                Show_add = f.Show_add,
                Show_update = f.Show_update,
                Show_delete = f.Show_delete
            }).ToList();
        }
    }
}
