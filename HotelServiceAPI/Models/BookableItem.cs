using System.ComponentModel.DataAnnotations;
using To_Do_app_server.Models.SoftDelete;

namespace HotelServiceAPI.Models
{
    public abstract class BookableItem : ISoftDeletable
    {
        [Key]
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        // N:N
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
