using HotelServiceAPI.Data;
using HotelServiceAPI.DTOs;
using HotelServiceAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        protected readonly HotelDbContext _context;
        public BookingController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookingGetDTO>>> GetBookings()
        {
            var bookings = await _context.Bookings.Include(b => b.Resources).ToListAsync();
            var bookingDTOs = bookings.Adapt<List<BookingGetDTO>>();
            return bookingDTOs;
        }
    }
}
