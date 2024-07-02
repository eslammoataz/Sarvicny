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
        private readonly ITransactionPaymentRepository _transactionPaymentRepository;
        public PaymentService(ILogger<PaymentService> logger, IPaymobPaymentService paymobPaymentService, IPaypalPaymentService paypalPaymentService, IUnitOfWork unitOfWork, IServiceProviderService providerService, ITransactionPaymentRepository transactionPaymentRepository)
        {
            _logger = logger;
            _paymobPaymentService = paymobPaymentService;
            _paypalPaymentService = paypalPaymentService;
            _unitOfWork = unitOfWork;
            _providerService = providerService;
            _transactionPaymentRepository = transactionPaymentRepository;
        }
        public async Task<Response<object>> PayOrder(string transactionPaymentId)
        {
            var transactionPayment = await _transactionPaymentRepository.GetTransactionPaymentAsync(transactionPaymentId);
            if (transactionPayment == null)
            {
                return new Response<object>()
                {
                    isError = true,
                    Message = "Order Not Found",
                    Payload = null
                };
            }

            var paymentMethod = transactionPayment.PaymentMethod;

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
                return await _paymobPaymentService.Pay(transactionPayment);
            }
            else if (paymentMethod == PaymentMethod.Paypal)
            {
                return await _paypalPaymentService.Pay(transactionPayment);
            }

            return new Response<object>()
            {
                isError = true,
                Message = "Error Inside PayOrder",
                Errors = new List<string> { "Error Inside PayOrder" },
                Payload = null
            };

        }

        public async Task<Response<object>> RefundOrder(string transactionPaymentId, List<string> orderIds)
        {
            var transactionPayment = await _transactionPaymentRepository.GetTransactionPaymentAsync(transactionPaymentId);
            if (transactionPayment == null)
            {
                return new Response<object>()
                {
                    isError = true,
                    Message = "Order Not Found",
                    Payload = null
                };
            }

            var orders = transactionPayment.OrderList;
            var orderIdsInTransaction = orders.Select(o => o.OrderID).ToList();

            foreach (var orderId in orderIds)
            {
                if (!orderIdsInTransaction.Contains(orderId))
                {
                    return new Response<object>()
                    {
                        isError = true,
                        Message = "Invalid Order ID doesnot exist in this Transaction: " + orderId,
                        Payload = null
                    };
                }
            }

            var validOrders = orders.Where(o => orderIds.Contains(o.OrderID)).ToList();

            var paymentMethod = transactionPayment.PaymentMethod;

            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod))
            {
                return new Response<object>()
                {
                    isError = true,
                    Message = "Invalid Payment Method",
                    Payload = null
                };
            }

            decimal totalOrderPrice = orders
                                    .Where(o => o.OrderDetails != null)
                                    .Sum(o => o.OrderDetails.Price);

            decimal refundAmount = 0;

            TimeSpan timeSincePayment = DateTime.UtcNow - transactionPayment.PaymentDate.Value;

            if (timeSincePayment.TotalDays < 1)
            {
                refundAmount = totalOrderPrice * 1.00m; // 100% refund
            }
            else if (timeSincePayment.TotalDays < 2)
            {
                refundAmount = totalOrderPrice * 0.75m; // 75% refund
            }
            else if (timeSincePayment.TotalDays < 3)
            {
                refundAmount = totalOrderPrice * 0.50m; // 50% refund
            }
            else
            {
                refundAmount = 0; // No refund
            }

            if (refundAmount == 0)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Refund not allowed it passed refundable days ",
                    isError = true,
                    Payload = null
                };
            }

            if (paymentMethod == PaymentMethod.Paymob)
                return await _paymobPaymentService.Refund(transactionPayment, validOrders, refundAmount);
            else if (paymentMethod == PaymentMethod.Paypal)
                return await _paypalPaymentService.Refund(transactionPayment, validOrders, refundAmount);

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
