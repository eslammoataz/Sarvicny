using Microsoft.Extensions.Logging;
using Sarvicny.Application.Common.Interfaces.Persistence;
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
        private readonly IServiceProviderService _providerService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(ILogger<PaymentService> logger, IPaymobPaymentService paymobPaymentService, IPaypalPaymentService paypalPaymentService, IUnitOfWork unitOfWork, IServiceProviderService providerService)
        {
            _logger = logger;
            _paymobPaymentService = paymobPaymentService;
            _paypalPaymentService = paypalPaymentService;
            _unitOfWork = unitOfWork;
            _providerService = providerService;
        }
        public async Task<Response<object>> PayOrder(Order order, PaymentMethod paymentMethod)
        {
            _logger.LogInformation("PaymentService.PayOrder");
            if (order.PaymentExpiryTime <= DateTime.UtcNow && order.PaymentExpiryTime != null)
            {
                order.OrderStatus = OrderStatusEnum.Removed;
                var originalSlot = await _providerService.getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
                if (originalSlot != null)
                {
                    originalSlot.isActive = true;
                }

                _unitOfWork.Commit();


                return new Response<object>()
                {
                    isError = true,
                    Message = "Payment Expiry date exceeded, order is canceled",
                    Payload = null
                };
            }

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

        public async Task<Response<object>> RefundOrder(Order order, decimal amount)
        {
            if (order.PaymentMethod == PaymentMethod.Paymob)
            {
                return await _paymobPaymentService.Refund(order, amount);
            }
            else if (order.PaymentMethod == PaymentMethod.Paypal)
            {
                return await _paypalPaymentService.Refund(order, amount);
            }
            else
            {
                return new Response<object>()
                {
                    isError = true,
                    Message = "Invalid Payment Method",
                    Payload = null
                };
            }
        }
    }
}
