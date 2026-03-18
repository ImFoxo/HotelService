namespace HotelServiceAPI.DTOs
{
    public class BookingGetDTO
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Guid> BookedItemIds { get; set; } = new List<Guid>();
        public string UserId { get; set; } = string.Empty;
    }
}
