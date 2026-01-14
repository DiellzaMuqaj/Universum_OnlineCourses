using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universum_OnlineCourses.Domain.Entities;
using Universum_OnlineCourses.Infrastructure.Persistence;

namespace Universum_OnlineCourses.API.Controllers
{
    [ApiController]
    [Route("api/modules")]
    [Authorize(Roles = "Instructor")]
    public class ModulesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModulesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateModule(Module module)
        {
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return Ok(module);
        }
    }
}
