namespace Universum_OnlineCourses.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public string StripeSessionId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentType { get; set; } = "Full";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
