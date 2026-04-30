using HotelServiceAPI.Data;
using HotelServiceAPI.DTOs;
using HotelServiceAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : HotelControllerBase
    {
        private readonly HotelDbContext _context;
        public BookingController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookingGetDTO>>> GetMyBookings()
        {
            var userId = CurrentUserId;

            var bookings = await _context.Bookings
                                    .Where(b => b.UserId == userId)
                                    .Include(b => b.BookedItems)
                                    .ToListAsync();

            var bookingDTOs = bookings.Adapt<List<BookingGetDTO>>();
            return bookingDTOs;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<BookingGetDTO>>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                                    .Include(b => b.BookedItems)
                                    .ToListAsync();

            var bookingDTOs = bookings.Adapt<List<BookingGetDTO>>();
            return bookingDTOs;
        }

        [HttpPost]
        public async Task<ActionResult> PostBooking(BookingPostDTO bookingPostDTO)
        {
            var userId = CurrentUserId;

            Booking newBooking = bookingPostDTO.Adapt<Booking>();

            if (newBooking.StartTime >= newBooking.EndTime)
                return BadRequest("Reservation must begin before it ends.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                    var collidingBookings = await _context.Bookings
                        .Where(b => b.StartTime < bookingPostDTO.EndTime &&
                                    b.EndTime > bookingPostDTO.StartTime &&
                                    b.BookedItems.Any(i => bookingPostDTO.ItemIds.Contains(i.Id)))
                        .Select(b => new
                        {
                            b.Id,
                            Items = b.BookedItems
                                .Where(i => bookingPostDTO.ItemIds.Contains(i.Id))
                                .Select(i => i.Id)
                        })
                        .ToListAsync();
                
                if (collidingBookings.Any())
                    return Conflict($"The following items are already booked for the selected time: {string.Join(", ", collidingBookings.SelectMany(c => c.Items))}");

                var itemsToBook = await _context.BookableItems
                                        .Where(i => bookingPostDTO.ItemIds.Contains(i.Id))
                                        .ToListAsync();

                foreach (var item in itemsToBook)
                {
                    if (item is Seat s)
                    {
                        var events = _context.Bookings.Where(b => b.StartTime < bookingPostDTO.EndTime &&
                                                             b.EndTime > bookingPostDTO.StartTime);
                        if (events == null)
                            return BadRequest("No events found for chosen seat in reservation time, seat id: " + item.Id);
                        if (events.Any(e => e.IsPrivate))
                            return BadRequest("One or more event connected to chosen seat is private, seat id: " + item.Id);
                    }
                }

                if (itemsToBook.Count != bookingPostDTO.ItemIds.Count)
                    return NotFound("One or more of the specified items were not found.");

                newBooking.BookedItems = itemsToBook;
                newBooking.UserId = userId;

                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new { Message = "Reservation created successfully.", BookingId = newBooking.Id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the booking.");
            }
        }
    }
}
