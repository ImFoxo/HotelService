using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HotelServiceAPI.Data;
using HotelServiceAPI.DTOs_POST;
using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ToDoApp.Tests;

namespace TestHotelService
{
    public class HotelTestBase : IClassFixture<HotelWebAppFactory>
    {
        protected readonly HttpClient _client;
        protected readonly HotelWebAppFactory _factory;

        // Email constants for test users
        protected readonly string user1Email = "user1@user.com";
        protected readonly string user2Email = "user2@user.com";
        protected readonly string user3Email = "user3@user.com";
        protected readonly string adminEmail = "admin@admin.com";
        protected readonly string userPassword = "user123";
        protected readonly string adminPassword = "admin123";

        public HotelTestBase(HotelWebAppFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        protected void SeedMockData()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<HotelDbContext>();
                var userManager = services.GetRequiredService<UserManager<HotelDbUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                context.Database.EnsureDeleted();
                DbInitilizer.Initialize(context, userManager, roleManager).Wait();
            }
        }

        protected async Task SetUserJWT(string email, string password)
        {
            var loginData = new LoginPostDTO { Email = email, Password = password };
            var loginContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var loginResponse = await _client!.PostAsync("/account/login", loginContent);
            loginResponse.EnsureSuccessStatusCode();
            var loginJson = await loginResponse.Content.ReadAsStringAsync();
            dynamic loginResult = JsonConvert.DeserializeObject(loginJson)!;
            string token = loginResult.token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
