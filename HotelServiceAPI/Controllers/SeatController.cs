using HotelServiceAPI.Data;
using HotelServiceAPI.DTOs;
using HotelServiceAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly HotelDbContext _context;
        public SeatController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<SeatGetDTO>>> GetSeats()
        {
            var seats = await _context.Seats.ToListAsync();
            var seatDTOs = seats.Adapt<List<SeatGetDTO>>();
            return seatDTOs;
        }

        [HttpGet("resource/{id}")]
        public async Task<ActionResult<List<SeatGetDTO>>> GetSeatsByResource(Guid id)
        {
            var resource = await _context.Resources.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == id);
            if (resource == null)
                return NotFound();
            var seats = resource.Seats.ToList();
            var seatDTOs = seats.Adapt<List<SeatGetDTO>>();
            return seatDTOs;
        }
    }
}
