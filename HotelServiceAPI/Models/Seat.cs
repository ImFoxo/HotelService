using System.ComponentModel.DataAnnotations;
using To_Do_app_server.Models.SoftDelete;

namespace HotelServiceAPI.Models
{
    public class Seat : SoftDeletableBase
    {
        [Key]
        public Guid Id { get; set; }
        public int Number{ get; set; }
        public int Row { get; set; }

        // N:1 - belongs to one
        public Guid ResourceId { get; set; }
        public Resource? Resource { get; set; }
    }
}
