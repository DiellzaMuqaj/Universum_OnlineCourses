using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Universum_OnlineCourses.Aplication.DTOs;
using Universum_OnlineCourses.Domain.Entities;
using Universum_OnlineCourses.Infrastructure.Persistence;

namespace Universum_OnlineCourses.Aplication.Services
{
    public interface IPaymentService
    {
        Task<string> CreateStripeSessionAsync(CreateStripePaymentDto dto);
        Task ConfirmPaymentAsync(Guid userId, Guid courseId, string paymentType, string sessionId, decimal amount);
    }

    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;

        public PaymentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<string> CreateStripeSessionAsync(CreateStripePaymentDto dto)
        {
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "eur",
                        UnitAmount = (long)(dto.Amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = dto.PaymentType == "Full"
                                ? "Course – Full Payment"
                                : "Course – Installment Payment"
                        }
                    }
                }
            },
                SuccessUrl = "https://stripetest123.netlify.app/payment-success",
                CancelUrl = "https://stripetest123.netlify.app/payment-cancel",
                Metadata = new Dictionary<string, string>
            {
                { "UserId", dto.UserId.ToString() },
                { "CourseId", dto.CourseId.ToString() },
                { "PaymentType", dto.PaymentType }
            }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task ConfirmPaymentAsync(
     Guid userId,
     Guid courseId,
     string paymentType,
     string stripeSessionId,
     decimal amount)
        {
            // 1️⃣ Prevent duplicate webhook execution
            bool alreadyProcessed = await _db.Payments
                .AnyAsync(p => p.StripeSessionId == stripeSessionId);

            if (alreadyProcessed)
                return;

            // 2️⃣ Save payment
            var payment = new Payment
            {
                UserId = userId,
                CourseId = courseId,
                Amount = amount,
                Status = "Completed",
                PaymentType = paymentType,
                StripeSessionId = stripeSessionId
            };

            _db.Payments.Add(payment);

            // 3️⃣ Decide module access
            int allowedModules = paymentType.ToLower() switch
            {
                "full" => 4,
                "installment" => 2,
                _ => throw new Exception("Invalid payment type")
            };

            // 4️⃣ Grant / upgrade access
            var access = await _db.UserCourseAccesses
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.CourseId == courseId);

            if (access == null)
            {
                access = new UserCourseAccess
                {
                    UserId = userId,
                    CourseId = courseId,
                    AllowedModules = allowedModules,
                    PaidInstallments = 1
                };

                _db.UserCourseAccesses.Add(access);
            }
            else
            {
                access.AllowedModules = Math.Max(access.AllowedModules, allowedModules);
                access.PaidInstallments += 1;
            }

            await _db.SaveChangesAsync();
        }

    }
}