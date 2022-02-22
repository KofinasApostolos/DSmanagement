using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.CustomModel;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersubscriptionsController : ControllerBase
    {
        private readonly IUsersubscriptionRepository _repo;
        public UsersubscriptionsController(IUsersubscriptionRepository repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public async Task<IActionResult> GetUsersubscriptions()
        {
            MessageLogger userSubscriptions = await _repo.GetUsersubscriptions();

            if (userSubscriptions.msg[0].Obj == null)
                return BadRequest(userSubscriptions.msg[0].Message);
            else
                return Ok(userSubscriptions.msg[0].Obj);
        }

        [Authorize(Roles = "1")]
        [HttpPost("{id}/userssubsxls")]
        public async Task<IActionResult> Create_And_WriteXLS_UsersSubs(string id, [FromBody]List<CustomUserSubscriptions> customUserSubscriptions)
        {
            if (customUserSubscriptions.Count == 0)
            {
                return BadRequest("There are no user subscriptions to export");
            }
            else
            {
                MessageLogger xlsusersubs = await _repo.Create_And_WriteXLS_UsersSubscriptions(id, customUserSubscriptions);

                if (xlsusersubs.msg[0].Obj == null)
                    return BadRequest(xlsusersubs.msg[0].Message);
                else
                    return Ok(xlsusersubs.msg[0]);
            }
        }

        //[HttpGet("states")]
        //public async Task<IActionResult> GetUserSubscriptionStates()
        //{
        //    MessageLogger userSubscriptionsStates = await _repo.GetUserSubscriptionStates();

        //    if (userSubscriptionsStates.msg[0].Obj == null)
        //        return BadRequest(userSubscriptionsStates.msg[0].Message);
        //    else
        //        return Ok(userSubscriptionsStates.msg[0].Obj);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsersubscription(string id)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != id.ToString())
            {
                return Unauthorized();
            }
            else
            {
                MessageLogger userSubscriptions = await _repo.GetUsersubscription(id);

                if (userSubscriptions.msg[0].Obj == null)
                    return BadRequest(userSubscriptions.msg[0].Message);
                else
                    return Ok(userSubscriptions.msg[0].Obj);
            }
        }

        [Authorize(Roles = "1")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UsersubscriptionForRegister usersubscriptionForRegister)
        {
            var usersubscriptionToCreate = new UserSubscriptions
            {
                Userid = Int32.Parse(usersubscriptionForRegister.Id)
            };

            MessageLogger createdUsersubscription = await _repo.Register(usersubscriptionToCreate, usersubscriptionForRegister.Id, usersubscriptionForRegister.Lessonid,
                                                                usersubscriptionForRegister.Day, usersubscriptionForRegister.Duration, usersubscriptionForRegister.Price,
                                                                  usersubscriptionForRegister.Date, usersubscriptionForRegister.Discount);
            if (createdUsersubscription.msg[0].Obj == null)
                return BadRequest(createdUsersubscription.msg[0].Message);
            else
                return Ok(createdUsersubscription.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsersubscription(string id)
        {
            MessageLogger usersubscription = await _repo.DeleteUsersubscription(id);

            if (usersubscription.msg[0].Obj == null)
                return BadRequest(usersubscription.msg[0].Message);
            else
                return Ok(usersubscription.msg[0]);
        }

        [Authorize(Roles = "1")]
        [HttpPut]
        public async Task<IActionResult> EditUsersubscription(UsersubscriptionForEdit usersubscriptionForEdit)
        {
            MessageLogger userSubscription = await _repo.Update(null, usersubscriptionForEdit.Id, usersubscriptionForEdit.Userid, usersubscriptionForEdit.Lessonid,
                                                        usersubscriptionForEdit.Day, usersubscriptionForEdit.Duration, usersubscriptionForEdit.Price,
                                                        usersubscriptionForEdit.Date, usersubscriptionForEdit.Discount, usersubscriptionForEdit.State);

            if (userSubscription.msg[0].Obj == null)
                return BadRequest(userSubscription.msg[0].Message);
            else
                return Ok(userSubscription.msg[0]);
        }
    }
}
