using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionsRepository _repo;
        public SubscriptionsController(ISubscriptionsRepository repo)
        {
            _repo = repo;
        }


        [HttpGet]
        public async Task<IActionResult> GetSubscriptions()
        {
            MessageLogger subscriptions = await _repo.GetSubscriptions();

            if (subscriptions.msg[0].Obj == null)
                return BadRequest(subscriptions.msg[0].Message);
            else
                return Ok(subscriptions.msg[0].Obj);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscription(string id)
        {
            MessageLogger subscription = await _repo.GetSubscription(id);

            if (subscription.msg[0].Obj == null)
                return BadRequest(subscription.msg[0].Message);
            else
                return Ok(subscription.msg[0].Obj);
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(string id)
        {
            MessageLogger subscription = await _repo.DeleteSubscription(id);

            if (subscription.msg[0].Obj == null)
                return BadRequest(subscription.msg[0].Message);
            else
                return Ok(subscription.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpPut]
        public async Task<IActionResult> UpdateSubscription(SubscriptionForEdit subscriptionForEdit)
        {
            MessageLogger subscription = await _repo.Update(null, subscriptionForEdit.Id.ToString(), subscriptionForEdit.Lessonid,
                                                subscriptionForEdit.Duration, subscriptionForEdit.Price, subscriptionForEdit.Discount, subscriptionForEdit.Discprice);

            if (subscription.msg[0].Obj == null)
                return BadRequest(subscription.msg[0].Message);
            else
                return Ok(subscription.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(SubscriptionForRegister subscriptionForRegister)
        {
            MessageLogger exist = await _repo.CheckifExist(subscriptionForRegister.Lessonid, subscriptionForRegister.Duration,
                                        subscriptionForRegister.Price, subscriptionForRegister.Discount, subscriptionForRegister.Discprice);

            if (exist.msg[0].Obj != null)
            {
                return BadRequest(exist.msg[0].Message);
            }
            {
                var subscriptionToCreate = new Subscriptions
                {
                    Lessonid = subscriptionForRegister.Lessonid
                };

                MessageLogger createdSubscription = await _repo.Register(subscriptionToCreate, subscriptionForRegister.Lessonid,
                                                               subscriptionForRegister.Duration, subscriptionForRegister.Price,
                                                               subscriptionForRegister.Discount, subscriptionForRegister.Discprice);

                if (createdSubscription.msg[0].Obj == null)
                    return BadRequest(createdSubscription.msg[0].Message);
                else
                    return Ok(createdSubscription.msg[0]);
            }
        }
    }
}