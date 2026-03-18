
using System.ComponentModel.DataAnnotations;
using HotelServiceAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Models
{
    public class Resource : BookableItem
    {
        public HotelResourceType Type { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }

        // 1:N - has many
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        public void GenerateSeats(int rows, int seatsPerRow)
        {
            for (int row = 1; row <= rows; row++)
            {
                for (int seatNumber = 1; seatNumber <= seatsPerRow; seatNumber++)
                {
                    Seats.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                        Row = row,
                        Number = seatNumber,
                        ResourceId = this.Id
                    });
                }
            }
        }
    }
}
