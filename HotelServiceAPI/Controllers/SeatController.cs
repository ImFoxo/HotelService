using HotelServiceAPI.Data;
using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        public readonly HotelDbContext _context;
        public SeatController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Seat>>> GetSeats()
        {
            var seats = await _context.Seats.ToListAsync();
            return seats;
        }
    }
}
