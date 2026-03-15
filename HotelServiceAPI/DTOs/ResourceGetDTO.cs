using HotelServiceAPI.Enums;

namespace HotelServiceAPI.DTOs
{
    public class ResourceGetDTO
    {
        public Guid Id { get; set; }
        public HotelResourceType Type { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public List<Guid> SeatIds { get; set; } = new List<Guid>();
    }
}
