using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using static WebApi.Helpers.MessageLogger;

namespace WebApi.Data
{
    public class SubscriptionsRepository : ISubscriptionsRepository
    {
        private readonly DanceSchoolContext _context;
        public SubscriptionsRepository(DanceSchoolContext context)
        {
            _context = context;
        }

        public async Task<MessageLogger> DeleteSubscription(string subscriptionid)
        {
            MessageLogger messageLogger = new MessageLogger();
            var subscription = (object)null;
            try
            {
                subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == Int16.Parse(subscriptionid));

                if (subscription != null)
                {
                    _context.Remove(subscription);
                    await _context.SaveChangesAsync();
                }

                messageLogger.AddMessage("Subscription deleted", subscription, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetSubscriptions()
        {
            MessageLogger messageLogger = new MessageLogger();
            var subscriptions = (object)null;
            try
            {
                subscriptions = await (from v in _context.Subscriptions
                                       select new
                                       {
                                           v.Id,
                                           v.Lessonid,
                                           v.Duration,
                                           v.Price,
                                           Discount = v.Discount == true ? 1 : 0,
                                           v.Discprice
                                       }).ToListAsync();

                messageLogger.AddMessage("", subscriptions, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetSubscription(string subscriptionid)
        {
            MessageLogger messageLogger = new MessageLogger();
            var subscription = (object)null;
            try
            {
                subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == Int16.Parse(subscriptionid));
                messageLogger.AddMessage("", subscription, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Register(Subscriptions subscriptions, string lessonid, string duration, string price, bool discount, string discprice)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                // var res = await CheckDiscount(discprice, discount);
                var res = await Task.Run(() => CheckDiscount(discprice, discount));

                if (!res)
                {
                    messageLogger.AddMessage("Discount must have a discount value and oposite", null, MessageCode.Error);
                    return messageLogger;
                }

                subscriptions.Lessonid = lessonid;
                subscriptions.Duration = duration;
                subscriptions.Price = price;
                subscriptions.Discount = discount;

                if (discprice != null && !String.IsNullOrWhiteSpace(discprice))
                {
                    subscriptions.Discprice = discprice;
                }
                else
                {
                    subscriptions.Discprice = "00.00";
                }

                await _context.Subscriptions.AddAsync(subscriptions);
                await _context.SaveChangesAsync();

                messageLogger.AddMessage("Subscription inserted", subscriptions, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> CheckifExist(string lessonid, string duration, string price, bool discount, string discprice)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var checkexist = await (from s in _context.Subscriptions
                                        where s.Duration == duration
                                        where s.Lessonid == lessonid
                                        where s.Price == price
                                        where s.Discprice == discprice
                                        select s).ToListAsync();

                if (checkexist.Count > 0)
                {
                    messageLogger.AddMessage("Subscription exist", checkexist, MessageCode.Error);
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

        public async Task<MessageLogger> Update(Subscriptions subscription, string id, string lessonid, string duration, string price, bool discount, string discprice)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == Int16.Parse(id));

                if (subscription != null)
                {
                    //var res = await CheckDiscount(discprice, discount);
                    var res = await Task.Run(() => CheckDiscount(discprice, discount));


                    if (!res)
                    {
                        messageLogger.AddMessage("Discount must have a discount value and oposite", null, MessageCode.Error);
                        return messageLogger;
                    }
                    else
                    {
                        subscription.Lessonid = lessonid.Trim();
                        subscription.Duration = duration.Trim();
                        subscription.Price = price.Trim();
                        subscription.Discount = discount;

                        if (discprice != null && !String.IsNullOrWhiteSpace(discprice))
                        {
                            subscription.Discprice = discprice;
                        }
                        else
                        {
                            subscription.Discprice = "00.00";
                        }

                        await _context.SaveChangesAsync();
                        messageLogger.AddMessage("Subscription updated", subscription, MessageCode.Information);
                        return messageLogger;
                    }
                }
                else
                {
                    messageLogger.AddMessage("Subscription not found", subscription, MessageCode.Error);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        private async Task<bool> CheckDiscount(string discprice, bool discount)
        {
            return await Task.Run(() =>
             {
                 var dvalue = decimal.Parse(discprice);
                 var dvaluerounded = decimal.Round(dvalue, 2);

                 if (dvaluerounded > 0 && discount) return true;
                 else if (!discount && dvaluerounded == 0) return true;
                 else if (!discount && dvaluerounded > 0) return false;
                 else if (discount && dvaluerounded == 0) return false;
                 else return false;
             });
        }
    }
}
