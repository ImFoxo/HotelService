using System.Linq.Expressions;
using HotelServiceAPI.Enums;
using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using To_Do_app_server.Models.SoftDelete;

namespace HotelServiceAPI.Data
{
    public class HotelDbContext : IdentityDbContext<HotelDbUser>
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<BookableItem> BookableItems { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global query filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    // Only apply to the base type in the hierarchy to avoid multiple filters on derived types
                    if (entityType.BaseType == null || !typeof(ISoftDeletable).IsAssignableFrom(entityType.BaseType.ClrType))
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, "e");
                        var deletedProperty = Expression.Property(parameter, nameof(ISoftDeletable.Deleted));
                        var filter = Expression.Lambda(Expression.Equal(deletedProperty, Expression.Constant(false)), parameter);
                        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter); 
                    }
                }
            }

            // TPT configuration (Table-Per-Type)
            modelBuilder.Entity<BookableItem>().ToTable("BookableItems");
            modelBuilder.Entity<Resource>().ToTable("Resources");
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.ToTable("Seats");
                entity.HasOne(s => s.Resource)
                    .WithMany(r => r.Seats)
                    .HasForeignKey(s => s.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict); // Avoids cascade delete which can interfere with soft delete logic in TPT architecture
            });
        }

        public override int SaveChanges()
        {
            ApplySoftDeleteLogic();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplySoftDeleteLogic();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplySoftDeleteLogic()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastUpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted: // Soft delete logic
                        if (entry.Entity is Resource resource)
                        {
                            var seats = Seats.Where(s => s.ResourceId == resource.Id && !s.Deleted).ToList();
                            foreach (var seat in seats)
                            {
                                seat.Deleted = true;
                                seat.DeletedAt = DateTime.UtcNow;
                            }
                        }
                        entry.State = EntityState.Modified;
                        entry.Entity.Deleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}