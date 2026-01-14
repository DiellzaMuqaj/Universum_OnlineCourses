using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universum_OnlineCourses.Aplication.DTOs;
using Universum_OnlineCourses.Aplication.Services;
using Stripe.Checkout;
using Universum_OnlineCourses.Infrastructure.Persistence;
using Universum_OnlineCourses.Domain.Entities;


namespace Universum_OnlineCourses.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _context;

        public PaymentsController(
            IPaymentService paymentService,
            AppDbContext context)
        {
            _paymentService = paymentService;
            _context = context;
        }

        // =============================
        // CREATE STRIPE CHECKOUT SESSION
        // =============================
        [HttpPost("stripe")]
        public async Task<IActionResult> CreateStripePayment(
            [FromBody] CreateStripePaymentDto dto)
        {
            var checkoutUrl = await _paymentService.CreateStripeSessionAsync(dto);
            return Ok(new { checkoutUrl });
        }

        // =============================
        // VERIFY PAYMENT AFTER SUCCESS
        // =============================
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment(
            [FromBody] VerifyPaymentDto dto)
        {
            // 1️⃣ Get Stripe session
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(dto.SessionId);

            // 2️⃣ Must be paid
            if (session.PaymentStatus != "paid")
                return Ok(new { success = false });

            // 3️⃣ Find payment
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p =>
                    p.StripeSessionId == dto.SessionId);

            if (payment == null)
                return BadRequest("Payment not found");

            // 4️⃣ Mark payment as completed
            payment.Status = "Completed";

            // 5️⃣ Calculate allowed modules
            int allowedModules = payment.PaymentType.ToLower() switch
            {
                "full" => 4,
                "installment" => 2,
                _ => 0
            };

            // 6️⃣ Grant / update access
            var access = await _context.UserCourseAccesses
                .FirstOrDefaultAsync(x =>
                    x.UserId == payment.UserId &&
                    x.CourseId == payment.CourseId);

            if (access == null)
            {
                access = new UserCourseAccess
                {
                    UserId = payment.UserId,
                    CourseId = payment.CourseId,
                    AllowedModules = allowedModules,
                    PaidInstallments = 1
                };

                _context.UserCourseAccesses.Add(access);
            }
            else
            {
                access.AllowedModules = Math.Max(access.AllowedModules, allowedModules);
                access.PaidInstallments += 1;
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }

}