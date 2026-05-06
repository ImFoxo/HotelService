namespace HotelServiceAPI.DTOs
{
    public class BookingPostDTO
    {
        public DateOnly StartTime { get; set; }
        public DateOnly EndTime { get; set; }
        public List<Guid> ItemIds { get; set; } = new List<Guid>(); 
    }
}
