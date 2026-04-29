using System.Runtime.CompilerServices;
using System.Security.Claims;
using HotelServiceAPI.DTOs_POST;
using HotelServiceAPI.Models;
using HotelServiceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<HotelDbUser> _signInManager;
        private readonly UserManager<HotelDbUser> _userManager;
        private readonly TokenService _tokenService;

        public AccountController(
            SignInManager<HotelDbUser> signInManager, 
            UserManager<HotelDbUser> userManager,
            TokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [Authorize]
        [HttpGet("whoami")]
        public async Task<ActionResult> WhoAmI()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (user == null)
                return Unauthorized("User not logged in");
            return Ok(new { user.Email, user.CreatedAt });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterPostDTO model)
        {
            var user = new HotelDbUser
            {
                UserName = model.Email,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                Deleted = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var token = _tokenService.CreateToken(user);

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginPostDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User with given email not found");
            if (user.Deleted)
                return Unauthorized("Account is deactivated");
            if (!(await _userManager.CheckPasswordAsync(user!, model.Password)))
                return BadRequest("Email or password incorrect");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }

        [HttpGet("login-google")]
        public IActionResult LoginGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("Error while pulling data from Google");

            // Log in if user already exists
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email!);

            if (user != null)
            {
                if (user.Deleted)
                    return Unauthorized("Account is deactivated");

                var token = _tokenService.CreateToken(user);
                return Ok(new { token });
            }
            else // Create new if user doesn't exist
            {
                user = new HotelDbUser
                {
                    UserName = email,
                    Email = email,
                    CreatedAt = DateTime.UtcNow,
                    Deleted = false
                };

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, info);
                    var token = _tokenService.CreateToken(user);
                    return Ok(new
                    {
                        Token = token,
                        User = user.Email
                    });
                }
            }

            return BadRequest("Error creating user account");
        }
    }
}
