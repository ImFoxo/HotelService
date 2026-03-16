using HotelServiceAPI.Data;
using HotelServiceAPI.DTOs;
using HotelServiceAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpGet("all")]
        public async Task<ActionResult<List<ResourceGetDTO>>> GetResources()
        {
            var resources = await _context.Resources.Include(x => x.Seats).ToListAsync();
            var resourceDTOs = resources.Adapt<List<ResourceGetDTO>>();
            return resourceDTOs;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceGetDTO>> GetResource(Guid id)
        {
            var resource = await _context.Resources.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == id);
            var resourceDTO = resource.Adapt<ResourceGetDTO>();
            return resourceDTO;
        }

        [HttpPost]
        public async Task<ActionResult> CreateResource(ResourceCreateDTO resourceDTO)
        {
            Resource resource = new Resource
            {
                Type = resourceDTO.Type,
                Number = resourceDTO.Number,
                Floor = resourceDTO.Floor,
                Capacity = resourceDTO.Capacity
            };

            if (resourceDTO.Rows > 0 && resourceDTO.SeatsPerRow > 0)
                resource.GenerateSeats((int)resourceDTO.Rows, (int)resourceDTO.SeatsPerRow);

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
