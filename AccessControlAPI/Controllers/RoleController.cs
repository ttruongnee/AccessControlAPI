using AccessControlAPI.DTOs;
using AccessControlAPI.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var roles = _roleService.GetAll();
            if (roles.Count == 0)
            {
                return NotFound(new { message = "Chưa có vai trò nào!"});
            }
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var role = _roleService.GetById(id);
            if (role == null)
            {
                return NotFound(new { message = $"Không tồn tại vai trò với id {id}" });
            }
            return Ok(role);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUpdateRoleDTO role)
        {
            if (role == null)
            {
                return BadRequest(new { message = "Dữ liệu vai trò không hợp lệ" });
            }

            if (_roleService.Create(role, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CreateUpdateRoleDTO role)
        {
            if (role == null)
            {
                return BadRequest(new { message = "Dữ liệu vai trò không hợp lệ" });
            }
            if (_roleService.Update(id, role, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_roleService.Delete(id, out string message))
            {
                return Ok(new { message });
            }
            else
            {
                return BadRequest(new { message });
            }
        } 
    }

}