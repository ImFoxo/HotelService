namespace HotelServiceAPI.DTOs
{
    public class SeatGetDTO
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public int Row { get; set; }
        public Guid ResourceId { get; set; }
    }
}
