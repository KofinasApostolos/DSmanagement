using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.CustomModel;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using static WebApi.CustomModel.CustomMessageLogger;
using static WebApi.Helpers.MessageLogger;
using System.Data;

namespace WebApi.Data
{
    public class LessonsRepository : ControllerBase, ILessonsRepository
    {
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly DanceSchoolContext _context;
        public LessonsRepository(DanceSchoolContext context, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
            _cloudinaryConfig.Value.CloudName,
            _cloudinaryConfig.Value.ApiKey,
            _cloudinaryConfig.Value.ApiSecret
           );
            _cloudinary = new Cloudinary(acc);
            _context = context;
        }

        public async Task<MessageLogger> DeleteLesson(string lessonid)
        {
            MessageLogger messageLogger = new MessageLogger();
            var lesson = (object)null;
            try
            {
                lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Lessonid == lessonid);

                if (lesson != null)
                {
                    _context.Remove(lesson);
                    await _context.SaveChangesAsync();
                }

                messageLogger.AddMessage("Lesson has been Deleted!", lesson, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }


        public async Task<MessageLogger> GetLessons()
        {
            MessageLogger messageLogger = new MessageLogger();
            var lessons = (object)null;
            try
            {
                lessons = await _context.Lessons.ToListAsync();

                messageLogger.AddMessage("", lessons, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetCustomLesson(string lessonid)
        {
            MessageLogger messageLogger = new MessageLogger();
            var customLesson = (object)null;
            try
            {
                customLesson = await (from l in _context.Lessons
                                      join t in _context.User
                                      on l.Teacherid equals t.Id
                                      join s in _context.Subscriptions
                                      on l.Lessonid equals s.Lessonid
                                      where l.Lessonid == lessonid
                                      //where s.Discount == true
                                      select new
                                      {
                                          l.Lessonid,
                                          Teacherid = t.Id,
                                          Firstname = t.FirstName,
                                          Lastname = t.LastName,
                                          ImageurlTeacher = t.ImageUrl,
                                          DescrTeacher = t.Descr,
                                          ImageurlLesson = l.ImageUrl,
                                          l.Lesson,
                                          DescrLesson = l.Descr,
                                          s.Price,
                                          s.Discount,
                                          Discountprice = s.Discprice,
                                          l.Utubeurl
                                      }).FirstOrDefaultAsync();

                messageLogger.AddMessage("", customLesson, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetSubscriptions(string lessonid)
        {
            MessageLogger messageLogger = new MessageLogger();
            var subscriptions = (object)null;
            try
            {
                subscriptions = await (from s in _context.Subscriptions
                                       where s.Lessonid == lessonid
                                       //where s.Discount == false
                                       select new
                                       {
                                           s.Id,
                                           s.Lessonid,
                                           Duration = Int32.Parse(s.Duration),
                                           s.Price,
                                           s.Discount
                                       }).OrderBy(x => x.Duration).ToListAsync();

                messageLogger.AddMessage("", subscriptions, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetLesson(string lessonid)
        {
            MessageLogger messageLogger = new MessageLogger();
            var lesson = (object)null;
            try
            {
                lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Lessonid == lessonid);
                messageLogger.AddMessage("", lesson, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> CheckifExist(string lessonid)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var checkExist = await (from l in _context.Lessons
                                        where l.Lessonid == lessonid
                                        select l).ToListAsync();

                if (checkExist.Count > 0)
                {
                    messageLogger.AddMessage("", checkExist, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    messageLogger.AddMessage("Lesson already exists!", null, MessageCode.Warning);
                    return messageLogger;
                }

            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Register(Lessons lessons, string lessonid, string teacherid,
                                            string lesson, string descr, string imageurl, string utubeurl, string publicid)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                lessons.Lessonid = lessonid;
                lessons.Lesson = lesson;
                lessons.Teacherid = Int16.Parse(teacherid);
                lessons.Descr = descr;
                lessons.ImageUrl = imageurl;
                lessons.Utubeurl = utubeurl;
                lessons.PublicId = publicid;

                await _context.Lessons.AddAsync(lessons);
                await _context.SaveChangesAsync();

                messageLogger.AddMessage("Lesson has been inserted!", lessons, MessageCode.Warning);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Update(Lessons lessons, string lessonid, string teacherid, string lesson, string descr, string utubeurl)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                lessons = await _context.Lessons.FirstOrDefaultAsync(l => l.Lessonid == lessonid);

                if (lessons != null)
                {
                    lessons.Teacherid = Int16.Parse(teacherid);
                    lessons.Lesson = lesson.Trim();
                    lessons.Descr = descr.Trim();
                    lessons.Utubeurl = utubeurl.Trim();

                    await _context.SaveChangesAsync();
                }
                messageLogger.AddMessage("Lesson has been updated!", lessons, MessageCode.Warning);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetTeachingProgram(string lessonid)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                List<CustomTeachingProgram> customTeachingPrograms = new List<CustomTeachingProgram>();

                DataTable daysdt = MemoryTables.DaysTable();

                var teachingProgram = await (from l in _context.Lessons
                                             join tp in _context.TeachingProgram
                                             on l.Lessonid equals tp.Lessonid
                                             join d in daysdt.AsEnumerable()
                                             on tp.Dayofweek equals d.Field<int>("Id")
                                             where l.Lessonid == lessonid
                                             select new
                                             {
                                                 tp.Id,
                                                 tp.Lessonid,
                                                 Dayofweek = d.Field<string>("Descr"),
                                                 tp.Lessonstart,
                                                 tp.Lessonend,
                                                 tp.Capacity
                                             }).ToListAsync();

                foreach (var itm in teachingProgram.ToList())
                {
                    var capacity = (from tr in _context.TempRegisters
                                    where tr.Teachingprogramid == itm.Id
                                    select tr).Count();

                    var fixcapacity = itm.Capacity - capacity;
                    customTeachingPrograms.Add(new CustomTeachingProgram(itm.Id.ToString(), itm.Lessonid, itm.Dayofweek,
                                                   itm.Lessonstart, itm.Lessonend, fixcapacity.ToString()));
                }
                messageLogger.AddMessage("", customTeachingPrograms, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public PhotoForCreation AddImageForLesson(PhotoForCreation photoForCreation)
        {
            try
            {
                var file = photoForCreation.File;
                var uploadResult = new ImageUploadResult();

                if (file == null)
                    return null;
                else
                {
                    if (file.Length > 0 && file.Length < 24000000)
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            var uploadParams = new ImageUploadParams()
                            {
                                File = new FileDescription(file.Name, stream),
                                Transformation = new Transformation()
                                .Width(500).Height(500).Crop("fill").Gravity("face")
                            };
                            uploadResult = _cloudinary.Upload(uploadParams);
                        }
                    }
                    photoForCreation.Url = uploadResult.Uri.ToString();
                    photoForCreation.PublicId = uploadResult.PublicId;
                    //var user = _repo.UpdateImage(id.ToString(), photoForCreation.Url, photoForCreation.PublicId);
                    return photoForCreation;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return null;
            }
        }

        public async Task<Lessons> UpdateImage(string id, string imgurl, string pubid)
        {
            try
            {
                var lesson = await (from l in _context.Lessons
                                    where l.Lessonid == id
                                    select l).FirstOrDefaultAsync();

                if (lesson != null)
                {
                    lesson.ImageUrl = imgurl;
                    lesson.PublicId = pubid;
                    _context.SaveChanges();
                }
                return lesson;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<MessageLogger> GetTeachersLessons(string teacherid)
        {
            MessageLogger messageLogger = new MessageLogger();
            var teachinglesson = (object)null;
            try
            {

                DataTable daysdt = MemoryTables.DaysTable();

                teachinglesson = await (from l in _context.Lessons
                                        join tp in _context.TeachingProgram
                                        on l.Lessonid equals tp.Lessonid
                                        join d in daysdt.AsEnumerable()
                                        on tp.Dayofweek equals d.Field<int>("Id")
                                        join u in _context.User
                                        on l.Teacherid equals u.Id
                                        where u.Id == Int32.Parse(teacherid)
                                        select new
                                        {
                                            l.Lesson,
                                            Day = d.Field<string>("Descr"),
                                            tp.Lessonstart,
                                            tp.Lessonend
                                        }).ToListAsync();

                messageLogger.AddMessage("", teachinglesson, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
    }
}
