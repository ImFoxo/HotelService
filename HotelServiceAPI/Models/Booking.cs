using System.ComponentModel.DataAnnotations;
using To_Do_app_server.Models.SoftDelete;

namespace HotelServiceAPI.Models
{
    public class Booking : SoftDeletableBase
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // N:1 - belongs to one
        public string UserId { get; set; } = string.Empty;
        public HotelDbUser? User { get; set; }

        // N:N
        public ICollection<BookableItem> BookedItems { get; set; } = new List<BookableItem>();
    }
}
