using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesRepository _repo;
        public RolesController(IRolesRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            MessageLogger roles = await _repo.GetRoles();

            if (roles.msg[0].Obj == null)
                return BadRequest(roles.msg[0].Message);
            else
                return Ok(roles.msg[0].Obj);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoles(short id)
        {
            MessageLogger role = await _repo.GetRole(id);

            if (role.msg[0].Obj == null)
                return BadRequest(role.msg[0].Message);
            else
                return Ok(role.msg[0].Obj);
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(short id)
        {
            MessageLogger role = await _repo.DeleteRole(id);

            if (role.msg[0].Obj == null)
                return BadRequest(role.msg[0].Message);
            else
                return Ok(role.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpPut]
        public async Task<IActionResult> UpdateRole(RoleForEdit roleForEdit)
        {
            MessageLogger role = await _repo.UpdateRole(null, roleForEdit.RoleCode, roleForEdit.RoleDescr);

            if (role.msg[0].Obj == null)
                return BadRequest(role.msg[0].Message);
            else
                return Ok(role.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RoleForRegister roleForRegister)
        {
            MessageLogger exist = await _repo.RoleExists(roleForRegister.RoleCode);
            if (exist.msg[0].Obj != null)
            {
                return BadRequest(exist.msg[0]);
            }
            else
            {
                var roleToCreate = new Roles
                {
                    RoleCode = roleForRegister.RoleCode
                };

                MessageLogger createdRole = await _repo.Register(roleToCreate, roleForRegister.RoleCode, roleForRegister.RoleDescr);

                if (createdRole.msg[0].Obj == null)
                    return BadRequest(createdRole.msg[0].Message);
                else
                    return Ok(createdRole.msg[0]);
            }
        }
    }
}
