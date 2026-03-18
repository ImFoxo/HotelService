namespace HotelServiceAPI.DTOs
{
    public class BookingPostDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Guid> ItemIds { get; set; } = new List<Guid>(); 
    }
}
