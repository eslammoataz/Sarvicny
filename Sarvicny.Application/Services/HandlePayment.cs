using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.CustomerSpecification;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Emails;
using static Sarvicny.Domain.Entities.OrderDetails;

namespace Sarvicny.Application.Services
{
    public class HandlePayment : IHandlePayment
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderService _orderService;
        private readonly IServiceProviderService _providerService;
        private readonly IEmailService _emailService;

        public HandlePayment(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ICustomerRepository customerRepository,IOrderService orderService,IEmailService emailService, IServiceProviderService providerService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _orderService = orderService;
            _emailService = emailService;
            _providerService = providerService;


        }

        public async Task<Response<object>> validateOrder(string orderId, bool transactionStatus,
            string transactionID, PaymentMethod paymentMethod)
        {

            #region Validation_Data
            var spec = new OrderWithDetailsSpecification(orderId);
            var order = await _orderRepository.GetOrder(spec);
            if (order == null)
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

            var customer = await _customerRepository.GetCustomerById(new CustomerWithCartSpecification(order.CustomerID));

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
                

                order.OrderStatus = OrderStatusEnum.Paid;
                

                order.PaymentExpiryTime = null;

                // change order paid status
                await _orderRepository.ChangeOrderPaidStatus(order, transactionID, paymentMethod, transactionStatus);

              
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
                order.OrderStatus = OrderStatusEnum.Removed;

                var originalSlot = await _providerService.getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
                if (originalSlot != null)
                {
                    originalSlot.isActive = true;
                }

                // change order Cancelled and saving transaction id and payment method
                await _orderRepository.ChangeOrderPaidStatus(order, transactionID, paymentMethod, transactionStatus);


                var orderDetailsForCustomer = _orderService.GenerateOrderDetailsMessage(order);
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
