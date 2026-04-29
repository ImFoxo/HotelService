using System.Security.Claims;
using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelServiceAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class HotelControllerBase : ControllerBase
    {
        protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}

    
