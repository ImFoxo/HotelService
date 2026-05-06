namespace HotelServiceAPI.DTOs
{
    public class BookingGetDTO
    {
        public Guid Id { get; set; }
        public DateOnly StartTime { get; set; }
        public DateOnly EndTime { get; set; }
        public List<Guid> BookedItemIds { get; set; } = new List<Guid>();
        public string UserId { get; set; } = string.Empty;
    }
}
