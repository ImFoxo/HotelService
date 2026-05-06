using System;
using System.Data.Common;
using HotelServiceAPI;
using HotelServiceAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ToDoApp.Tests
{
    public class HotelWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove basic DbContext
                var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<HotelDbContext>));
                if (contextDescriptor != null)
                    services.Remove(contextDescriptor);

                var connectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                if (connectionDescriptor != null)
                    services.Remove(connectionDescriptor);

                var dbContextOptionsDescriptors = services
                    .Where(d => d.ServiceType.FullName != null &&
                    d.ServiceType.FullName.Contains("HotelDbContext"))
                    .ToList();

                foreach (var desc in dbContextOptionsDescriptors)
                {
                    services.Remove(desc);
                }

                // InMemory SQLite
                var connection = new SqliteConnection("Filename=:memory:");
                connection.Open();

                services.AddSingleton<DbConnection>(connection);

                services.AddDbContext<HotelDbContext>(options =>
                {
                    options.UseSqlite(connection);
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}