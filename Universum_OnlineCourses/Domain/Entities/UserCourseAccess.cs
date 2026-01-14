namespace Universum_OnlineCourses.Domain.Entities
{
    public class UserCourseAccess
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid CourseId { get; set; }

        public Course Course { get; set; } = null!;

        public int AllowedModules { get; set; }
        public int PaidInstallments { get; set; }
    }

}
