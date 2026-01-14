using Microsoft.EntityFrameworkCore;
using Universum_OnlineCourses.Aplication.DTOs;
using Universum_OnlineCourses.Infrastructure.Persistence;

namespace Universum_OnlineCourses.Aplication.Services
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetUserCoursesAsync(Guid userId);
        Task<List<ModuleDto>> GetAccessibleModules(Guid userId, Guid courseId);
    }

    public class CourseService : ICourseService
    {
        private readonly AppDbContext _db;
        public CourseService(AppDbContext db) => _db = db;

        public async Task<List<CourseDto>> GetUserCoursesAsync(Guid userId)
        {
            var accessList = await _db.UserCourseAccesses
                .Where(a => a.UserId == userId)
                .Include(a => a.Course)
                .ThenInclude(c => c.Modules)
                .ToListAsync();

            return accessList.Select(a => new CourseDto
            {
                Id = a.Course.Id,
                Title = a.Course.Title,
                Description = a.Course.Description,
                PriceFull = a.Course.PriceFull,
                PriceInstallments = a.Course.PriceInstallments,
                Modules = a.Course.Modules.Select(m => new ModuleDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Order = m.Order,
                    Lessons = m.Lessons.Select(l => new LessonDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        VimeoVideoId = l.VimeoVideoId,
                        Order = l.Order
                    }).ToList()
                }).ToList()
            }).ToList();
        }

        public async Task<List<ModuleDto>> GetAccessibleModules(Guid userId, Guid courseId)
        {
            var access = await _db.UserCourseAccesses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.CourseId == courseId);

            if (access == null) return new List<ModuleDto>();

            var modules = await _db.Modules
                .Where(m => m.CourseId == courseId)
                .OrderBy(m => m.Order)
                .Take(access.AllowedModules)
                .Include(m => m.Lessons)
                .ToListAsync();

            return modules.Select(m => new ModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Order = m.Order,
                Lessons = m.Lessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    VimeoVideoId = l.VimeoVideoId,
                    Order = l.Order
                }).ToList()
            }).ToList();
        }
    }

}
