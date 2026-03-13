using HotelServiceAPI.Data;
using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        protected readonly HotelDbContext _context;

        public ResourceController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Resource>>> GetResources()
        {
            var resources = await _context.Resources.ToListAsync();

            return resources;
        }

        [HttpPost]
        public async Task<ActionResult<Resource>> CreateResource(Resource resource)
        {
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            //return CreatedAtAction(nameof(GetResources), new { id = resource.Id }, resource);
            return resource;
        }
    }
}
