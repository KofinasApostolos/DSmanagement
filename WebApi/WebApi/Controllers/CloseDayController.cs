using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Authorize(Roles = "1")]
    [Route("api/[controller]")]
    [ApiController]
    public class CloseDayController : ControllerBase
    {
        private readonly DanceSchoolContext _context;
        public CloseDayController(DanceSchoolContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CloseDay()
        {
            try
            {
                //update user subscriptions
                var updateUserSubscriptions = await (from us in _context.UserSubscriptions
                                               where us.State == 1
                                               select us).ToListAsync();

                if (updateUserSubscriptions != null)
                {
                    foreach (var itm in updateUserSubscriptions.ToList())
                    {
                        //check if subscription has not finished yet.
                        DateTime localDate = DateTime.Now;
                        var months = itm.Duration;
                        var dt = itm.Date.AddMonths(months);
                        int result = DateTime.Compare(dt, localDate);

                        if (result < 0) //subscription has been expired
                            itm.State = 2;
                        else //1 subscription has not been expired
                            itm.State = 1;
                    }
                    await _context.SaveChangesAsync();
                }
                return Ok("Operation completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("Operation failed!");
            }
        }
    }
}