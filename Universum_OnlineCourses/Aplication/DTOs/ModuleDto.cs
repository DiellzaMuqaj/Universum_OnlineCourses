namespace Universum_OnlineCourses.Aplication.DTOs
{
    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public List<LessonDto> Lessons { get; set; } = new();
    }
}
