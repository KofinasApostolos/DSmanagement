using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Classes;
using WebApi.CustomModel;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;
using WebApi.Helpers;
using static WebApi.Helpers.MessageLogger;
using System.Data;

namespace WebApi.Data
{
    public class UsersubscriptionRepository : IUsersubscriptionRepository
    {

        private readonly DanceSchoolContext _context;
        public UsersubscriptionRepository(DanceSchoolContext context)
        {
            _context = context;
        }

        public async Task<MessageLogger> DeleteUsersubscription(string id)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var usersubscription = await _context.UserSubscriptions.FirstOrDefaultAsync(us => us.Id == Int16.Parse(id));

                if (usersubscription != null)
                {
                    _context.Remove(usersubscription);
                    await _context.SaveChangesAsync();
                }

                var tempRegisters = (from v in _context.TempRegisters
                                     where v.Userid == usersubscription.Userid
                                     select v).ToList();

                if (tempRegisters.Count != 0)
                {
                    foreach (var itm in tempRegisters.ToList())
                        _context.Remove(itm);

                    await _context.SaveChangesAsync();
                }

                messageLogger.AddMessage("Subscription deleted", usersubscription, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Create_And_WriteXLS_UsersSubscriptions(string id, List<CustomUserSubscriptions> customUserSubscriptions)
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
                        var worksheet = workbook.Worksheets.Add("userssubs");
                        var currentRow = 1;

                        #region Header
                        worksheet.Cell(currentRow, 1).Value = "Id";
                        worksheet.Cell(currentRow, 2).Value = "Username";
                        worksheet.Cell(currentRow, 3).Value = "Lesson";
                        worksheet.Cell(currentRow, 4).Value = "Duration";
                        worksheet.Cell(currentRow, 5).Value = "Price";
                        worksheet.Cell(currentRow, 6).Value = "Day";
                        worksheet.Cell(currentRow, 7).Value = "Date";
                        worksheet.Cell(currentRow, 8).Value = "ExpDate";
                        worksheet.Cell(currentRow, 9).Value = "Discount";
                        worksheet.Cell(currentRow, 10).Value = "State";
                        #endregion

                        #region Body
                        foreach (var item in customUserSubscriptions.ToList())
                        {
                            var username = _context.User.FirstOrDefault(x => x.Id == Int32.Parse(item.Userid));
                            var lesson = _context.Lessons.FirstOrDefault(l => l.Lessonid == item.Lessonid);

                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = item.Id == null ? "" : item.Id.ToString();

                            if (username != null)
                                worksheet.Cell(currentRow, 2).Value = item.Userid == null ? "" : username.Username.ToString();
                            else
                                worksheet.Cell(currentRow, 2).Value = item.Userid == null ? "" : "";

                            if (lesson != null)
                                worksheet.Cell(currentRow, 3).Value = item.Lessonid == null ? "" : lesson.Lesson.ToString();
                            else
                                worksheet.Cell(currentRow, 3).Value = item.Lessonid == null ? "" : "";

                            worksheet.Cell(currentRow, 4).Value = item.Duration == null ? "" : item.Duration.ToString();
                            worksheet.Cell(currentRow, 5).Value = item.Price == null ? "" : item.Price.ToString();

                            if (item.Day == null)
                                worksheet.Cell(currentRow, 6).Value = "";
                            else
                            {
                                string dd = "";
                                dd = DayConvertions.GetDayFromInt(dd, item);
                                worksheet.Cell(currentRow, 6).Value = dd;
                            }

                            worksheet.Cell(currentRow, 7).Value = item.Date == null ? "" : item.Date.ToString();
                            worksheet.Cell(currentRow, 8).Value = item.ExpDate == null ? "" : item.ExpDate.ToString();

                            if (item.Discount == null)
                                worksheet.Cell(currentRow, 9).Value = "";
                            else if (item.Discount == "1")
                                worksheet.Cell(currentRow, 9).Value = "Yes";
                            else if (item.Discount == "0")
                                worksheet.Cell(currentRow, 9).Value = "No";

                            if (item.State == null)
                                worksheet.Cell(currentRow, 10).Value = "";
                            else if (item.State == "1")
                                worksheet.Cell(currentRow, 10).Value = "Activated";
                            else if (item.State == "2")
                                worksheet.Cell(currentRow, 10).Value = "Deactivated";
                        }
                        #endregion

                        workbook.SaveAs(path + @"\userssubs.xlsx");
                        Thread.Sleep(5000);
                        var content = System.IO.File.ReadAllBytes(path + @"\userssubs.xlsx");

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

        public async Task<MessageLogger> GetUsersubscriptions()
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var userSubscriptions = (object)null;

                userSubscriptions = await (from us in _context.UserSubscriptions
                                           select new
                                           {
                                               us.Id,
                                               us.Userid,
                                               us.Lessonid,
                                               us.Duration,
                                               us.Price,
                                               Date = us.Date == null ? null : us.Date.ToShortDateString(),
                                               ExpDate = us.Date.AddMonths(us.Duration) == null ? null : us.Date.AddMonths(us.Duration).ToShortDateString(),
                                               us.State,
                                               Discount = us.Discount.Value == true ? 1 : 0,
                                               Day = us.Dayid.ToArray()
                                           }).ToListAsync();

                messageLogger.AddMessage("", userSubscriptions, MessageCode.Information);
                return messageLogger;

            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetUsersubscription(string id)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var userSubscription = await _context.UserSubscriptions.FirstOrDefaultAsync(us => us.Id == Int16.Parse(id));
                messageLogger.AddMessage("", userSubscription, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Register(UserSubscriptions userSubscriptions, string id, string lessonid, string[] day,
                                                        string duration, string price, string date, bool discount)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                List<string> list1 = new List<string>();
                List<DaysCapacity> list2 = new List<DaysCapacity>();

                DataTable daysdt = MemoryTables.DaysTable();

                var tp = await (from tp1 in _context.TeachingProgram
                                join d in daysdt.AsEnumerable()
                                on tp1.Dayofweek equals d.Field<int>("Id")
                                where tp1.Lessonid == lessonid.Trim()
                                select new
                                {
                                    day = d.Field<int>("Id"),
                                    capacity = tp1.Capacity,
                                    hours = tp1.Lessonstart + " " + tp1.Lessonend
                                }).ToListAsync();

                var getusubs = await (from us in _context.UserSubscriptions
                                      where us.State == 1
                                      where us.Lessonid == lessonid.Trim()
                                      select us).ToListAsync();

                if (getusubs.Count != 0)
                {
                    foreach (var itm in getusubs.ToList())
                    {
                        var temparr = itm.Dayid.ToCharArray();
                        for (var i = 0; i < temparr.Length; i++)
                            list1.Add(temparr[i].ToString());
                    }

                    var counts = DayConvertions.GetCountOfEachDays(list1);

                    list2 = DayConvertions.UnionDatesAndCounts(tp, counts);

                    bool allSame = list2.All(item => item.Count == "0");

                    if (allSame == true)
                    {
                        messageLogger.AddMessage("Subscription already exist", null, MessageCode.Error);
                        return messageLogger;
                    }
                    else
                    {
                        string dd = "";
                        for (int i = 0; i < day.Length; i++)
                        {
                            var gg = (from v in list2.ToList()
                                      where v.Dayid == dd
                                      select v.Count).FirstOrDefault();

                            if (gg == "0")
                            {
                                messageLogger.AddMessage("This day doesnt not exist for subscription", null, MessageCode.Error);
                                return messageLogger;
                            }
                            dd = "";
                        }
                    }
                }
                var price1 = price.TrimEnd('€');
                //check if subscription is legit from subscriptions table
                var subscriptions = await (from s in _context.Subscriptions
                                           where s.Duration == duration.Trim()
                                           where s.Price == price1.Trim()
                                           where s.Lessonid == lessonid.Trim()
                                           where s.Discount == discount
                                           select s).FirstOrDefaultAsync();

                //if subscription exists then cont, else return null cause user changed values like duration and price or lesson from client side
                if (subscriptions != null)
                {
                    //check if samesubscription for the same lesson and same duration has been made
                    var userSubscription = await (from us in _context.UserSubscriptions
                                                  join u in _context.User
                                                  on us.Userid equals u.Id
                                                  join l in _context.Lessons
                                                  on us.Lessonid equals l.Lessonid
                                                  where us.Lessonid == lessonid.Trim()
                                                  where us.Userid == Int16.Parse(id)
                                                  where us.Duration == Int16.Parse(duration)
                                                  where us.State == 1
                                                  //where us.Price == Price.Trim()
                                                  //where us.Discount == discount
                                                  select new
                                                  {
                                                      us.Id,
                                                      us.Userid,
                                                      us.Lessonid,
                                                      us.Duration,
                                                      us.Price,
                                                      us.Date,
                                                      us.State,
                                                      us.Discount,
                                                      Dayid = us.Dayid.ToArray(),
                                                      u.Username,
                                                      Firstname = u.FirstName,
                                                      Lastname = u.LastName,
                                                      u.Email,
                                                      l.Lesson
                                                  }).FirstOrDefaultAsync();

                    //if same subscription exist then  
                    if (userSubscription != null)
                    {
                        //check if subscription has not finished yet.
                        DateTime localDate = DateTime.Now;
                        var months = userSubscription.Duration;
                        var dt = userSubscription.Date.AddMonths(months);
                        int result = DateTime.Compare(dt, localDate);
                        bool expired;

                        if (result < 0) //subscription has been expired
                            expired = true;
                        else //1 subscription has not been expired
                            expired = false;

                        //if has not finished yet then cannot subscribe again!
                        if (expired == false)
                        {
                            messageLogger.AddMessage("Subscription has not finished yet", null, MessageCode.Error);
                            return messageLogger;
                        }
                        else
                        {
                            //update status to deactivated
                            userSubscriptions.State = 2;
                            await _context.SaveChangesAsync();

                            //insert new subscription row
                            return await InsertSubscription(userSubscriptions, id, lessonid.Trim(), duration, price1, day, discount, DateTime.Parse(date), _context,
                            userSubscription.Firstname, userSubscription.Lastname, userSubscription.Email, userSubscription.Lesson, tp);
                        }
                    }
                    else
                    {
                        var user = await (from u in _context.User
                                          where u.Id == Int32.Parse(id)
                                          select new
                                          {
                                              Userid = u.Id,
                                              u.Username,
                                              Firstname = u.FirstName,
                                              Lastname = u.LastName,
                                              u.Email,
                                          }).FirstOrDefaultAsync();

                        if (user != null)
                        {
                            var lesson = await (from l in _context.Lessons
                                                where l.Lessonid == lessonid.Trim()
                                                select l.Lesson).FirstOrDefaultAsync();

                            if (lesson != null)
                                //insert new subscription row
                                return await InsertSubscription(userSubscriptions, id, lessonid.Trim(), duration, price1, day, discount, DateTime.Parse(date), _context,
                                user.Firstname, user.Lastname, user.Email, lesson, tp);
                            else
                            {
                                messageLogger.AddMessage("Lesson doesnt exist", null, MessageCode.Error);
                                return messageLogger;
                            }
                        }
                        else
                        {
                            messageLogger.AddMessage("User not exist", null, MessageCode.Error);
                            return messageLogger;
                        }
                    }
                }
                else
                {
                    messageLogger.AddMessage("Subscription not exist", null, MessageCode.Error);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> InsertSubscription(UserSubscriptions userSubscriptions, string userid, string lessonid,
                                                               string duration, string price, string[] day, bool discount,
                                                               DateTime date, DanceSchoolContext _context, string firstname,
                                                               string lastname, string email, string lesson, IEnumerable<dynamic> list)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                List<string> arr = new List<string>();
                string[] cArray;
                string dd = "";

                dd = DayConvertions.GetDayFromInt(dd, day);

                userSubscriptions.Userid = Int32.Parse(userid);
                userSubscriptions.Lessonid = lessonid.Trim();
                userSubscriptions.Duration = Int16.Parse(duration);
                userSubscriptions.Price = price.Trim();
                userSubscriptions.Date = date;
                userSubscriptions.Dayid = string.Join("", day);
                userSubscriptions.State = 1;
                userSubscriptions.Discount = discount;

                await _context.UserSubscriptions.AddAsync(userSubscriptions);
                await _context.SaveChangesAsync();

                cArray = dd.Split(",");

                await Task.Run(() => { MailInform.SendMailRegisterSubscription(duration, price, cArray, discount, date, firstname, lastname, email, lesson, arr); });

                messageLogger.AddMessage("Subscription inserted", userSubscriptions, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Update(UserSubscriptions userSubscriptions, string id, string userid, string lessonid, string[] day,
                                                        string duration, string price, string date, bool discount, short state)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                userSubscriptions = await _context.UserSubscriptions.FirstOrDefaultAsync(us => us.Id == Int16.Parse(id));

                if (userSubscriptions != null)
                {
                    List<string> list1 = new List<string>();
                    List<DaysCapacity> list2 = new List<DaysCapacity>();

                    DataTable daysdt = MemoryTables.DaysTable();

                    var tp = await (from tp1 in _context.TeachingProgram
                                    join d in daysdt.AsEnumerable()
                                    on tp1.Dayofweek equals d.Field<int>("Id")
                                    where tp1.Lessonid == lessonid.Trim()
                                    select new
                                    {
                                        day = d.Field<int>("Id"),
                                        capacity = tp1.Capacity,
                                        hours = tp1.Lessonstart + " " + tp1.Lessonend
                                    }).ToListAsync();


                    var getusubs = await (from us in _context.UserSubscriptions
                                          where us.State == 1
                                          where us.Lessonid == lessonid.Trim()
                                          select us).ToListAsync();

                    if (getusubs.Count != 0)
                    {
                        foreach (var itm in getusubs.ToList())
                        {
                            var temparr = itm.Dayid.ToCharArray();
                            for (var i = 0; i < temparr.Length; i++)
                                list1.Add(temparr[i].ToString());
                        }


                        var counts = DayConvertions.GetCountOfEachDays(list1);

                        list2 = DayConvertions.UnionDatesAndCounts(tp, counts);

                        bool allSame = list2.All(item => item.Count == "0");

                        if (allSame == true)
                        {
                            messageLogger.AddMessage("Subscription already exist", null, MessageCode.Error);
                            return messageLogger;
                        }
                        else
                        {
                            string dd = "";

                            var dayz = DayConvertions.GetIntFromDateAndCheckFromList(day, dd, list2);

                            if (dayz)
                            {
                                messageLogger.AddMessage("Day doesnt exist", null, MessageCode.Error);
                                return messageLogger;
                            }
                        }
                    }

                    //check if subscription is legit from subscriptions table
                    var subscriptions = await (from s in _context.Subscriptions
                                               where s.Duration == duration.Trim()
                                               where s.Price == price.Trim()
                                               where s.Lessonid == lessonid.Trim()
                                               where s.Discount == discount
                                               select s).FirstOrDefaultAsync();

                    //if subscription exists then cont, else return null cause user changed values like duration and price or lesson from client side
                    if (subscriptions != null)
                    {
                        //check if samesubscription for the same lesson and same duration has been made
                        var userSubscription = await (from us in _context.UserSubscriptions
                                                      join u in _context.User
                                                      on us.Userid equals u.Id
                                                      join l in _context.Lessons
                                                      on us.Lessonid equals l.Lessonid
                                                      where us.Lessonid == lessonid.Trim()
                                                      where us.Userid == Int16.Parse(id)
                                                      where us.Duration == Int16.Parse(duration)
                                                      where us.State == 1
                                                      //where us.Price == Price.Trim()
                                                      //where us.Discount == discount
                                                      select new
                                                      {
                                                          us.Id,
                                                          us.Userid,
                                                          us.Lessonid,
                                                          us.Duration,
                                                          us.Price,
                                                          us.Date,
                                                          us.State,
                                                          us.Discount,
                                                          Dayid = us.Dayid.ToArray(),
                                                          u.Username,
                                                          Firstname = u.FirstName,
                                                          Lastname = u.LastName,
                                                          u.Email,
                                                          l.Lesson
                                                      }).FirstOrDefaultAsync();

                        //if same subscription exist then  
                        if (userSubscription != null)
                        {
                            //check if subscription has not finished yet.
                            DateTime localDate = DateTime.Now;
                            var months = userSubscription.Duration;
                            var dt = userSubscription.Date.AddMonths(months);
                            int result = DateTime.Compare(dt, localDate);
                            bool expired;

                            if (result < 0) //subscription has been expired
                                expired = true;
                            else //1 subscription has not been expired
                                expired = false;

                            //if has not finished yet then cannot subscribe again!
                            if (expired == false)
                            {
                                messageLogger.AddMessage("Subscription already exist", null, MessageCode.Error);
                                return messageLogger;
                            }
                            else
                            {
                                //update status to deactivated
                                userSubscriptions.State = 2;
                                await _context.SaveChangesAsync();

                                //insert new subscription row
                                return await InsertSubscription(userSubscriptions, userid, lessonid.Trim(), duration, price, day, discount, DateTime.Parse(date), _context,
                                userSubscription.Firstname, userSubscription.Lastname, userSubscription.Email, userSubscription.Lesson, tp);
                            }
                        }
                        else
                        {
                            var user = await (from u in _context.User
                                              where u.Id == Int32.Parse(userid)
                                              select new
                                              {
                                                  Userid = u.Id,
                                                  u.Username,
                                                  Firstname = u.FirstName,
                                                  Lastname = u.LastName,
                                                  u.Email,
                                              }).FirstOrDefaultAsync();

                            if (user != null)
                            {
                                var lesson = await (from l in _context.Lessons
                                                    where l.Lessonid == lessonid.Trim()
                                                    select l.Lesson).FirstOrDefaultAsync();

                                if (lesson != null)
                                {
                                    //userSubscriptions.Userid = Int32.Parse(userid);
                                    userSubscriptions.Lessonid = lessonid.Trim();
                                    userSubscriptions.Duration = Int16.Parse(duration);
                                    userSubscriptions.Price = price.Trim();
                                    userSubscriptions.Dayid = String.Join("", day);
                                    userSubscriptions.Date = DateTime.Parse(date);
                                    userSubscriptions.Discount = discount;
                                    userSubscriptions.State = state;

                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    messageLogger.AddMessage("Lesson doesnt exist", null, MessageCode.Error);
                                    return messageLogger;
                                }
                            }
                            else
                            {
                                messageLogger.AddMessage("User doesnt exist", null, MessageCode.Error);
                                return messageLogger;
                            }
                        }
                    }
                    else
                    {
                        messageLogger.AddMessage("Subscription doesnt exist", null, MessageCode.Error);
                        return messageLogger;
                    }
                }
                messageLogger.AddMessage("Subscription updated", userSubscriptions, MessageCode.Information);
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
