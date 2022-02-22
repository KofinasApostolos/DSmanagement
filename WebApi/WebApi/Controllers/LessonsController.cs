using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonsRepository _repo;
        public LessonsController(ILessonsRepository repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "1")]
        [HttpPost("{id}/uploadimage")]
        public IActionResult AddImageForLesson(string id, [FromForm]PhotoForCreation photoForCreation)
        {
            var photo = _repo.AddImageForLesson(photoForCreation);

            if (photo == null)
                return BadRequest("Fail to upload image");
            else
                return Ok(photo);
        }

        [Authorize(Roles = "1")]
        [HttpPost("{id}/editimage")]
        public IActionResult EditImageForLesson(string id, [FromForm]PhotoForCreation photoForCreation)
        {
            var photo = _repo.AddImageForLesson(photoForCreation);
            var lesson = _repo.UpdateImage(id.ToString(), photo.Url, photo.PublicId);

            if (photo == null)
                return BadRequest("Fail to update image");
            else
                return Ok(photo);
        }

        [HttpGet]
        public async Task<IActionResult> GetLessons()
        {
            MessageLogger lesson = await _repo.GetLessons();
            if (lesson.msg[0].Obj == null)
                return BadRequest(lesson.msg[0].Message);
            else
                return Ok(lesson.msg[0].Obj);
        }

        [Authorize(Roles = "1,2")]
        [HttpGet("{id}/teachinglessons")]
        public async Task<IActionResult> GetTeachersLessons(string id)
        {
            MessageLogger lessons = await _repo.GetTeachersLessons(id);

            if (lessons.msg[0].Obj == null)
                return BadRequest(lessons.msg[0].Message);
            else
                return Ok(lessons.msg[0].Obj);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLesson(string id)
        {
            MessageLogger lesson = await _repo.GetCustomLesson(id);
            if (lesson.msg[0].Obj == null)
                return BadRequest(lesson.msg[0].Message);
            else
                return Ok(lesson.msg[0].Obj);
        }

        [HttpGet("teachingprogram/{id}")]
        public async Task<IActionResult> GetTeachingProgram(string id)
        {
            MessageLogger teachingProgram = await _repo.GetTeachingProgram(id);

            if (teachingProgram.msg[0].Obj == null)
                return BadRequest(teachingProgram.msg[0].Message);
            else
                return Ok(teachingProgram.msg[0].Obj);
        }

        [HttpGet("subscriptions/{id}")]
        public async Task<IActionResult> GetSubscriptions(string id)
        {
            MessageLogger subscriptions = await _repo.GetSubscriptions(id);

            if (subscriptions.msg[0].Obj == null)
                return BadRequest(subscriptions.msg[0].Message);
            else
                return Ok(subscriptions.msg[0].Obj);
        }

        [Authorize(Roles = "1")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(LessonsForRegister lessonsForRegister)
        {
            MessageLogger exist = await _repo.CheckifExist(lessonsForRegister.Lessonid);

            if (exist.msg[0].Obj != null)
            {
                return BadRequest(exist.msg[0].Message);
            }
            else
            {
                var lessonToCreate = new Lessons
                {
                    Lessonid = lessonsForRegister.Lessonid
                };

                MessageLogger createdLesson = await _repo.Register(lessonToCreate, lessonsForRegister.Lessonid, lessonsForRegister.Teacherid,
                                                            lessonsForRegister.Lesson, lessonsForRegister.Descr, lessonsForRegister.Imageurl,
                                                            lessonsForRegister.Utubeurl, lessonsForRegister.Publicid);
                if (createdLesson.msg[0].Obj == null)
                    return BadRequest(createdLesson.msg[0].Message);
                else
                    return Ok(createdLesson.msg[0]);
            }
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(string id)
        {
            MessageLogger lesson = await _repo.DeleteLesson(id);
            if (lesson.msg[0].Obj == null)
                return BadRequest(lesson.msg[0].Message);
            else
                return Ok(lesson.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditLesson(LessonsForEdit lessonsForEdit)
        {
            MessageLogger lesson = await _repo.Update(null, lessonsForEdit.Lessonid, lessonsForEdit.Teacherid, lessonsForEdit.Lesson,
                                            lessonsForEdit.Descr, lessonsForEdit.Utubeurl);

            if (lesson.msg[0].Obj == null)
                return BadRequest(lesson.msg[0].Message);
            else
                return Ok(lesson.msg[0]);
        }
    }
}