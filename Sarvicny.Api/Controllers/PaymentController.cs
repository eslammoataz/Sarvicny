using Microsoft.AspNetCore.Mvc;
using Sarvicny.Application.Services.Abstractions;

namespace Sarvicny.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController :ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> ProcessPayment()
    {
        try
        {
            var token = await _paymentService.GetAuthToken();
            return Ok(token);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, "An error occurred while processing payment");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> RegisterOrder()
    {
        try
        {
            var order = await _paymentService.OrderRegistration();
            return Ok(order);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, "An error occurred while registering order");
        }
    }
    
    


    
}