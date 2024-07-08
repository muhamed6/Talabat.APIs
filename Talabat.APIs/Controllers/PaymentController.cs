using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        // This is your Stripe CLI webhook secret for testing your endpoint locally.
        private const string whSecret = "whsec_4f5e115f2efd1be61b3da0d3e694365833a7d65779fe83b7693f1ee333ba64ec";


        public PaymentController(IPaymentService PaymentService, ILogger<PaymentController> logger)
        {
            _paymentService = PaymentService;
            _logger = logger;
        }
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketid}")]
        public async Task<ActionResult<CustomerBasket?>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket=await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket is null) return BadRequest(new ApiResponse(400, "An Error With your Basket"));
           
            return Ok(basket);
        }



        // strip will consume this endpoint
        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], whSecret);

            Order? order;

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, true);
                    _logger.LogInformation($"Order is succeeded {order?.PaymentIntentId}");
                    break;
                case Events.PaymentIntentPaymentFailed:
                    order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, false);
                    _logger.LogInformation($"Order is failed {order?.PaymentIntentId}");
                    break;
            }

            return Ok();

        }
    
    }
}
