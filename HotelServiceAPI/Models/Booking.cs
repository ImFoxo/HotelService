using System.ComponentModel.DataAnnotations;
using To_Do_app_server.Models.SoftDelete;

namespace HotelServiceAPI.Models
{
    public class Booking : SoftDeletableBase
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // N:1 - belongs to one
        //public Guid UserId { get; set; }

        // N:N
        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}
