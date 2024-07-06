using Microsoft.Extensions.Logging;
using Sarvicny.Application.Common.Helper;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Paymob;
using Sarvicny.Application.Services.Paypal;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;

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
        private readonly IEmailService _emailService;
        public PaymentService(ILogger<PaymentService> logger, IPaymobPaymentService paymobPaymentService, IPaypalPaymentService paypalPaymentService, IUnitOfWork unitOfWork, IServiceProviderService providerService, ITransactionPaymentRepository transactionPaymentRepository,IEmailService emailService)
        {
            _logger = logger;
            _paymobPaymentService = paymobPaymentService;
            _paypalPaymentService = paypalPaymentService;
            _unitOfWork = unitOfWork;
            _providerService = providerService;
            _transactionPaymentRepository = transactionPaymentRepository;
            _emailService = emailService;
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

        public async Task<Response<object>> RefundAffectedTransaction(string transactionPaymentId)
        {
            var spec = new TransactionPaymentWithDetailsSpecification(transactionPaymentId);
            var transactionPayment = await _transactionPaymentRepository.GetTransactionPaymentByIdAsync(spec);
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

            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod))
            {
                return new Response<object>()
                {
                    isError = true,
                    Message = "Invalid Payment Method",
                    Payload = null
                };
            }

            var canceledOrders = transactionPayment.OrderList
                .Where(o => o.OrderStatus == OrderStatusEnum.Canceled)
                .ToList();

            decimal totalCanceledOrdersPrice = canceledOrders.Sum(o=>o.OrderDetails.Price);

            decimal refundAmount = 0;

            foreach (var order in canceledOrders)
            {
                TimeSpan timeUntilRequest = (TimeSpan)(order.OrderDetails.RequestedSlot.RequestedDay - order.CancelDate);

                if (timeUntilRequest.TotalDays >= 2)
                {
                    refundAmount += totalCanceledOrdersPrice * 1.00m; // 100% refund
                }
                else if (timeUntilRequest.TotalDays >= 1)
                {
                    refundAmount += totalCanceledOrdersPrice * 0.50m; // 50% refund
                }
                else
                {
                    refundAmount = 0; // No refund

                    var orderDetailsForCustomer = HelperMethods.GenerateOrderDetailsMessageForCustomer(order);
                    var message = new EmailDto(order.Customer.Email!, "Sarvicny: Refund Canceled", $"Unfortunately! Your Refund is Canceled in this order: \n\nOrder Details:\n{orderDetailsForCustomer}, due to the cancelation in the same day of the request");
                    _emailService.SendEmail(message);

                }
   
            }
            var otherRefundableOrders = transactionPayment.OrderList
            .Where(o => o.OrderStatus == OrderStatusEnum.ReAssigned || o.OrderStatus == OrderStatusEnum.RemovedWithRefund)
            .ToList();

            decimal totalRefundableOrdersPrice = otherRefundableOrders.Sum(o => o.OrderDetails.Price);

            refundAmount += totalRefundableOrdersPrice;
           

            if (paymentMethod == PaymentMethod.Paymob)
                return await _paymobPaymentService.Refund(transactionPayment, refundAmount);
            else if (paymentMethod == PaymentMethod.Paypal)
                return await _paypalPaymentService.Refund(transactionPayment, refundAmount);

            _unitOfWork.Commit();
            return new Response<object>()
            {
                isError = true,
                Message = "Error Inside Refund",
                Errors = new List<string> { "Error Inside Refund" },
                Payload = null
            };


        }
    }
}
