namespace Universum_OnlineCourses.Aplication.DTOs
{
        public class CreateStripePaymentDto
    {
            public Guid UserId { get; set; }
            public Guid CourseId { get; set; }
            public decimal Amount { get; set; }
            public string PaymentType { get; set; } = "Full"; // Full | Installment
        }
    

}
