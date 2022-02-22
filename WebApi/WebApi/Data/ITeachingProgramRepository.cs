using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.CustomModel;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Data
{
    public interface ITeachingProgramRepository
    {
        Task<MessageLogger> UpdateUserSuggestion(TeachingProgramTemp teachingProgramTemp, string userid, string id, string lessonid, string Dayofweek,
                                                    string Lessonstart, string Lessonend);
        Task<MessageLogger> CheckIfExist(string lessonid, short Dayofweek, string Lessonstart, string Lessonend);
        Task<MessageLogger> UserSuggestionRegister(string userid, string lessonid, string Dayofweek,
                                                            string Lessonstart, string Lessonend);
        Task<MessageLogger> Register(TeachingProgram teachingProgram, string lessonid, short Dayofweek, string Lessonstart, string Lessonend, short capacity);
        Task<MessageLogger> Τransfer2Core(TeachingProgram teachingProgram, string lessonid, string dayofweek,
                                                                   string lessonstart, string lessonend);
        Task<MessageLogger> GetTeachingPrograms();
        //Task<MessageLogger> GetDays();
        Task<MessageLogger> DeleteTeachingProgramSuggestion(string id, string lessonid, string day,
                                                                    string lessonstart, string lessonend, string count, string userid);
        Task<MessageLogger> RegisterClassroom(ClassRegister classRegister, string userid, string capacity, string day,
                                                                        string lessonstart, string lessonend, string lessonid);
        Task<MessageLogger> GetTeachingProgram(string id);
        Task<MessageLogger> DeleteSuggestion(string id);
        Task<MessageLogger> GetCustomDays();
        Task<MessageLogger> GetSuggestionsUser(string id);
        Task<MessageLogger> GetSuggestionsAdmin();
        Task<MessageLogger> DeleteTeachingProgram(string id);
        Task<MessageLogger> Create_And_WriteXLS_TP(string id, List<CustomTP> customUsers);
        Task<MessageLogger> Update(TeachingProgram teachingProgram, string id, string lessonid, short Dayofweek, string Lessonstart, string Lessonend, short capacity);
    }
}
