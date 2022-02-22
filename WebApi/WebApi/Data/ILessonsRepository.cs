using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Data
{
    public interface ILessonsRepository
    {
        Task<MessageLogger> Register(Lessons lessons, string lessonid, string teacherid, string lesson, string descr, string imageurl, string utubeurl, string publicid);
        Task<MessageLogger> GetLessons();
        Task<MessageLogger> GetLesson(string lessonid);
        Task<MessageLogger> GetCustomLesson(string lessonid);
        Task<MessageLogger> GetSubscriptions(string lessonid);
        Task<MessageLogger> GetTeachingProgram(string lessonid);
        Task<MessageLogger> GetTeachersLessons(string teacherid);
        Task<MessageLogger> DeleteLesson(string lessonid);
        Task<MessageLogger> CheckifExist(string lessonid);
        Task<MessageLogger> Update(Lessons lessons, string lessonid, string teacherid, string lesson, string descr, string utubeurl);
        PhotoForCreation AddImageForLesson(PhotoForCreation photoForCreation);
        Task<Lessons> UpdateImage(string id, string imgurl, string pubid);
    }
}
