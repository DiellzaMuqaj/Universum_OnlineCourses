namespace Universum_OnlineCourses.Domain.Entities
{
    public class Lesson
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string VimeoVideoId { get; set; } = null!;
        public int Order { get; set; }
    }
}
