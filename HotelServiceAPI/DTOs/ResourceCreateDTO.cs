using HotelServiceAPI.Enums;

namespace HotelServiceAPI.DTOs
{
    public class ResourceCreateDTO
    {
        public HotelResourceType Type { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public int? Rows { get; set; }
        public int? SeatsPerRow { get; set; }
    }
}
