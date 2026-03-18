using HotelServiceAPI.Data;
using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using To_Do_app_server.Data;

namespace HotelServiceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Db context
            builder.Services.AddDbContext<HotelDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentityApiEndpoints<HotelDbUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<HotelDbContext>();

            // Identity options
            // TODO: remove in production
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Booking API", Version = "v1" });

                // Security scheme (Bearer Token)
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Wpisz sam token w polu poni¿ej (bez s³owa Bearer)."
                });

                // Documentation
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();
            
            // Automatic DB update from migrations
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

            if (!app.Environment.IsDevelopment())
                app.UseHsts();
            app.UseHttpsRedirection();

            // Create login and register endpoints
            app.MapIdentityApi<HotelDbUser>();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            MapsterInitializer.SetMapsterConfig();

            // Seeding data
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<HotelDbUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                DbInitilizer.Initialize(dbContext, userManager, roleManager).Wait();
            }

            app.Run();
        }
    }
}
