using Sarvicny.Application.Common.Helper;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Emails;

namespace Sarvicny.Application.Services
{
    public class HandlePayment : IHandlePayment
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServiceProviderService _providerService;
        private readonly IEmailService _emailService;
        private readonly ITransactionPaymentRepository _transactionPaymentRepository;


        public HandlePayment(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IEmailService emailService, IServiceProviderService providerService, ITransactionPaymentRepository transactionPaymentRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _emailService = emailService;
            _providerService = providerService;
            _transactionPaymentRepository = transactionPaymentRepository;
        }

        public async Task<Response<object>> validateOrder(string transactionPaymentId, bool transactionStatus,
            string transactionID, string saleId, PaymentMethod paymentMethod)
        {

            #region Validation_Data
            var transactionPayment = await _transactionPaymentRepository.GetTransactionPaymentAsync(transactionPaymentId);

            if (transactionPayment == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order Not Found",
                    Payload = null,
                    isError = true,
                    Errors = new List<string> { "Order Not Found" }

                };
            }

            var customer = await _transactionPaymentRepository.GetCustomerByTransactionPaymentId(transactionPaymentId);

            //var customer = await _customerRepository.GetCustomerById(new CustomerWithCartSpecification(order.CustomerID));

            if (customer == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Customer Not Found",
                    Payload = null,
                    isError = true,
                    Errors = new List<string> { "Customer Not Found" }

                };
            }

            #endregion

            if (transactionStatus)
            {

                transactionPayment.TransactionPaymentStatus = TransactionPaymentStatusEnum.Success;
                transactionPayment.TransactionID = transactionID;
                transactionPayment.SaleID = saleId;
                transactionPayment.PaymentMethod = paymentMethod;
                transactionPayment.PaymentDate = DateTime.UtcNow;
                transactionPayment.PaymentExpiryTime = null;

                var orders = transactionPayment.OrderList;
                foreach (var order in orders)
                {
                    order.OrderStatus = OrderStatusEnum.Paid;
                    order.IsPaid = true;
                    var requestedSlot = order.OrderDetails.RequestedSlot;
                    var originalSlot = await _providerService.getOriginalSlot(requestedSlot, order.OrderDetails.ProviderID);
                    if (originalSlot != null)
                    {
                        originalSlot.isActive = false;
                    }
                }

                //order.PaymentExpiryTime = null;

                // change order paid status
                //await _orderRepository.ChangeOrderPaidStatus(order, transactionID, saleId, paymentMethod, transactionStatus);


                try
                {
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    return new Response<object>()
                    {
                        Status = "failed",
                        Message = "Error while saving data",
                        Payload = null,
                        isError = true,
                        Errors = new List<string> { ex.Message }

                    };
                }

                return new Response<object>()
                {
                    Status = "success",
                    Message = "Order is paid",
                    Payload = null,
                    isError = false

                };
            }
            else
            {
                transactionPayment.TransactionPaymentStatus = TransactionPaymentStatusEnum.Failed;
                transactionPayment.TransactionID = transactionID;
                transactionPayment.SaleID = saleId;
                transactionPayment.PaymentMethod = paymentMethod;
                transactionPayment.PaymentDate = DateTime.UtcNow;
                transactionPayment.PaymentExpiryTime = null;

                var orders = transactionPayment.OrderList;
                foreach (var order in orders)
                {
                    order.OrderStatus = OrderStatusEnum.Canceled;
                    order.IsPaid = false;
                    var originalSlot = await _providerService.getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
                    if (originalSlot != null)
                    {
                        originalSlot.isActive = true;
                    }
                }


                //order.OrderStatus = OrderStatusEnum.Removed;

                //var originalSlot = await _providerService.getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
                //if (originalSlot != null)
                //{
                //    originalSlot.isActive = true;
                //}

                // change order Cancelled and saving transaction id and payment method
                //await _orderRepository.ChangeOrderPaidStatus(order, transactionID, saleId, paymentMethod, transactionStatus);


                var orderDetailsForCustomer = HelperMethods.GenerateTranasctionMessageForCustomer(transactionPayment);
                var message = new EmailDto(customer.Email!, "Sarvicny: order is removed ", $"Sorry your order is removed due to failed transaction  with payment Method. \n\nOrder Details:\n{orderDetailsForCustomer}");


                _emailService.SendEmail(message);
                _unitOfWork.Commit();
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order is Removed",
                    Payload = null,
                    isError = true

                };
            }
        }
    }
}
