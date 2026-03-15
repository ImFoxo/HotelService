using HotelServiceAPI.Enums;
using HotelServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }
        
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Resource>()
            //    .Property(r => r.Type)
            //    .HasConversion(
            //        v => v.ToString(), // Conversion while saving (Enum -> string)
            //        v => (HotelResourceType)Enum.Parse(typeof(HotelResourceType), v) // Conversion while retrieving (string -> Enum)
            //    );

        }
    }
}
