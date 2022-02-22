using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Data
{
    public interface ISubscriptionsRepository
    {
        Task<MessageLogger> Register(Subscriptions subscriptions, string lessonid, string duration, string price, bool discount, string discprice);
        Task<MessageLogger> GetSubscriptions();
        Task<MessageLogger> GetSubscription(string subscriptionid);
        Task<MessageLogger> DeleteSubscription(string subscriptionid);
        Task<MessageLogger> Update(Subscriptions subscriptions, string id, string lessonid, string duration, string price, bool discount, string discprice);
        Task<MessageLogger> CheckifExist(string lessonid, string duration, string price, bool discount, string discprice);
    }
}
