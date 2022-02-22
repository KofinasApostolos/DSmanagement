using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Classes;
using WebApi.CustomModel;
using WebApi.Dtos;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using static WebApi.Helpers.MessageLogger;
using System.Data;
using System.IO;

namespace WebApi.Data
{
    public class TeachingProgramRepository : ITeachingProgramRepository
    {
        private readonly DanceSchoolContext _context;
        public TeachingProgramRepository(DanceSchoolContext context)
        {
            _context = context;
        }

        public async Task<MessageLogger> DeleteTeachingProgram(string id)
        {
            MessageLogger messageLogger = new MessageLogger();
            var teachingProgram = (object)null;
            try
            {
                teachingProgram = await _context.TeachingProgram.FirstOrDefaultAsync(l => l.Id == Int16.Parse(id));

                if (teachingProgram != null)
                {
                    _context.Remove(teachingProgram);
                    await _context.SaveChangesAsync();
                }

                messageLogger.AddMessage("Teaching program deleted", teachingProgram, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetCustomDays()
        {
            MessageLogger messageLogger = new MessageLogger();
            var days = (object)null;
            try
            {
                DataTable daysdt = MemoryTables.DaysTable();

                days = await (from tp in _context.TeachingProgram
                              join l in _context.Lessons
                              on tp.Lessonid equals l.Lessonid
                              join d in daysdt.AsEnumerable()
                              on tp.Dayofweek equals d.Field<int>("Id")
                              select new
                              {
                                  Id = d.Field<int>("Id").ToString(),
                                  Descr = d.Field<string>("Descr").ToString(),
                                  Lesson = l.Lessonid.ToString()
                              }).OrderBy(x => x.Id).ToListAsync();

                messageLogger.AddMessage("", days, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetTeachingProgram(string id)
        {
            MessageLogger messageLogger = new MessageLogger();
            var teachingProgram = (object)null;
            try
            {
                teachingProgram = await _context.TeachingProgram.FirstOrDefaultAsync(l => l.Id == Int16.Parse(id));
                messageLogger.AddMessage("", teachingProgram, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetTeachingPrograms()
        {
            MessageLogger messageLogger = new MessageLogger();
            var teachingPrograms = (object)null;
            try
            {
                teachingPrograms = await _context.TeachingProgram.ToListAsync();

                messageLogger.AddMessage("", teachingPrograms, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Register(TeachingProgram teachingProgram, string lessonid, short Dayofweek,
                                                        string Lessonstart, string Lessonend, short capacity)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                char[] startarr = Lessonstart.Take(2).ToArray();
                char[] endarr = Lessonend.Take(2).ToArray();

                var start = string.Join("", startarr);
                var end = string.Join("", endarr);

                var fixstart = Int16.Parse(start);
                var fixend = Int16.Parse(end);

                if (fixend <= fixstart || fixend == fixstart || fixstart < 13 || fixend > 23)
                {
                    messageLogger.AddMessage("Invalid hours", null, MessageCode.Error);
                    return messageLogger;
                }
                else
                {
                    teachingProgram.Lessonid = lessonid.Trim();
                    teachingProgram.Dayofweek = Dayofweek;
                    teachingProgram.Lessonstart = Lessonstart.Trim();
                    teachingProgram.Lessonend = Lessonend.Trim();
                    teachingProgram.Capacity = capacity;

                    await _context.TeachingProgram.AddAsync(teachingProgram);
                    await _context.SaveChangesAsync();

                    messageLogger.AddMessage("Teaching program inserted", teachingProgram, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> UserSuggestionRegister(string userid, string lessonid,
                                                                    string Dayofweek, string Lessonstart, string Lessonend)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                TeachingProgramTemp teachingProgramtemp = null;

                char[] startarr = Lessonstart.Take(2).ToArray();
                char[] endarr = Lessonend.Take(2).ToArray();

                var start = string.Join("", startarr);
                var end = string.Join("", endarr);

                var fixstart = Int16.Parse(start);
                var fixend = Int16.Parse(end);


                if (((fixstart >= 13 && fixstart <= 23) && (fixend >= 14 && fixend <= 23)) && (fixstart < fixend))
                {
                    bool exist = false;

                    DataTable daysdt = MemoryTables.DaysTable();

                    var teachingProgramTemp = (from tpt in _context.TeachingProgramTemp
                                               join l in _context.Lessons
                                               on tpt.Lessonid equals l.Lessonid
                                               join d in daysdt.AsEnumerable()
                                               on tpt.Dayofweek equals d.Field<int>("Id")
                                               where tpt.Userid == Int16.Parse(userid)
                                               select new
                                               {
                                                   tpt.Userid,
                                                   tpt.Id,
                                                   l.Lessonid,
                                                   Dayofweek = d.Field<int>("Id"),
                                                   tpt.Lessonstart,
                                                   tpt.Lessonend
                                               }).GroupBy(x => x.Lessonid).Select(x => x.ToList());

                    foreach (var itm in teachingProgramTemp.ToList())
                    {
                        if (itm.First().Lessonid == lessonid.Trim() &&
                            itm.First().Dayofweek == Int16.Parse(Dayofweek.Trim()) &&
                            itm.First().Lessonstart == Lessonstart.Trim() &&
                            itm.Last().Lessonend == Lessonend.Trim() &&
                            itm.First().Userid == Int16.Parse(userid))
                            exist = true;
                    }

                    if (exist == true)
                    {
                        messageLogger.AddMessage("Suggestion already exist", null, MessageCode.Error);
                        return messageLogger;
                    }
                    else
                    {
                        var users = (from tpt in _context.TeachingProgramTemp
                                     where tpt.Userid == Int16.Parse(userid)
                                     select tpt.Count).ToList();

                        int userCount = 0;

                        if (users.Count != 0)
                            userCount = users.Max();
                        else
                            userCount = 0;

                        if (userCount < 3)
                        {
                            var diff = fixend - fixstart;
                            var z = diff;
                            for (int i = 0; i < diff; i++)
                            {
                                z--;
                                var resEnd = fixend - z;
                                var res1 = fixstart + i;
                                string[] starr = Lessonstart.Split(":");
                                var fixxxxxst = res1 + ":" + starr[1];
                                string[] enn = Lessonend.Split(":");
                                var fixxxxxen = resEnd + ":" + enn[1];
                                var test = fixxxxxst + " - " + fixxxxxen;

                                teachingProgramtemp = new TeachingProgramTemp
                                {
                                    Lessonid = lessonid,
                                    Userid = Int16.Parse(userid),
                                    Dayofweek = Int16.Parse(Dayofweek),
                                    Lessonstart = fixxxxxst.Trim(),
                                    Lessonend = fixxxxxen.Trim(),
                                    Count = userCount + 1
                                };

                                await _context.TeachingProgramTemp.AddAsync(teachingProgramtemp);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    messageLogger.AddMessage("Suggestion inserted", teachingProgramtemp, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    messageLogger.AddMessage("Invalid hours", null, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> RegisterClassroom(ClassRegister classRegister, string capacity, string day,
                                                                        string lessonstart, string lessonend, string lessonid, string userid)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                DateTime today, answer;
                answer = DateTime.Now;
                //check if subscription is made first for the specific lesson
                var checkIfSubscriptionExist = await (from us in _context.UserSubscriptions
                                                      join l in _context.Lessons
                                                      on us.Lessonid equals l.Lessonid
                                                      join u in _context.User
                                                      on us.Userid equals u.Id
                                                      where us.Userid == Int16.Parse(userid)
                                                      where us.Lessonid == lessonid
                                                      where us.State == 1
                                                      select new
                                                      {
                                                          l.Lesson,
                                                          Firstname = u.FirstName,
                                                          Lastname = u.LastName,
                                                          u.Email,
                                                          us.Dayid
                                                      }).FirstOrDefaultAsync();

                //if exist subscription for specific lesson
                if (checkIfSubscriptionExist != null)
                {
                    DataTable daysdt = MemoryTables.DaysTable();

                    var getDate = (from d in daysdt.AsEnumerable()
                                   where d.Field<string>("Descr") == day.Trim()
                                   select d.Field<int>("Id")).FirstOrDefault();

                    //check validity of date
                    if (getDate != 0)
                    {
                        today = DateTime.Now;
                        var todayz = today.DayOfWeek.ToString();
                        var ccToday = 0;

                        if (todayz == "Monday")
                            ccToday = 1;
                        else if (todayz == "Tuesday")
                            ccToday = 2;
                        else if (todayz == "Wednesday")
                            ccToday = 3;
                        else if (todayz == "Thursday")
                            ccToday = 4;
                        else if (todayz == "Friday")
                            ccToday = 5;
                        else if (todayz == "Saturday")
                            ccToday = 6;
                        else if (todayz == "Sunday")
                            ccToday = 7;

                        if (ccToday > getDate)
                        {
                            var res = ccToday - getDate;
                            if (res == 1)
                                answer = today.AddDays(6);
                            else if (res == 2)
                                answer = today.AddDays(5);
                            else if (res == 3)
                                answer = today.AddDays(4);
                            else if (res == 4)
                                answer = today.AddDays(3);
                            else if (res == 5)
                                answer = today.AddDays(2);
                            else if (res == 6)
                                answer = today.AddDays(1);
                        }
                        else if (ccToday < getDate)
                        {
                            var res1 = ccToday - getDate;
                            var result = "";

                            if (res1.ToString().Length > 1)
                                result = res1.ToString().Substring(1, 1);
                            else
                                result = res1.ToString();

                            answer = today.AddDays(Int32.Parse(result));
                        }
                        else
                        {
                            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");
                            string s = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone).ToString("HH:mm");

                            char[] st = s.Take(2).ToArray();
                            var foo = String.Join("", st);
                            var start = foo;

                            char[] st1 = lessonstart.Take(2).ToArray();
                            var foo1 = String.Join("", st1);
                            var start1 = foo;

                            if (Int32.Parse(start) <= Int32.Parse(start1))
                                answer = DateTime.Now;
                            else
                            {
                                messageLogger.AddMessage("Already registered for " + answer.ToShortDateString(), null, MessageCode.Error);
                                return messageLogger;
                            }
                        }

                        var arr = checkIfSubscriptionExist.Dayid.ToArray();
                        bool existDay = false;

                        foreach (var i in arr)
                        {
                            if (getDate.ToString() == i.ToString())
                            {
                                existDay = true;
                                break;
                            }
                        }

                        if (existDay == true)
                        {
                            //get correct id of teaching program (classroom) user want to register 
                            var dbTeachingProgram = await (from tp in _context.TeachingProgram
                                                           where tp.Dayofweek == getDate
                                                           where tp.Lessonend == lessonend.Trim()
                                                           where tp.Lessonstart == lessonstart.Trim()
                                                           //where tp.Capacity == Int16.Parse(capacity)
                                                           where tp.Lessonid == lessonid.Trim()
                                                           select tp).FirstOrDefaultAsync();

                            //if it is correct and exist
                            if (dbTeachingProgram != null)
                            {
                                //check if user is already registered in classroom
                                var checkIfRegisteredAlready = await (from tr in _context.TempRegisters
                                                                      where tr.Userid == Int32.Parse(userid)
                                                                      where tr.Teachingprogramid == dbTeachingProgram.Id
                                                                      select tr).LastOrDefaultAsync();
                                //if he is not registered then cont
                                if (checkIfRegisteredAlready == null)
                                {
                                    //register user to classroom
                                    TempRegisters tempRegisters1 = new TempRegisters
                                    {
                                        Teachingprogramid = dbTeachingProgram.Id,
                                        Userid = Int32.Parse(userid),
                                        Lessonid = dbTeachingProgram.Lessonid,
                                        Date = answer
                                    };

                                    await _context.TempRegisters.AddAsync(tempRegisters1);
                                    await _context.SaveChangesAsync();

                                    await Task.Run(() =>
                                    {
                                        MailInform.SendMailRegisterClass(checkIfSubscriptionExist.Email, checkIfSubscriptionExist.Firstname,
                                                         checkIfSubscriptionExist.Lastname, checkIfSubscriptionExist.Lesson,
                                                         day, lessonstart, lessonend, answer);
                                    });

                                    messageLogger.AddMessage("Class register successful for " + answer.ToShortDateString(), dbTeachingProgram, MessageCode.Information);
                                    return messageLogger;
                                }
                                else
                                {
                                    if (answer.Date > checkIfRegisteredAlready.Date.Value)
                                    {
                                        //register user to classroom
                                        TempRegisters tempRegisters1 = new TempRegisters
                                        {
                                            Teachingprogramid = dbTeachingProgram.Id,
                                            Userid = Int32.Parse(userid),
                                            Lessonid = dbTeachingProgram.Lessonid,
                                            Date = answer
                                        };

                                        await _context.TempRegisters.AddAsync(tempRegisters1);
                                        await _context.SaveChangesAsync();

                                        await Task.Run(() =>
                                        {
                                            MailInform.SendMailRegisterClass(checkIfSubscriptionExist.Email, checkIfSubscriptionExist.Firstname,
                                                                        checkIfSubscriptionExist.Lastname, checkIfSubscriptionExist.Lesson,
                                                                        day, lessonstart, lessonend, answer);
                                        });

                                        messageLogger.AddMessage("Class register successful for " + answer.ToShortDateString(), dbTeachingProgram, MessageCode.Information);
                                        return messageLogger;
                                    }
                                    else if (answer.Date == checkIfRegisteredAlready.Date.Value)
                                    {
                                        _context.Remove(checkIfRegisteredAlready);
                                        await _context.SaveChangesAsync();

                                        //register user to classroom
                                        TempRegisters tempRegisters1 = new TempRegisters
                                        {
                                            Teachingprogramid = dbTeachingProgram.Id,
                                            Userid = Int32.Parse(userid),
                                            Lessonid = dbTeachingProgram.Lessonid,
                                            Date = answer
                                        };

                                        await _context.TempRegisters.AddAsync(tempRegisters1);
                                        await _context.SaveChangesAsync();

                                        await Task.Run(() =>
                                        {
                                            MailInform.SendMailRegisterClass(checkIfSubscriptionExist.Email, checkIfSubscriptionExist.Firstname,
                                                     checkIfSubscriptionExist.Lastname, checkIfSubscriptionExist.Lesson,
                                                     day, lessonstart, lessonend, answer);
                                        });


                                        messageLogger.AddMessage("Class register successful for " + answer.ToShortDateString(), dbTeachingProgram, MessageCode.Information);
                                        return messageLogger;
                                    }
                                    else
                                    {
                                        messageLogger.AddMessage("Class register successful for " + answer.ToShortDateString(), null, MessageCode.Error);
                                        return messageLogger;
                                    }
                                }
                            }
                            else
                            {
                                messageLogger.AddMessage("Teaching program not exist", null, MessageCode.Error);
                                return messageLogger;
                            }
                        }
                        else
                        {
                            messageLogger.AddMessage("Day does not exist", null, MessageCode.Error);
                            return messageLogger;
                        }
                    }
                    else
                    {
                        messageLogger.AddMessage("Day does not exist", null, MessageCode.Error);
                        return messageLogger;
                    }
                }
                else
                {
                    messageLogger.AddMessage("Already registered for " + answer.ToShortDateString(), null, MessageCode.Error);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> CheckIfExist(string lessonid, short Dayofweek, string Lessonstart, string Lessonend)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var lessonexist = await (from tp in _context.TeachingProgram
                                         where tp.Lessonid == lessonid.Trim()
                                         where tp.Dayofweek == Dayofweek
                                         where tp.Lessonstart == Lessonstart.Trim()
                                         where tp.Lessonend == Lessonend.Trim()
                                         select tp).FirstOrDefaultAsync();

                if (lessonexist != null)
                {
                    messageLogger.AddMessage("Teaching program already exist", lessonexist, MessageCode.Error);
                    return messageLogger;
                }
                else
                {
                    messageLogger.AddMessage("", null, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        private bool TimeValidation()
        {


            return true;
        }

        public async Task<MessageLogger> Update(TeachingProgram teachingProgram, string id, string lessonid, short Dayofweek,
                                                     string Lessonstart, string Lessonend, short capacity)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                char[] startarr = Lessonstart.Take(2).ToArray();
                char[] endarr = Lessonend.Take(2).ToArray();

                var start = string.Join("", startarr);
                var end = string.Join("", endarr);

                var fixstart = Int16.Parse(start);
                var fixend = Int16.Parse(end);

                if (fixend <= fixstart || fixend == fixstart || fixstart < 13 || fixend > 23)
                {
                    messageLogger.AddMessage("Invalid hours", null, MessageCode.Error);
                    return messageLogger;
                }
                else
                {
                    teachingProgram = await _context.TeachingProgram.FirstOrDefaultAsync(tp => tp.Id == Int16.Parse(id));

                    if (teachingProgram != null)
                    {
                        teachingProgram.Lessonid = lessonid.Trim();
                        teachingProgram.Dayofweek = Dayofweek;
                        teachingProgram.Lessonstart = Lessonstart.Trim();
                        teachingProgram.Lessonend = Lessonend.Trim();
                        teachingProgram.Capacity = capacity;

                        await _context.SaveChangesAsync();
                    }

                    var dt = DateTime.Now;
                    var checkRegistrations = await (from tr in _context.TempRegisters
                                                    join u in _context.User
                                                    on tr.Userid equals u.Id
                                                    join l in _context.Lessons
                                                    on tr.Lessonid equals l.Lessonid
                                                    where tr.Teachingprogramid == teachingProgram.Id
                                                    where tr.Date.Value > dt.Date
                                                    select new
                                                    {
                                                        u.Email,
                                                        u.Username,
                                                        Firstname = u.FirstName,
                                                        Lastname = u.LastName,
                                                        l.Lesson
                                                    }).ToListAsync();

                    if (checkRegistrations.Count != 0)
                    {
                        foreach (var itm in checkRegistrations.ToList())
                            MailInform.SendMailUpdateTeachingProgram(itm.Email, itm.Firstname, itm.Lastname,
                                                                            itm.Lesson, Dayofweek.ToString(), Lessonstart, Lessonend);
                    }
                    messageLogger.AddMessage("Teaching program updated", teachingProgram, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetSuggestionsUser(string id)
        {
            return await Task.Run(() =>
              {
                  MessageLogger messageLogger = new MessageLogger();
                  try
                  {
                      List<CustomSuggestions> customSuggestions = new List<CustomSuggestions>();

                      for (var i = 1; i < 4; i++)
                      {
                          DataTable daysdt = MemoryTables.DaysTable();

                          var teachingProgramTemp = (from tpt in _context.TeachingProgramTemp
                                                     join l in _context.Lessons
                                                     on tpt.Lessonid equals l.Lessonid
                                                     join d in daysdt.AsEnumerable()
                                                     on tpt.Dayofweek equals d.Field<int>("Id")
                                                     where tpt.Userid == Int16.Parse(id)
                                                     where tpt.Count == i
                                                     select new
                                                     {
                                                         tpt.Id,
                                                         l.Lessonid,
                                                         Dayofweek = d.Field<int>("Id"),
                                                         tpt.Lessonstart,
                                                         tpt.Lessonend,
                                                         tpt.Count
                                                     }).GroupBy(x => new { x.Lessonid, x.Dayofweek }).Select(x => x.ToList());

                          if (teachingProgramTemp.Count() != 0)
                              foreach (var itm in teachingProgramTemp.ToList())
                                  customSuggestions.Add(new CustomSuggestions(itm.First().Id.ToString(), itm.First().Lessonid.ToString(),
                                                                              (Int16)itm.First().Dayofweek, itm.First().Lessonstart, itm.Last().Lessonend, itm.First().Count.ToString()));
                      }

                      if (customSuggestions == null)
                      {
                          messageLogger.AddMessage("There are no suggestions", null, MessageCode.Error);
                          return messageLogger;
                      }
                      else
                      {
                          messageLogger.AddMessage("", customSuggestions, MessageCode.Information);
                          return messageLogger;
                      }
                  }
                  catch (Exception ex)
                  {
                      messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                      return messageLogger;
                  }
              });
        }

        public async Task<MessageLogger> GetSuggestionsAdmin()
        {
            return await Task.Run(() =>
             {
                 MessageLogger messageLogger = new MessageLogger();
                 try
                 {
                     //get all elements including master and detail
                     var q = (from tpt in _context.TeachingProgramTemp
                              join u in _context.User
                              on tpt.Userid equals u.Id
                              select new
                              {
                                  // detail fields
                                  fullname = u.FirstName + " " + u.LastName,
                                  email = u.Email,
                                  phone = u.Phonenumber,
                                  userid = u.Id,
                                  //Master fields from here
                                  lessonid = tpt.Lessonid,
                                  dayofweek = tpt.Dayofweek,
                                  lessonstart = tpt.Lessonstart,
                                  lessonend = tpt.Lessonend
                              });

                     q = q.Distinct().Where(lesson => lesson.userid == lesson.userid &&
                                                     lesson.lessonid == lesson.lessonid &&
                                                     lesson.dayofweek == lesson.dayofweek &&
                                                     lesson.lessonstart == lesson.lessonstart &&
                                                     lesson.lessonend == lesson.lessonend);

                     //group by lessonid, day, lesson start, lesson end
                     var md = q.ToLookup(ee => new
                     {
                         ee.lessonid,
                         ee.dayofweek,
                         ee.lessonstart,
                         ee.lessonend
                     })
                        .Select(c => new
                        {
                            //lesson = c.Key, //get master
                            c.Key.lessonid,
                            c.Key.dayofweek,
                            c.Key.lessonstart,
                            c.Key.lessonend,
                            //Count = c.Count(), //get details
                            users = c.Select(ff => new
                            {
                                Email = ff.email,
                                Name = ff.fullname,
                                Phone = ff.phone
                            }).ToList()
                        });

                     messageLogger.AddMessage("", md, MessageCode.Information);
                     return messageLogger;
                 }
                 catch (Exception ex)
                 {
                     messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                     return messageLogger;
                 }
             });
        }

        public async Task<MessageLogger> Τransfer2Core(TeachingProgram teachingProgram, string lessonid,
                                                            string dayofweek, string lessonstart, string lessonend)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var checkTp = await (from tp in _context.TeachingProgram
                                     where tp.Dayofweek == Int16.Parse(dayofweek)
                                     where tp.Lessonstart == lessonstart.Trim()
                                     where tp.Lessonend == lessonend.Trim()
                                     select tp).FirstOrDefaultAsync();

                if (checkTp != null)
                {
                    return null;
                }
                else
                {
                    teachingProgram.Lessonid = lessonid.Trim();
                    teachingProgram.Dayofweek = Int16.Parse(dayofweek.Trim());
                    teachingProgram.Lessonstart = lessonstart.Trim();
                    teachingProgram.Lessonend = lessonend.Trim();

                    await _context.TeachingProgram.AddAsync(teachingProgram);
                    await _context.SaveChangesAsync();

                    messageLogger.AddMessage("Teaching program transfered", teachingProgram, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> DeleteSuggestion(string id)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var teachingProgram = await _context.TeachingProgramTemp.FirstOrDefaultAsync(l => l.Id == Int16.Parse(id));

                if (teachingProgram != null)
                {
                    _context.Remove(teachingProgram);
                    await _context.SaveChangesAsync();
                }

                messageLogger.AddMessage("Teaching program deleted", teachingProgram, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> DeleteTeachingProgramSuggestion(string id, string lessonid, string day,
                                                                    string lessonstart, string lessonend, string count, string userid)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var teachingProgram = await (from tpt in _context.TeachingProgramTemp
                                             where tpt.Lessonid == lessonid
                                             where tpt.Userid == Int16.Parse(userid)
                                             where tpt.Count == Int16.Parse(count)
                                             where tpt.Dayofweek == Int16.Parse(day.Trim())
                                             select tpt).ToListAsync();

                if (teachingProgram != null)
                {
                    foreach (var itm in teachingProgram.ToList())
                        _context.Remove(itm);

                    await _context.SaveChangesAsync();
                }

                var teachingProgram1 = (from tpt in _context.TeachingProgramTemp
                                        where tpt.Userid == Int16.Parse(userid)
                                        select tpt).GroupBy(x => new { x.Lessonid, x.Dayofweek, x.Count }).Select(x => x.ToList());


                var foo = teachingProgram1.Count();
                if (foo != 0)
                {
                    var cc = 0;
                    foreach (var group in teachingProgram1.ToList())
                    {
                        cc++;
                        foreach (var groupedItem in group.ToList())
                        {
                            var updrecord = await (from v in _context.TeachingProgramTemp
                                                   where v.Lessonid == groupedItem.Lessonid
                                                   where v.Dayofweek == groupedItem.Dayofweek
                                                   where v.Lessonstart == groupedItem.Lessonstart
                                                   where v.Lessonend == groupedItem.Lessonend
                                                   where v.Count == groupedItem.Count
                                                   select v).FirstOrDefaultAsync();

                            if (updrecord != null)
                            {
                                updrecord.Count = cc;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                messageLogger.AddMessage("Suggestion deleted", teachingProgram, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> UpdateUserSuggestion(TeachingProgramTemp teachingProgramTemp, string userid, string id, string lessonid,
                                                                 string Dayofweek, string Lessonstart, string Lessonend)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var dbtpt = await (from tpt in _context.TeachingProgramTemp
                                   where tpt.Userid == Int16.Parse(userid)
                                   where tpt.Id == Int32.Parse(id)
                                   select tpt).FirstOrDefaultAsync();

                if (dbtpt != null)
                {
                    var allRecords = await (from tpt in _context.TeachingProgramTemp
                                            where tpt.Count == dbtpt.Count
                                            where tpt.Userid == Int32.Parse(userid)
                                            select tpt).ToListAsync();

                    TeachingProgramTemp teachingProgramtemp = null;

                    char[] startarr = Lessonstart.Take(2).ToArray();
                    char[] endarr = Lessonend.Take(2).ToArray();

                    var start = string.Join("", startarr);
                    var end = string.Join("", endarr);

                    var fixstart = Int16.Parse(start);
                    var fixend = Int16.Parse(end);

                    bool exist = false;

                    DataTable daysdt = MemoryTables.DaysTable();

                    var teachingProgramTemp1 = (from tpt in _context.TeachingProgramTemp
                                                join l in _context.Lessons
                                                on tpt.Lessonid equals l.Lessonid
                                                join d in daysdt.AsEnumerable()
                                                on tpt.Dayofweek equals d.Field<int>("Id")
                                                where tpt.Userid == Int16.Parse(userid)
                                                select new
                                                {
                                                    tpt.Userid,
                                                    tpt.Id,
                                                    l.Lessonid,
                                                    Dayofweek = d.Field<int>("Id"),
                                                    tpt.Lessonstart,
                                                    tpt.Lessonend
                                                }).GroupBy(x => x.Lessonid).Select(x => x.ToList());


                    foreach (var itm in teachingProgramTemp1.ToList())
                    {
                        if (itm.First().Lessonid == lessonid.Trim() &&
                            itm.First().Dayofweek == Int16.Parse(Dayofweek.Trim()) &&
                            itm.First().Lessonstart == Lessonstart.Trim() &&
                            itm.Last().Lessonend == Lessonend.Trim() &&
                            itm.First().Userid == Int16.Parse(userid))
                            exist = true;
                    }

                    if (fixend <= fixstart || fixend == fixstart || fixstart < 13 || fixend > 23 || exist == true)
                    {
                        messageLogger.AddMessage("Invalid hours", null, MessageCode.Error);
                        return messageLogger;
                    }
                    else
                    {
                        if (allRecords.Count != 0)
                        {
                            foreach (var itm in allRecords.ToList())
                                _context.Remove(itm);

                            await _context.SaveChangesAsync();
                        }

                        var diff = fixend - fixstart;
                        var z = diff;
                        for (int i = 0; i < diff; i++)
                        {
                            z--;
                            var resEnd = fixend - z;
                            var res1 = fixstart + i;
                            string[] starr = Lessonstart.Split(":");
                            var fixxxxxst = res1 + ":" + starr[1];
                            string[] enn = Lessonend.Split(":");
                            var fixxxxxen = resEnd + ":" + enn[1];
                            var test = fixxxxxst + " - " + fixxxxxen;

                            teachingProgramtemp = new TeachingProgramTemp
                            {
                                Lessonid = lessonid,
                                Userid = Int16.Parse(userid),
                                Dayofweek = Int16.Parse(Dayofweek),
                                Lessonstart = fixxxxxst.Trim(),
                                Lessonend = fixxxxxen.Trim(),
                                Count = dbtpt.Count
                            };

                            await _context.TeachingProgramTemp.AddAsync(teachingProgramtemp);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                messageLogger.AddMessage("Suggestion updated", dbtpt, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Create_And_WriteXLS_TP(string id, List<CustomTP> customTPs)
        {
            return await Task.Run(() =>
            {
                MessageLogger messageLogger = new MessageLogger();
                try
                {
                    string path = Path.Combine(Environment.CurrentDirectory, @"XlsFiles\" + id);

                    if (System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.Delete(path, true);
                        System.IO.Directory.CreateDirectory(path);
                    }
                    else
                        System.IO.Directory.CreateDirectory(path);

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("teaching_program");
                        var currentRow = 1;

                        #region Header
                        worksheet.Cell(currentRow, 1).Value = "Id";
                        worksheet.Cell(currentRow, 2).Value = "Lesson";
                        worksheet.Cell(currentRow, 3).Value = "Day";
                        worksheet.Cell(currentRow, 4).Value = "Lessonstart";
                        worksheet.Cell(currentRow, 5).Value = "Lessonend";
                        worksheet.Cell(currentRow, 6).Value = "Capacity";
                        #endregion

                        #region Body
                        foreach (var item in customTPs.ToList())
                        {
                            currentRow++;
                            var getLesson = _context.Lessons.FirstOrDefault(x => x.Lessonid == item.Lessonid.Trim());

                            worksheet.Cell(currentRow, 1).Value = item.Id == null ? "" : item.Id.ToString();
                            worksheet.Cell(currentRow, 2).Value = item.Lessonid == null ? "" : getLesson.Lesson.ToString();

                            if (item.Dayofweek == "1")
                                worksheet.Cell(currentRow, 3).Value = item.Dayofweek == null ? "" : "Monday";
                            else if (item.Dayofweek == "2")
                                worksheet.Cell(currentRow, 3).Value = item.Dayofweek == null ? "" : "Tuesday";
                            else if (item.Dayofweek == "3")
                                worksheet.Cell(currentRow, 3).Value = item.Dayofweek == null ? "" : "Wednesday";
                            else if (item.Dayofweek == "4")
                                worksheet.Cell(currentRow, 3).Value = item.Dayofweek == null ? "" : "Thursday";
                            else if (item.Dayofweek == "5")
                                worksheet.Cell(currentRow, 3).Value = item.Dayofweek == null ? "" : "Friday";
                            else if (item.Dayofweek == "6")
                                worksheet.Cell(currentRow, 3).Value = item.Dayofweek == null ? "" : "Saturday";
                            else if (item.Dayofweek == "7")
                                worksheet.Cell(currentRow, 3).Value = item.Dayofweek == null ? "" : "Sunday";

                            worksheet.Cell(currentRow, 4).Value = item.Lessonstart == null ? "" : item.Lessonstart.ToString();
                            worksheet.Cell(currentRow, 5).Value = item.Lessonend == null ? "" : item.Lessonend.ToString();
                            worksheet.Cell(currentRow, 6).Value = item.Capacity == null ? "" : item.Capacity.ToString();
                        }
                        #endregion
                        workbook.SaveAs(path + @"\teaching_program.xlsx");
                        Thread.Sleep(5000);
                        var content = System.IO.File.ReadAllBytes(path + @"\teaching_program.xlsx");

                        if (System.IO.Directory.Exists(path))
                            System.IO.Directory.Delete(path, true);

                        messageLogger.AddMessage("xls has been created", System.Convert.ToBase64String(content), MessageCode.Information);
                        return messageLogger;
                    }
                }
                catch (Exception ex)
                {
                    messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                    return messageLogger;
                }
            });
        }
    }
}
