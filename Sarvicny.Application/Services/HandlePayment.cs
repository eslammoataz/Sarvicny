using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.CustomerSpecification;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services
{
    public class HandlePayment : IHandlePayment
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;

        public HandlePayment(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
        }

        public async Task<Response<object>> validateOrder(string orderId, bool transactionStatus)
        {
            #region Validation_Data
            var spec = new OrderWithRequestsSpecification(orderId);
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

            if (!transactionStatus)
            {
                order.ServiceRequests.ForEach(sr => sr.Slot.enable = false);
                await _orderRepository.ChangeToPaid(order); // change order paid status
                customer.Cart.ServiceRequests = null; // empty cart of the customer
                _unitOfWork.Commit();
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
                order.OrderStatusID = OrderStatusEnum.Canceled.ToString(); //value (status name)=cancelled
                _unitOfWork.Commit();
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order is cancelled",
                    Payload = null,
                    isError = true

                };
            }
        }
    }
}
