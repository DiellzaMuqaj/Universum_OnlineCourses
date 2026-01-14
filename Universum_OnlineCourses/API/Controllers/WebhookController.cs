using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Universum_OnlineCourses.Aplication.Services;
namespace Universum_OnlineCourses.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;

        public StripeWebhookController(
            IPaymentService paymentService,
            IConfiguration config)
        {
            _paymentService = paymentService;
            _config = config;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _config["Stripe:WebhookSecret"]
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    await _paymentService.ConfirmPaymentAsync(
                        Guid.Parse(session.Metadata["UserId"]),
                        Guid.Parse(session.Metadata["CourseId"]),
                        session.Metadata["PaymentType"],
                        session.Id,
                        session.AmountTotal.Value / 100m
                    );
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }

    }

}
