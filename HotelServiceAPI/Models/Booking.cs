using System.ComponentModel.DataAnnotations;
using To_Do_app_server.Models.SoftDelete;

namespace HotelServiceAPI.Models
{
    public class Booking : SoftDeletableBase
    {
        public DateOnly StartTime { get; set; }
        public DateOnly EndTime { get; set; }
        public bool IsPrivate { get; set; } = false; // for private events when booking halls

        // N:1 - belongs to one
        public string UserId { get; set; } = string.Empty;
        public HotelDbUser? User { get; set; }

        // N:N
        public ICollection<BookableItem> BookedItems { get; set; } = new List<BookableItem>();
    }
}
