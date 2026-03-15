using HotelServiceAPI.Data;
using Microsoft.EntityFrameworkCore;
using To_Do_app_server.Data;

namespace HotelServiceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<HotelDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            
            // Automatic DB migration
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<HotelDbContext>();
                    context.Database.Migrate();
                    Console.WriteLine("Database migration successful");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while updating database: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            MapsterInitializer.SetMapsterConfig();

            app.Run();
        }
    }
}
