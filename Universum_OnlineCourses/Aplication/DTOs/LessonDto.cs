namespace Universum_OnlineCourses.Aplication.DTOs
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string VimeoVideoId { get; set; } = null!;
        public int Order { get; set; }
    }
}
