namespace HotelServiceAPI.Models
{
    public class Resource
    {
        public Guid Id { get; set; }
        required public string Number { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public bool HasSeating { get; set; }
    }
}
