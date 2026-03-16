using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HotelServiceAPI.Models
{
    public class HotelDbUser : IdentityUser
    {
        [Required]
        public bool Deleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // 1:N - has many
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
