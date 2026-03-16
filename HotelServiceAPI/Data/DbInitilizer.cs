using System.Threading.Tasks;
using HotelServiceAPI.Enums;
using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelServiceAPI.Data
{
    public class DbInitilizer
    {
        public static async Task Initialize(
            HotelDbContext context,
            UserManager<HotelDbUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            await GenerateRolesAsync(roleManager);
            await GenerateUsersAsync(userManager);
            await GenerateResourcesAsync(context);
            await GenerateBookingsAsync(context, userManager);

            context.SaveChanges();
        }

        private static async Task GenerateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task GenerateUsersAsync(UserManager<HotelDbUser> userManager)
        {
            string adminEmail = "admin@admin.com";
            string adminPassword = "admin123";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new HotelDbUser { UserName = adminEmail, Email = adminEmail };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (roleResult.Succeeded)
                        Console.WriteLine("Admin user created successfully");
                    else
                        Console.WriteLine($"Error assigning admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
                else
                {
                    Console.WriteLine($"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            string[] userEmails = ["user1@user.com", "user2@user.com"];
            string userPassword = "user123";
            foreach (var userEmail in userEmails)
            {
                if (await userManager.FindByEmailAsync(userEmail) == null)
                {
                    var user = new HotelDbUser { UserName = userEmail, Email = userEmail };
                    var result = await userManager.CreateAsync(user, userPassword);
                    if (result.Succeeded)
                    {
                        var roleResult = await userManager.AddToRoleAsync(user, "User");
                        if (roleResult.Succeeded)
                            Console.WriteLine($"User {userEmail} created successfully");
                        else
                            Console.WriteLine($"Error assigning user role to {userEmail}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                    else
                    {
                        Console.WriteLine($"Error creating user {userEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private static async Task GenerateResourcesAsync(HotelDbContext context)
        {
            if (!context.Resources.Any())
            {
                var resources = new Resource[]
                {
                    new Resource { Type = HotelResourceType.Room, Capacity = 4, Floor = 1, Number= 1},
                    new Resource { Type = HotelResourceType.Room, Capacity = 2, Floor = 1, Number= 2},
                    new Resource { Type = HotelResourceType.Hall, Capacity = 10, Floor = 2, Number= 3},
                    new Resource { Type = HotelResourceType.Hall, Capacity = 20, Floor = 2, Number= 4},
                };
                foreach (var resource in resources)
                {
                    if (resource.Type == HotelResourceType.Hall)
                        resource.GenerateSeats(2, 2);
                    context.Resources.Add(resource);
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task GenerateBookingsAsync(
            HotelDbContext context,
            UserManager<HotelDbUser> userManager)
        {
            if (!context.Bookings.Any())
            {
                var adminUser = await userManager.FindByEmailAsync("admin@admin.com");
                var user1 = await userManager.FindByEmailAsync("user1@user.com");
                var user2 = await userManager.FindByEmailAsync("user2@user.com");
                if (adminUser == null || user1 == null || user2 == null)
                {
                    Console.WriteLine("Error: Admin or user accounts not found. Cannot create bookings.");
                    return;
                }

                List <Resource> resources = context.Resources.Where(r => r.Type == HotelResourceType.Room).ToList();
                if (resources.Count < 2)
                {
                    Console.WriteLine("Error: Not enough resources available to create bookings.");
                    return;
                }

                var bookingAdmin = new Booking
                {
                    UserId = adminUser!.Id,
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(2)
                };
                bookingAdmin.Resources.Add(resources[0]);
                context.Bookings.Add(bookingAdmin);
                await context.SaveChangesAsync();

                var bookingUser1 = new Booking
                {
                    UserId = user1!.Id,
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(2)
                };
                bookingUser1.Resources.Add(resources[1]);
                context.Bookings.Add(bookingUser1);
                await context.SaveChangesAsync();

                var bookingUser2 = new Booking
                {
                    UserId = user2!.Id,
                    StartTime = DateTime.Now.AddDays(3),
                    EndTime = DateTime.Now.AddDays(4)
                };
                bookingUser2.Resources.Add(resources[1]);
                context.Bookings.Add(bookingUser2);
                await context.SaveChangesAsync();
            }
        }
    }
}
