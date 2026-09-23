using Microsoft.AspNetCore.Mvc;
using universityBooking.Application.DTOs;
using universityBooking.Application.Interfaces;

namespace universityBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallsController : ControllerBase
    {
        private readonly IHallService _hallService;

        public HallsController(IHallService hallService)
        {
            _hallService = hallService;
        }

        // GET: api/Halls
        [HttpGet]
        public async Task<IActionResult> GetHalls()
        {
            var halls = await _hallService.GetAllHallsAsync();
            return Ok(halls);
        }

        // GET: api/Halls/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHall(int id)
        {
            var hall = await _hallService.GetHallByIdAsync(id);
            if (hall == null)
            {
                return NotFound();
            }
            return Ok(hall);
        }

        // POST: api/Halls
        [HttpPost]
        public async Task<IActionResult> CreateHall([FromBody] HallDto dto)
        {
            if (dto == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(dto.Image))
            {
                dto.Image = "assets/hall_101.png";
            }
            var created = await _hallService.AddHallAsync(dto);
            return CreatedAtAction(nameof(GetHall), new { id = created.Id }, created);
        }

        // DELETE: api/Halls/5
        [HttpDelete("{id}")]
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteHall(int id)
        {
            try
            {
                await _hallService.DeleteHallAsync(id);
                return Ok(new { success = true, message = "تم حذف القاعة بنجاح", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
