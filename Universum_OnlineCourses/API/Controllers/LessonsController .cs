using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Universum_OnlineCourses.Domain.Entities;
using Universum_OnlineCourses.Infrastructure.Persistence;

namespace Universum_OnlineCourses.API.Controllers
{
    [ApiController]
    [Route("api/lessons")]
    [Authorize(Roles = "Instructor")]
    public class LessonsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LessonsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLesson(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return Ok(lesson);
        }
    }

}
