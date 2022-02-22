using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using WebApi.Helpers;
using static WebApi.Helpers.MessageLogger;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingsRepository _repo;
        public BookingsController(IBookingsRepository repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "1,2")]
        [HttpGet("calendar")]
        public async Task<IActionResult> GetBookingsCalendar()
        {
            MessageLogger bookings = await _repo.GetBookingsCalendar();

            if (bookings.msg[0].Obj == null)
                return BadRequest(bookings.msg[0].Message);
            else
                return Ok(bookings.msg[0].Obj);
        }

        [Authorize(Roles = "1,2")]
        [HttpGet("detailed/{id}")]
        public async Task<IActionResult> GetBookingsDetailed(string id)
        {
            MessageLogger bookings = await _repo.GetBookingsDetailed(id);

            if (bookings.msg[0].Obj == null)
                return BadRequest(bookings.msg[0].Message);
            else
                return Ok(bookings.msg[0].Obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            MessageLogger bookings = await _repo.GetBookings();

            if (bookings.msg[0].Obj == null)
                return BadRequest(bookings.msg[0].Message);
            else
                return Ok(bookings.msg[0].Obj);
        }

        [HttpGet("capacity/{id}")]
        public async Task<IActionResult> GetCapacitySubscriptions(string id)
        {
            var bookings = await _repo.CheckIfFull(id);


            if (bookings.msg[0].Message == null || bookings.msg[0].Messagecode == MessageCode.Error)
                return BadRequest(bookings.msg[0].Message);
            else
                return Ok(bookings.msg[0]);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserSubscriptions(short id)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != id.ToString())
                return Unauthorized();
            else
            {
                MessageLogger bookings = await _repo.GetUserSubscriptions(id);

                if (bookings.msg[0].Obj == null)
                    return BadRequest(bookings.msg[0].Message);
                else
                    return Ok(bookings.msg[0].Obj);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registerstring(BookingForRegister bookingForRegister)
        {
            var iClaim = User.Claims.FirstOrDefault().Value;

            if (iClaim != bookingForRegister.Userid.ToString())
                return Unauthorized();
            else
            {
                var bookingToCreate = new UserSubscriptions();

                MessageLogger createdBooking = await _repo.Register(bookingToCreate, bookingForRegister.Userid, bookingForRegister.Lessonid,
                                                               bookingForRegister.Duration, bookingForRegister.Price, bookingForRegister.Day,
                                                               bookingForRegister.Discount, bookingForRegister.Date);

                if (createdBooking.msg[0].Obj == null)
                    return BadRequest(createdBooking.msg[0].Message);
                else
                    return Ok(createdBooking.msg[0]);
            }
        }
    }
}
