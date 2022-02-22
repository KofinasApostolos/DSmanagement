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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeachingProgramController : ControllerBase
    {
        private readonly ITeachingProgramRepository _repo;
        public TeachingProgramController(ITeachingProgramRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeachingPrograms()
        {
            MessageLogger teachingPrograms = await _repo.GetTeachingPrograms();

            if (teachingPrograms.msg[0].Obj == null)
                return BadRequest(teachingPrograms.msg[0].Message);
            else
                return Ok(teachingPrograms.msg[0].Obj);
        }

        //[HttpGet("{days}")]
        //public async Task<IActionResult> GetDays()
        //{
        //    MessageLogger days = await _repo.GetDays();

        //    if (days.msg[0].Obj == null)
        //        return BadRequest(days.msg[0].Message);
        //    else
        //        return Ok(days.msg[0].Obj);
        //}

        [HttpGet("cdays")]
        public async Task<IActionResult> GetCustomDays()
        {
            MessageLogger days = await _repo.GetCustomDays();

            if (days.msg[0].Obj == null)
                return BadRequest(days.msg[0].Message);
            else
                return Ok(days.msg[0].Obj);
        }

        [HttpGet("{id}")]
        public async Task<Object> GetTeachingProgram(string id)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != id.ToString())
                return Unauthorized();
            else
            {
                MessageLogger teachingProgram = await _repo.GetTeachingProgram(id);

                if (teachingProgram.msg[0].Obj == null)
                    return BadRequest(teachingProgram.msg[0].Message);
                else
                    return Ok(teachingProgram.msg[0].Obj);
            }
        }

        [HttpPost("usersuggestion")]
        public async Task<IActionResult> UserSuggestionProgram(UserSuggestionForRegister userSuggestionForRegister)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != userSuggestionForRegister.Userid.ToString())
                return Unauthorized();
            else
            {
                MessageLogger createdTeachingProgram = await _repo.UserSuggestionRegister(userSuggestionForRegister.Userid, userSuggestionForRegister.Lessonid,
                                                            userSuggestionForRegister.Dayofweek, userSuggestionForRegister.Lessonstart, userSuggestionForRegister.Lessonend);

                if (createdTeachingProgram.msg[0].Obj == null)
                    return BadRequest(createdTeachingProgram.msg[0].Message);
                else
                    return Ok(createdTeachingProgram.msg[0]);
            }
        }

        [HttpGet("{id}/getsuggestionsuser")]
        public async Task<IActionResult> GetSuggestionsUser(string id)
        {
            MessageLogger suggestions = await _repo.GetSuggestionsUser(id);

            if (suggestions.msg[0].Obj == null)
                return BadRequest(suggestions.msg[0].Message);
            else
                return Ok(suggestions.msg[0].Obj);
        }

        [Authorize(Roles = "1")]
        [HttpGet("getsuggestionadmin")]
        public async Task<IActionResult> GetSuggestionsAdmin()
        {
            MessageLogger suggestions = await _repo.GetSuggestionsAdmin();

            if (suggestions.msg[0].Obj == null)
                return BadRequest(suggestions.msg[0].Message);
            else
                return Ok(suggestions.msg[0].Obj);
        }

        [Authorize(Roles = "1")]
        [HttpPost("transfer2core")]
        public async Task<IActionResult> Τransfer2Core(Transfer2CoreProgram transfer2CoreProgram)
        {
            var teachingProgramToCreate = new TeachingProgram
            {
                Lessonid = transfer2CoreProgram.Lessonid
            };

            MessageLogger createdTeachingProgram = await _repo.Τransfer2Core(teachingProgramToCreate,
                                                                   transfer2CoreProgram.Lessonid, transfer2CoreProgram.Dayofweek,
                                                                   transfer2CoreProgram.Lessonstart, transfer2CoreProgram.Lessonend);

            if (createdTeachingProgram.msg[0].Obj == null)
                return BadRequest(createdTeachingProgram.msg[0].Message);
            else
                return Ok(createdTeachingProgram.msg[0]);
        }

        [HttpPost("delsuggestionuser/{id}")]
        public async Task<ActionResult<TeachingProgramTemp>> DeleteSuggestion(TeachingProgramSuggestionForDelete teachingProgramSuggestionForDelete, string id)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != id)
                return Unauthorized();
            else
            {
                MessageLogger teachingProgram = await _repo.DeleteTeachingProgramSuggestion(teachingProgramSuggestionForDelete.Id, teachingProgramSuggestionForDelete.Lessonid,
                                                            teachingProgramSuggestionForDelete.Day, teachingProgramSuggestionForDelete.Lessonstart,
                                                            teachingProgramSuggestionForDelete.Lessonend, teachingProgramSuggestionForDelete.Count, id);

                if (teachingProgram.msg[0].Obj == null)
                    return BadRequest(teachingProgram.msg[0].Message);
                else
                    return Ok(teachingProgram.msg[0]);
            }
        }

        [Authorize(Roles = "1")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(TeachingProgramForRegister teachingProgramForRegister)
        {
            MessageLogger exist = await _repo.CheckIfExist(teachingProgramForRegister.Lessonid, teachingProgramForRegister.Id,
                                                        teachingProgramForRegister.Lessonstart, teachingProgramForRegister.Lessonend);
            if (exist.msg[0].Obj != null)
            {
                return BadRequest("Teaching Program already exist");
            }
            else
            {
                var teachingProgramToCreate = new TeachingProgram
                {
                    Lessonid = teachingProgramForRegister.Lessonid
                };

                var createdTeachingProgram = await _repo.Register(teachingProgramToCreate, teachingProgramForRegister.Lessonid, teachingProgramForRegister.Id,
                                                            teachingProgramForRegister.Lessonstart, teachingProgramForRegister.Lessonend, teachingProgramForRegister.Capacity);

                if (createdTeachingProgram.msg[0].Obj == null)
                    return BadRequest(createdTeachingProgram.msg[0].Message);
                else
                    return Ok(createdTeachingProgram.msg[0]);
            }
        }

        [HttpPost("registerclass")]
        public async Task<IActionResult> RegisterClass(ClassRegister classRegister)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != classRegister.Userid.ToString())
                return Unauthorized();
            else
            {
                MessageLogger registerClassroom = await _repo.RegisterClassroom(classRegister, classRegister.Capacity.ToString(), classRegister.Day,
                                                        classRegister.Lessonstart, classRegister.Lessonsend, classRegister.Lessonid, classRegister.Userid);

                if (registerClassroom.msg[0].Obj == null)
                    return BadRequest(registerClassroom.msg[0].Message);
                else
                    return Ok(registerClassroom.msg[0]);
            }
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<TeachingProgram>> DeleteTeachingProgram(string id)
        {
            MessageLogger teachingProgram = await _repo.DeleteTeachingProgram(id);

            if (teachingProgram.msg[0].Obj == null)
                return BadRequest(teachingProgram.msg[0].Message);
            else
                return Ok(teachingProgram.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpPost("{id}/tpxls")]
        public async Task<IActionResult> Create_And_WriteXLS_TP(string id, [FromBody]List<CustomTP> customTPs)
        {
            if (customTPs.Count == 0)
            {
                return BadRequest("There are is no teaching program to export");
            }
            else
            {
                MessageLogger xlsPayload = await _repo.Create_And_WriteXLS_TP(id, customTPs);

                if (xlsPayload.msg[0].Obj == null)
                {
                    return BadRequest(xlsPayload.msg[0].Message);
                }
                else
                {
                    return Ok(xlsPayload.msg[0]);
                }
            }
        }

        [HttpPut("{id}/updatesuggestion")]
        public async Task<IActionResult> EditTeachingProgram(string id, TeachingProgramTempForEdit teachingProgramTempForEdit)
        {
            MessageLogger teachingProgramTemp = await _repo.UpdateUserSuggestion(null, id, teachingProgramTempForEdit.Id, teachingProgramTempForEdit.Lessonid,
                                                                teachingProgramTempForEdit.Dayofweek, teachingProgramTempForEdit.Lessonstart,
                                                                teachingProgramTempForEdit.Lessonend);

            if (teachingProgramTemp.msg[0].Obj == null)
                return BadRequest(teachingProgramTemp.msg[0].Message);
            else
                return Ok(teachingProgramTemp.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpPut]
        public async Task<IActionResult> EditTeachingProgram(TeachingProgramForEdit teachingProgramForEdit)
        {
            MessageLogger teachingProgram = await _repo.Update(null, teachingProgramForEdit.Id, teachingProgramForEdit.Lessonid, teachingProgramForEdit.Dayofweek,
                                                     teachingProgramForEdit.Lessonstart, teachingProgramForEdit.Lessonend, teachingProgramForEdit.Capacity);

            if (teachingProgram.msg[0].Obj == null)
                return BadRequest(teachingProgram.msg[0].Message);
            else
                return Ok(teachingProgram.msg[0]);
        }
    }
}