using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLogin userForLoginDto)
        {
            if (userForLoginDto.Username.Trim().Length == 0 || userForLoginDto.Password.Trim().Length == 0)
                return BadRequest("Wrong username or password");
            else
            {
                MessageLogger user = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password, _config);

                if (user.msg[0].Obj == null)
                    return BadRequest(user.msg[0].Message);
                else
                {
                    return Ok(new
                    {
                        token = user.msg[1].Obj,
                        user = user.msg[0].Obj,
                        message = user.msg[0].Message
                    });
                }
            }
        }

        [HttpPost("loginsocial")]
        public async Task<IActionResult> LoginSocial(UserLoginSocial userLoginSocial)
        {
            MessageLogger user = await _repo.LoginSocial(userLoginSocial.Firstname, userLoginSocial.Lastname, userLoginSocial.Email, _config);

            if (user.msg[0].Obj == null)
                return Unauthorized();
            else
            {
                return Ok(new
                {
                    token = user.msg[0].Obj,
                    user = user.msg[1].Obj,
                    message = user.msg[0].Message
                });
            }
        }
    }
}
