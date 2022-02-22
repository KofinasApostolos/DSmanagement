using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.CustomModel;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IUsersubscriptionRepository
    {
        Task<MessageLogger> Register(UserSubscriptions userSubscriptions, string id, string lessonid,
                                            string[] day, string duration, string price, string Date, bool discount);
        Task<MessageLogger> GetUsersubscriptions();
        //Task<MessageLogger> GetUserSubscriptionStates();
        Task<MessageLogger> GetUsersubscription(string id);
        Task<MessageLogger> DeleteUsersubscription(string id);
        Task<MessageLogger> Update(UserSubscriptions userSubscriptions, string id, string userid, string lessonid,
                                        string[] day, string duration, string price, string Date, bool discount, short state);
        Task<MessageLogger> Create_And_WriteXLS_UsersSubscriptions(string id, List<CustomUserSubscriptions> customUserSubscriptions);
        Task<MessageLogger> InsertSubscription(UserSubscriptions userSubscriptions, string userid, string lessonid,
                                                       string duration, string price, string[] day, bool discount,
                                                       DateTime date, DanceSchoolContext _context, string firstname,
                                                       string lastname, string email, string lesson, IEnumerable<dynamic> list);
    }
}
