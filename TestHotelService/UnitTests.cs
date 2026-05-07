using FluentAssertions;
using HotelServiceAPI;
using HotelServiceAPI.Data;
using HotelServiceAPI.DTOs_POST;
using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Tests;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using System.Net;
using HotelServiceAPI.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace TestHotelService
{
    public class BookingTests : HotelTestBase
    {
        public BookingTests(HotelWebAppFactory factory) : base(factory)
        {
        }

        // Unit tests
        [Fact]
        public async Task SeedData_ReturnsSuccess()
        {
            // Arrange
            SeedMockData();

            using (var scope = _factory.Services.CreateScope())
            {
                // Act
                var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
                var users = await context.Users.ToListAsync();
                var resources = await context.Resources.ToListAsync();
                var bookings = await context.Bookings.ToListAsync();
                
                // Assert
                users.Should().NotBeNullOrEmpty();
                resources.Should().NotBeNullOrEmpty();
                bookings.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public async Task Post_Register_ReturnsSuccess()
        {
            // Arrange
            SeedMockData();

            var registerData = new RegisterPostDTO
            {
                Email = "newUser@test.com",
                Password = "newUser123"
            };
            var registerContent = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");

            using (var scope = _factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<HotelDbUser>>();

                // Act
                var response = await _client.PostAsync("/account/register", registerContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

                var newUser = await userManager.FindByEmailAsync(registerData.Email);

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Should().ContainKey("token");
                result["token"].Should().NotBeNullOrEmpty();
                newUser.Should().NotBeNull(); 
            }
        }

        [Fact]
        public async Task Post_Register_WeakPassword_ReturnsBadRequest()
        {
            // Arrange
            SeedMockData();

            var registerData = new RegisterPostDTO
            {
                Email = "newUser@test.com",
                Password = "123"
            };
            var registerContent = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");

            using (var scope = _factory.Services.CreateScope())
            {
                var usermanager = scope.ServiceProvider.GetRequiredService<UserManager<HotelDbUser>>();

                // Act
                var response = await _client.PostAsync("/account/register", registerContent);

                var newUser = await usermanager.FindByEmailAsync(registerData.Email);

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                newUser.Should().BeNull(); 
            }
        }

        [Fact]
        public async Task Post_Register_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            SeedMockData();
            var registerData = new RegisterPostDTO
            {
                Email = user1Email,
                Password = "newUser123"
            };
            var registerContent = new StringContent(JsonConvert.SerializeObject(registerData), Encoding.UTF8, "application/json");

            using (var scope = _factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<HotelDbUser>>();

                // Act
                var response = await _client.PostAsync("/account/register", registerContent);

                var usersWithEmail = await userManager.Users.Where(u => u.Email == user1Email).ToListAsync();

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                usersWithEmail.Count.Should().Be(1); 
            }
        }

        [Fact]
        public async Task Post_Login_ReturnsSuccess()
        {
            // Arrange
            SeedMockData();

            var loginData = new LoginPostDTO
            {
                Email = user1Email,
                Password = userPassword
            };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/account/login", loginContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().ContainKey("token");
            result["token"].Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Post_Booking_OverlappingDates_ReturnsConflict()
        {
            // Arrange
            SeedMockData();
            await SetUserJWT(user1Email, userPassword);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

                var existingBooking = await context.Bookings.Include(b => b.BookedItems).FirstAsync(b => b.User!.Email != user1Email);
                var bookingData = new BookingPostDTO
                {
                    StartTime = existingBooking.EndTime.AddDays(-1),
                    EndTime = existingBooking.EndTime.AddDays(1),
                    ItemIds = { existingBooking.BookedItems.First().Id }
                };
                var bookingContent = new StringContent(JsonConvert.SerializeObject(bookingData), Encoding.UTF8, "application/json");

                // Act
                var response = await _client.PostAsync("/booking", bookingContent);

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.Conflict); 
            }
        }

        [Fact]
        public async Task Post_Booking_SeatForPrivateEvent_ReturnsBadRequest()
        {
            // Arrange
            SeedMockData();
            await SetUserJWT(user3Email, userPassword);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
                var privateEvent = await context.Bookings.Include(b => b.BookedItems).FirstOrDefaultAsync(b => b.IsPrivate);
                var resourceId = privateEvent!.BookedItems.OfType<Resource>().FirstOrDefault()!.Id;
                var resource = await context.Resources.Include(r => r.Seats).FirstOrDefaultAsync(r => r.Id == resourceId);
                var seat = resource!.Seats.FirstOrDefault();
                var seatId = seat!.Id;

                var bookingData = new BookingPostDTO
                {
                    StartTime = privateEvent.StartTime,
                    EndTime = privateEvent.EndTime,
                    ItemIds = { seatId }
                };
                var bookingContent = new StringContent(JsonConvert.SerializeObject(bookingData), Encoding.UTF8, "application/json");

                // Act
                var response = await _client.PostAsync("/booking", bookingContent);

                // Assert
                privateEvent.IsPrivate.Should().BeTrue();
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task Post_Booking_SeatForNonExistingEvent_ReturnsBadRequest()
        {
            // Arrange
            SeedMockData();
            await SetUserJWT(user3Email, userPassword);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
                var seat = context.Seats.FirstOrDefault();

                var bookingData = new BookingPostDTO
                {
                    StartTime = DateOnly.FromDateTime(DateTime.Now).AddDays(100),
                    EndTime = DateOnly.FromDateTime(DateTime.Now).AddDays(101),
                    ItemIds = { seat!.Id }
                };
                var bookingContent = new StringContent(JsonConvert.SerializeObject(bookingData), Encoding.UTF8, "application/json");

                // Act
                var response = await _client.PostAsync("/booking", bookingContent);

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            }
        }

    }
}