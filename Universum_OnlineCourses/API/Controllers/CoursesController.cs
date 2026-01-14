using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universum_OnlineCourses.Aplication.Services;
using Universum_OnlineCourses.Domain.Entities;
using Universum_OnlineCourses.Infrastructure.Persistence;

namespace Universum_OnlineCourses.API.Controllers
{
[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserCourses(Guid userId)
        {
            var courses = await _courseService.GetUserCoursesAsync(userId);
            return Ok(courses);
        }
        [HttpGet("{courseId}/modules/user/{userId}")]
        public async Task<IActionResult> GetAccessibleModules(Guid courseId, Guid userId)
        {
            var modules = await _courseService.GetAccessibleModules(userId, courseId);
            return Ok(modules);
        }

    }


}
