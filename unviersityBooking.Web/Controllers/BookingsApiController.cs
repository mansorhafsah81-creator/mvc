using Microsoft.AspNetCore.Mvc;
using universityBooking.Application.DTOs;
using universityBooking.Application.Interfaces;

namespace universityBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        // POST: api/Bookings (يستدعى من فلاتر عند تأكيد الحجز)
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            if (dto == null)
            {
                return BadRequest("بيانات الحجز فارغة");
            }

            var created = await _bookingService.CreateBookingAsync(dto);
            return Ok(created);
        }

        // DELETE: api/Bookings/5 (يستدعى من فلاتر أو الويب لإلغاء/حذف الحجز)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var success = await _bookingService.CancelBookingAsync(id);
            if (!success)
            {
                return NotFound(new { message = "الحجز غير موجود" });
            }

            return Ok(new { message = "تم إلغاء الحجز بنجاح" });
        }
    }
}
