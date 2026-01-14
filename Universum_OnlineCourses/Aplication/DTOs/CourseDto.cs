namespace Universum_OnlineCourses.Aplication.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal PriceFull { get; set; }
        public decimal PriceInstallments { get; set; }
        public List<ModuleDto> Modules { get; set; } = new();
    }
}
