using E_Commerce.BLL.DTOs.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;

    [HttpPost("create-payment-intent")]
    public IActionResult CreatePaymentIntent([FromBody] PaymentRequestDto request)
    {
        if (request.Amount <= 0)
            return BadRequest("Amount must be greater than 0");

        if (string.IsNullOrWhiteSpace(request.Currency))
            return BadRequest("Currency is required");

        var clientSecret = _paymentService.CreatePaymentIntent(request.Amount, request.Currency);
        return Ok(new { clientSecret });
    }
}

