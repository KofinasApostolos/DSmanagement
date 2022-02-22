using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Classes;
using WebApi.CustomModel;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using static WebApi.Helpers.MessageLogger;
using System.Data;
using System.Globalization;

namespace WebApi.Data
{
    public class BookingsRepository : IBookingsRepository
    {
        private readonly DanceSchoolContext _context;
        public BookingsRepository(DanceSchoolContext context)
        {
            _context = context;
        }

        public async Task<MessageLogger> GetBookings()
        {
            MessageLogger messageLogger = new MessageLogger();
            var dbbookings = (object)null;
            try
            {
                dbbookings = await (from us in _context.UserSubscriptions
                                    join l in _context.Lessons
                                    on us.Lessonid equals l.Lessonid
                                    select new
                                    {
                                        l.Lesson,
                                        us.Duration,
                                        us.Price,
                                        Date = us.Date == null ? null : us.Date.ToShortDateString()
                                    }).ToListAsync();

                messageLogger.AddMessage("", dbbookings, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetBookingsCalendar()
        {
            MessageLogger messageLogger = new MessageLogger();
            var calendarBookings = (object)null;
            try
            {
                calendarBookings = await (from tr in _context.TempRegisters
                                          join u in _context.User
                                          on tr.Userid equals u.Id
                                          join l in _context.Lessons
                                          on tr.Lessonid equals l.Lessonid
                                          select new
                                          {
                                              title = u.LastName + " " + u.FirstName,
                                              start = tr.Date
                                          }).ToListAsync();

                if (calendarBookings == null)
                {
                    messageLogger.AddMessage("", null, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    messageLogger.AddMessage("", calendarBookings, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetBookingsDetailed(string dt)
        {
            MessageLogger messageLogger = new MessageLogger();
            var calendarBookings = (object)null;
            try
            {
                string day = Convert.ToString(dt.Substring(6, 2));
                string month = Convert.ToString(dt.Substring(4, 2));
                string year = Convert.ToString(dt.Substring(0, 4));
                string fulldt = year + "/" + month + "/" + day;
                DateTime fixeddt = DateTime.Parse(fulldt);

                calendarBookings = await (from tr in _context.TempRegisters
                                          join u in _context.User
                                          on tr.Userid equals u.Id
                                          join l in _context.Lessons
                                          on tr.Lessonid equals l.Lessonid
                                          where tr.Date == fixeddt
                                          select new
                                          {
                                              Userid = u.Id,
                                              u.Username,
                                              Firstname = u.FirstName,
                                              Lastname = u.LastName,
                                              l.Lesson
                                          }).ToListAsync();

                if (calendarBookings == null)
                {
                    messageLogger.AddMessage("", null, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    messageLogger.AddMessage("", calendarBookings, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetUserSubscriptions(short id)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                List<CustomMySubscriptions> customMySubscriptions = new List<CustomMySubscriptions>();

                DataTable statesdt = MemoryTables.SubscriptionStatesTable();

                var dbbookings = await (from us in _context.UserSubscriptions
                                        join l in _context.Lessons
                                        on us.Lessonid equals l.Lessonid
                                        join st in statesdt.AsEnumerable()
                                        on us.State equals st.Field<int>("Id")
                                        where us.Userid == id
                                        select new
                                        {
                                            l.Lesson,
                                            us.Duration,
                                            us.Price,
                                            Date = us.Date == null ? null : us.Date.ToShortDateString(),
                                            ExpDate = us.Date == null ? null : us.Date.AddMonths(us.Duration).ToShortDateString(),
                                            State = st.Field<string>("Descr"),
                                            Discount = us.Discount.Value.ToString() == "1" ? "Yes" : "No",
                                            Day = us.Dayid.ToCharArray(),
                                        }).ToListAsync();

                var dd = "";
                foreach (var itm in dbbookings.ToList())
                {
                    dd = DayConvertions.GetDayFromInt(dd, itm);
                    customMySubscriptions.Add(new CustomMySubscriptions(itm.Lesson, itm.Duration.ToString(), itm.Price, itm.Date, itm.ExpDate, itm.State, itm.Discount, dd));
                    dd = "";
                }

                messageLogger.AddMessage("", customMySubscriptions, MessageCode.Information);
                return messageLogger;
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
                char[] cArray;
                string dd = "";
                dd = DayConvertions.GetIntFromDate(day, dd);

                userSubscriptions.Userid = Int32.Parse(userid);
                userSubscriptions.Lessonid = lessonid.Trim();
                userSubscriptions.Duration = Int16.Parse(duration);
                userSubscriptions.Price = price.Trim();
                userSubscriptions.Date = date;
                userSubscriptions.Dayid = dd.Trim();
                userSubscriptions.State = 1;
                userSubscriptions.Discount = discount;

                await _context.UserSubscriptions.AddAsync(userSubscriptions);
                await _context.SaveChangesAsync();

                cArray = dd.ToCharArray();

                foreach (var itm in list.ToList())
                {
                    for (var k = 0; k < cArray.Length; k++)
                    {
                        if (itm.day.ToString() == cArray[k].ToString())
                            arr.Add(itm.hours.ToString());
                    }
                }

                await Task.Run(() =>
                {
                    MailInform.SendMailRegisterSubscription(duration, price, day, discount, date, firstname, lastname, email, lesson, arr);
                });

                messageLogger.AddMessage("Subscription has been inserted", userSubscriptions, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> CheckIfFull(string Lessonid)
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
                                where tp1.Lessonid == Lessonid
                                select new
                                {
                                    day = d.Field<int>("Id"),
                                    capacity = tp1.Capacity,
                                    hours = tp1.Lessonstart + " " + tp1.Lessonend
                                }).ToListAsync();

                var getusubs = await (from us in _context.UserSubscriptions
                                      where us.State == 1
                                      where us.Lessonid == Lessonid
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
                        messageLogger.AddMessage("full", null, MessageCode.Information);
                        return messageLogger;
                    }
                    else
                    {
                        var getDaysNotFull = (from v in list2.ToList()
                                              join v1 in _context.TeachingProgram
                                              on v.Dayid.Trim() equals v1.Dayofweek.ToString().Trim()
                                              //where v.Count != "0"
                                              select v).Distinct();

                        var compl = "";
                        foreach (var itm in getDaysNotFull.ToList())
                        {
                            if (itm.Count != "0")
                                compl = compl + "," + itm.Day.ToString();
                        }
                        compl = compl.Remove(0, 1);
                        var ff = getDaysNotFull.ToList();
                        messageLogger.AddMessage("Days/hours available to buy a subscription are: " + compl, ff, MessageCode.Information);
                        return messageLogger;
                    }
                }
                else
                {
                    messageLogger.AddMessage("not full", null, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Register(UserSubscriptions userSubscriptions, string Userid, string Lessonid,
                                                           string Duration, string Price, string[] day, bool discount, DateTime date)
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
                                where tp1.Lessonid == Lessonid
                                select new
                                {
                                    day = d.Field<int>("Id"),
                                    capacity = tp1.Capacity,
                                    hours = tp1.Lessonstart + " " + tp1.Lessonend
                                }).ToListAsync();

                var getusubs = await (from us in _context.UserSubscriptions
                                      where us.State == 1
                                      where us.Lessonid == Lessonid
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
                        messageLogger.AddMessage("This kind of subscription already exists", null, MessageCode.Error);
                        return messageLogger;
                    }
                    else
                    {
                        string dd = "";

                        var days = DayConvertions.GetIntFromDateAndCheckFromList(day, dd, list2);

                        if (days)
                        {
                            messageLogger.AddMessage("", null, MessageCode.Error);
                            return messageLogger;
                        }
                    }
                }
                //check if subscription is legit from subscriptions table
                var subscriptions = await (from s in _context.Subscriptions
                                           where s.Duration == Duration.Trim()
                                           //where s.Price == Price.Trim()
                                           where s.Lessonid == Lessonid.Trim()
                                           //where s.Discount == discount
                                           select s).FirstOrDefaultAsync();

                //if subscription exists then cont, else return null cause user changed values like duration and price or lesson from client side
                if (subscriptions != null)
                {
                    CultureInfo culture = new CultureInfo("en-US");
                    decimal decimal_price = Convert.ToDecimal(subscriptions.Price, culture);
                    decimal decimal_discprice = Convert.ToDecimal(subscriptions.Discprice, culture);
                    decimal decimal_userprice = Convert.ToDecimal(Price, culture);

                    if (day.Count() == tp.Count)
                    {
                        if (discount == true)
                        {

                            if (decimal_userprice != (decimal_discprice * day.Count()))
                            {
                                messageLogger.AddMessage("Invalid price subscription", null, MessageCode.Error);
                                return messageLogger;
                            }
                        }
                        else
                        {
                            if (decimal_userprice != (decimal_price * day.Count()))
                            {
                                messageLogger.AddMessage("Invalid price subscription", null, MessageCode.Error);
                                return messageLogger;
                            }
                        }
                    }
                    else
                    {
                        if (decimal_userprice != (decimal_price * day.Count()))
                        {
                            messageLogger.AddMessage("Invalid price subscription", null, MessageCode.Error);
                            return messageLogger;
                        }
                    }

                    //check if samesubscription for the same lesson and same duration has been made
                    var userSubscription = await (from us in _context.UserSubscriptions
                                                  join u in _context.User
                                                  on us.Userid equals u.Id
                                                  join l in _context.Lessons
                                                  on us.Lessonid equals l.Lessonid
                                                  where us.Lessonid == Lessonid.Trim()
                                                  where us.Userid == Int16.Parse(Userid)
                                                  where us.Duration == Int16.Parse(Duration)
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
                            messageLogger.AddMessage("Subscription has not been expired yet", null, MessageCode.Error);
                            return messageLogger;
                        }
                        else
                        {
                            //update status to deactivated
                            userSubscriptions.State = 2;
                            await _context.SaveChangesAsync();

                            //insert new subscription row
                            return await InsertSubscription(userSubscriptions, Userid, Lessonid, Duration, Price, day, discount, date, _context,
                            userSubscription.Firstname, userSubscription.Lastname, userSubscription.Email, userSubscription.Lesson, tp);
                        }
                    }
                    else
                    {
                        //cancel prv subs of same lesson
                        var prvsubs = await (from us in _context.UserSubscriptions
                                             join u in _context.User
                                             on us.Userid equals u.Id
                                             join l in _context.Lessons
                                             on us.Lessonid equals l.Lessonid
                                             where us.Lessonid == Lessonid.Trim()
                                             where us.Userid == Int16.Parse(Userid)
                                             //where us.Duration == Int16.Parse(Duration)
                                             where us.State == 1
                                             //where us.Price == Price.Trim()
                                             //where us.Discount == discount
                                             select us).LastOrDefaultAsync();

                        if (prvsubs != null)
                        {
                            prvsubs.State = 2;
                            await _context.SaveChangesAsync();
                        }

                        //get extra infos for mail                 
                        var user = await (from u in _context.User
                                          where u.Id == Int32.Parse(Userid)
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
                            //get extra infos for lesson    
                            var lesson = await (from l in _context.Lessons
                                                where l.Lessonid == Lessonid
                                                select l.Lesson).FirstOrDefaultAsync();

                            if (lesson != null)
                                //insert new subscription row
                                return await InsertSubscription(userSubscriptions, Userid, Lessonid, Duration, Price, day, discount, date, _context,
                                user.Firstname, user.Lastname, user.Email, lesson, tp);
                            else
                            {
                                messageLogger.AddMessage("Cannot find lesson", null, MessageCode.Error);
                                return messageLogger;
                            }
                        }
                        else
                        {
                            messageLogger.AddMessage("Cannot find user", null, MessageCode.Error);
                            return messageLogger;
                        }
                    }
                }
                else
                {
                    messageLogger.AddMessage("Cannot find this type of subscription", null, MessageCode.Error);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
    }
}
