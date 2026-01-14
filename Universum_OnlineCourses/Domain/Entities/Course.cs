namespace Universum_OnlineCourses.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal PriceFull { get; set; }
        public decimal PriceInstallments { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Module> Modules { get; set; } = new List<Module>();
        public ICollection<UserCourseAccess> UserCourseAccesses { get; set; } = new List<UserCourseAccess>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

}

