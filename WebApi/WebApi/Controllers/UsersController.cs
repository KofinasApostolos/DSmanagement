using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.CustomModel;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _repo;
        public UsersController(IUsersRepository repo)
        {
            _repo = repo;
        }

        [Authorize]
        [HttpPost("{id}/uploadimage")]
        public async Task<IActionResult> AddImageForUser(string id, [FromForm]PhotoForCreation photoForCreation)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != id.ToString())
            {
                return Unauthorized();
            }
            else
            {
                MessageLogger photo = await _repo.AddImageForUser(photoForCreation);

                MessageLogger user = await _repo.UpdateImage(id.ToString(), photo.msg[0].Obj);

                if (photo.msg[0].Obj == null)
                    return BadRequest(photo.msg[0].Message);
                else
                    return Ok(photo.msg[0].Obj);
            }
        }

        [Authorize(Roles = "1")]
        [HttpPost("{id}/usersxls")]
        public async Task<IActionResult> Create_And_WriteXLS_Users(string id, [FromBody]List<CustomUsers> customUsers)
        {
            if (customUsers.Count == 0)
                return BadRequest("There are no users to export");

            MessageLogger xlsPayload = await _repo.Create_And_WriteXLS_Users(id, customUsers);

            if (xlsPayload.msg[0].Obj == null)
                return BadRequest(xlsPayload.msg[0].Message);
            else
                return Ok(xlsPayload.msg[0]);
        }

        [Authorize]
        [HttpPost("{id}/editimage")]
        public async Task<IActionResult> EditImageForUser(string id, [FromForm]PhotoForCreation photoForCreation)
        {
            MessageLogger photo = await _repo.AddImageForUser(photoForCreation);

            MessageLogger user = await _repo.UpdateImage(id.ToString(), photo.msg[0].Obj);

            if (photo.msg[0].Obj == null)
                return BadRequest(photo.msg[0].Message);
            else
            {
                return Ok(photo.msg[0].Obj);
            }
        }

        [Authorize(Roles = "1")]
        [HttpPost("{id}/uploadimageadmin")]
        public async Task<IActionResult> EditImageForUserAdmin(string id, [FromForm]PhotoForCreation photoForCreation)
        {
            MessageLogger photo = await _repo.AddImageForUser(photoForCreation);

            if (photo.msg[0].Obj == null)
                return BadRequest(photo.msg[0].Message);
            else
                return Ok(photo.msg[0]);
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            MessageLogger passwordReset = await _repo.ResetPassword(resetPassword.Email);

            if (passwordReset.msg[0].Obj == null)
                return BadRequest(passwordReset.msg[0].Message);
            else
                return Ok(passwordReset.msg[0]);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister userForRegisterDto)
        {
            MessageLogger exist = await _repo.UserExists(userForRegisterDto.username);
            if (exist.msg[0].Obj != null)
            {
                return BadRequest("User AlreadyExists");
            }
            else
            {
                var userToCreate = new User
                {
                    Username = userForRegisterDto.username
                };

                MessageLogger createdUser = await _repo.Register(userToCreate, userForRegisterDto.password, userForRegisterDto.firstname,
                                                       userForRegisterDto.lastname, userForRegisterDto.city, userForRegisterDto.area,
                                                       userForRegisterDto.street, userForRegisterDto.phonenumber, DateTime.Parse(userForRegisterDto.birthdate),
                                                       userForRegisterDto.email, userForRegisterDto.isadmin, userForRegisterDto.imageurl, userForRegisterDto.descr,
                                                       userForRegisterDto.PublicId);

                if (createdUser.msg[0].Obj == null)
                    return BadRequest(createdUser.msg[0].Message);
                else
                    return Ok(createdUser.msg[0]);
            }
        }

        [Authorize(Roles = "1,2,0")]
        [HttpPut("profile/{id}")]
        public async Task<IActionResult> EditUserProfile(string id, UserForEdit userForEditDto)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != id.ToString())
                return Unauthorized();
            else
            {
                MessageLogger user = await _repo.UpdateUserProfile(null, userForEditDto.username, userForEditDto.firstname, userForEditDto.lastname,
                                     userForEditDto.city, userForEditDto.street, userForEditDto.area, userForEditDto.birthdate,
                                     userForEditDto.email, userForEditDto.phonenumber, userForEditDto.password, userForEditDto.descr);
                if (user.msg[0].Obj == null)
                    return BadRequest(user.msg[0].Message);
                else
                    return Ok(user.msg[0]);
            }
        }

        [Authorize(Roles = "1")]
        [HttpPut]
        public async Task<IActionResult> EditUser(UserForEdit userForEditDto)
        {
            MessageLogger user = await _repo.UpdateUser(null, userForEditDto.username, userForEditDto.firstname, userForEditDto.lastname,
                                     userForEditDto.city, userForEditDto.street, userForEditDto.area, userForEditDto.birthdate,
                                     userForEditDto.email, userForEditDto.phonenumber, userForEditDto.password, userForEditDto.descr, userForEditDto.isadmin.ToString());
            if (user.msg[0].Obj == null)
                return BadRequest(user.msg[0].Message);
            else
                return Ok(user.msg[0]);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            MessageLogger users = await _repo.GetUsers();

            if (users.msg[0].Obj == null)
                return BadRequest(users.msg[0].Message);
            else
                return Ok(users.msg[0].Obj);

        }

        [Authorize(Roles = "1")]
        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            MessageLogger teachers = await _repo.GetTeachers();

            if (teachers.msg[0].Obj == null)
                return BadRequest(teachers.msg[0].Message);
            else
                return Ok(teachers.msg[0].Obj);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != id.ToString())
                return Unauthorized();
            else
            {
                MessageLogger user = await _repo.GetUser(id);

                if (user.msg[0].Obj == null)
                    return BadRequest(user.msg[0].Message);
                else
                    return Ok(user.msg[0].Obj);
            }
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;
            MessageLogger user = await _repo.DeleteUser(id, Int32.Parse(iClaim));

            if (user.msg[0].Obj == null)
                return BadRequest(user.msg[0].Message);
            else
                return Ok(user.msg[0]);
        }
    }
}
