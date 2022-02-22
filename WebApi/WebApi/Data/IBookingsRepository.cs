using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IBookingsRepository
    {
        Task<MessageLogger> Register(UserSubscriptions userSubscriptions, string Userid, string Lessonid,
                                                           string Duration, string Price, string[] Day, bool Discount, DateTime Date);
        Task<MessageLogger> GetBookings();
        Task<MessageLogger> CheckIfFull(string Lessonid);
        Task<MessageLogger> GetBookingsCalendar();
        Task<MessageLogger> GetBookingsDetailed(string dt);
        Task<MessageLogger> GetUserSubscriptions(short id);
    }
}
