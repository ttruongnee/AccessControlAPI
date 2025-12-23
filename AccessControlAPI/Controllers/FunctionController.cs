using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : ControllerBase
    {
        public readonly IFunctionService _functionService;
        public FunctionController(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var functions = _functionService.GetAll();
            if (functions.Count == 0)
            {
                return NotFound(new { message = "Chưa có chức năng nào!" });
            }
            return Ok(functions);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id) 
        {
            id = id.Trim().ToUpper();
            var function = _functionService.GetById(id);
            if (function == null)
            {
                return NotFound(new { message = $"Không tồn tại chức năng với id {id}" });
            }
            return Ok(function);
        }
    }
}
