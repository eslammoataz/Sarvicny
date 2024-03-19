using Microsoft.Extensions.Logging;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Paymob;
using Sarvicny.Application.Services.Paypal;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymobPaymentService _paymobPaymentService;
        private readonly IPaypalPaymentService _paypalPaymentService;

        public PaymentService(ILogger<PaymentService> logger, IPaymobPaymentService paymobPaymentService, IPaypalPaymentService paypalPaymentService)
        {
            _logger = logger;
            _paymobPaymentService = paymobPaymentService;
            _paypalPaymentService = paypalPaymentService;
        }
        public async Task<Response<object>> PayOrder(Order order, PaymentMethod paymentMethod)
        {
            _logger.LogInformation("PaymentService.PayOrder");

            // Check if paymentMethod is a valid enum value
            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod))
            {
                return new Response<object>()
                {
                    isError = true,
                    Message = "Invalid Payment Method",
                    Payload = null
                };
            }

            if (paymentMethod == PaymentMethod.Paymob)
            {
                return await _paymobPaymentService.Pay(order);
            }
            else if (paymentMethod == PaymentMethod.Paypal)
            {
                return await _paypalPaymentService.Pay(order);
            }

            return new Response<object>()
            {
                isError = true,
                Message = "Error Inside PayOrder",
                Errors = new List<string> { "Error Inside PayOrder" },
                Payload = null
            };

        }
    }
}
