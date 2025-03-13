using HotelBookingDb.Data;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;

[Route("api/payment")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly HotelBookingDbContext _context;
    public PaymentsController(IConfiguration configuration, HotelBookingDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("create-checkout-session/{bookingID}")]
    public async Task<IActionResult> CreateCheckoutSession(int bookingID)
    {
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

        var booking = await _context.Bookings.FindAsync(bookingID);
        if (booking == null)
        {
            return NotFound(new { message = "Booking not found" });
        }
        long totalPriceCents = (long)(booking.TotalPrice * 100);


        var domain = "http://localhost:3000";
        var options = new SessionCreateOptions
        {
            UiMode = "embedded",
            LineItems = new List<SessionLineItemOptions>
{
    new SessionLineItemOptions
            {
                PriceData= new SessionLineItemPriceDataOptions{
                    Currency="sek",
                    ProductData=new SessionLineItemPriceDataProductDataOptions{
Name = "Hotel Booking ID: " + bookingID
                    },
                    UnitAmount = totalPriceCents,
                },
                Quantity = 1,
            },
},
            Mode = "payment",
            ReturnUrl = domain + "/Hotels",
        };
        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return Ok(new { clientSecret = session.ClientSecret });
    }

}

[Route("session-status")]
[ApiController]
public class SessionStatusController : Controller
{
    [HttpGet]
    public ActionResult SessionStatus([FromQuery] string session_id)
    {
        var sessionService = new SessionService();
        Session session = sessionService.Get(session_id);

        return Json(new { status = session.Status, customer_email = session.CustomerDetails.Email });
    }
}
