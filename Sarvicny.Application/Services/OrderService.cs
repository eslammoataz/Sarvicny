using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private IUnitOfWork _unitOfWork;
        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<object>> AddCustomerRating(CustomerRating customerRating)
        {
            var spec = new ServiceRequest_RatingSpecification(customerRating.ServiceRequestID);
            var request = await _orderRepository.GetServiceRequestByID(spec);

            if (request == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "ServiceRequest Not Found",
                    Payload = null,
                    isError = true

                };
            }

            var rates = await _orderRepository.GetAllCustomerRating();
            if (rates.Any(s => s.ServiceRequestID == customerRating.ServiceRequestID))
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Dublicate Rate",
                    Payload = null,
                    isError = true

                };
            }

            customerRating.customerID = request.order.CustomerID;
            customerRating.OrderID = request.OrderId;

            var rating = await _orderRepository.AddCustomerRating(customerRating);

            request.customerRatingId = rating.RatingId;
            request.CRate = rating;

            _unitOfWork.Commit();
            var ratingAsObj = new
            {
                rating.RatingId,
                rating.ServiceRequestID,
                rating.OrderID,
                rating.customerID,
                rating.Rating,
                rating.Comment


            };

            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully ",
                Payload = ratingAsObj,
                isError = false

            };
        }

        public async Task<Response<object>> AddProviderRating(ProviderRating providerRating)
        {
            var spec = new ServiceRequest_RatingSpecification(providerRating.ServiceRequestID);
            var request = await _orderRepository.GetServiceRequestByID(spec);

            if (request == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "ServiceRequest Not Found",
                    Payload = null,
                    isError = true

                };
            }

            var rates = await _orderRepository.GetAllProviderRating();
            if (rates.Any(s => s.ServiceRequestID == providerRating.ServiceRequestID))
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Dublicate Rate",
                    Payload = null,
                    isError = true

                };
            }

            providerRating.providerId = request.providerService.ProviderID;
            providerRating.orderId = request.OrderId;

            var rating = await _orderRepository.AddProviderRating(providerRating);

            request.providerRatingId = rating.RatingId;
            request.PRate = rating;

            _unitOfWork.Commit();
            var ratingAsObj = new
            {
                rating.RatingId,
                rating.ServiceRequestID,
                rating.orderId,
                rating.providerId,
                rating.Rating,
                rating.Comment


            };

            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully ",
                Payload = ratingAsObj,
                isError = false

            };
        }

        public async Task<Response<object>> ShowAllOrderDetails(string orderId)
        {
            var spec = new OrderWithRequestsSpecification(orderId);
            var order = await _orderRepository.GetOrder(spec);
            if (order == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order Not Found",
                    Payload = null,
                    isError = true

                };
            }

            var customer = order.Customer;
            var orderAsObject = new
            {
                orderId = order.OrderID,
                customerId = order.CustomerID,
                customerFN = customer.FirstName,
                orderStatus = order.OrderStatus,
                orderDate = order.OrderDate,

                orderPrice = order.TotalPrice,
                orderService = order.ServiceRequests.Select(s => new
                {
                    s.providerService.Provider.Id,
                    s.providerService.Provider.FirstName,
                    s.providerService.Provider.LastName,
                    s.providerService.Service.ServiceID,
                    s.providerService.Service.ServiceName,
                    s.providerService.Service.ParentServiceID,
                    parentServiceName = s.providerService.Service.ParentService?.ServiceName,
                    s.providerService.Service.CriteriaID,
                    s.providerService.Service.Criteria?.CriteriaName,
                    s.SlotID,
                    s.Slot.StartTime,
                    s.providerDistrict.DistrictID,
                    s.providerDistrict.District.DistrictName,

                    s.Price,
                    s.ProblemDescription
                }).ToList<object>(),
            };

            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = orderAsObject
            };
        }  //feha tfasel provider

        public async Task<Response<object>> ShowAllOrderDetailsForCustomer(string orderId)
        {
            var spec = new OrderWithRequestsSpecification(orderId);
            var order = await _orderRepository.GetOrder(spec);
            if (order == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order Not Found",
                    Payload = null,
                    isError = true

                };
            }

            var orderAsObject = new
            {
                orderId = order.OrderID,
                orderStatus = order.OrderStatus,
                orderPrice = order.TotalPrice,
                orderDate = order.OrderDate,
                orderService = order.ServiceRequests.Select(s => new
                {
                    s.providerService.Provider.Id,
                    s.providerService.Provider.FirstName,
                    s.providerService.Provider.LastName,
                    s.providerService.Service.ServiceID,
                    s.providerService.Service.ServiceName,
                    s.providerService.Service.ParentServiceID,
                    parentServiceName = s.providerService.Service.ParentService?.ServiceName,
                    s.providerService.Service.CriteriaID,
                    s.providerService.Service.Criteria?.CriteriaName,
                    s.SlotID,
                    s.Slot.StartTime,
                    s.providerDistrict.DistrictID,
                    s.providerDistrict.District.DistrictName,
                    s.Price,
                    s.ProblemDescription
                }).ToList<object>(),
            };

            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = orderAsObject
            };
        } //mfhash customer



        public async Task<Response<object>> ShowOrderDetailsForProvider(string orderId) //feha tfasel customer
        {
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
                };
            }

            var customer = order.Customer;
            var orderAsObject = new
            {
                orderId = order.OrderID,
                customerId = order.CustomerID,
                customerFN = customer.FirstName,
                customerLN = customer.LastName,
                Address = customer.Address,


                orderStatus = order.OrderStatus,
                orderPrice = order.TotalPrice,
                orderDate = order.OrderDate,
                orderService = order.ServiceRequests.Select(s => new
                {

                    s.providerService.Service.ServiceID,
                    s.providerService.Service.ServiceName,
                    s.providerService.Service.ParentServiceID,
                    parentServiceName = s.providerService.Service.ParentService?.ServiceName,
                    s.providerService.Service.CriteriaID,
                    s.providerService.Service.Criteria?.CriteriaName,
                    s.SlotID,
                    s.Slot.StartTime,
                    s.providerDistrict.DistrictID,
                    s.providerDistrict.District.DistrictName,
                    s.Price,
                    s.ProblemDescription
                }).ToList<object>(),
            };

            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = orderAsObject
            };
        }

        public async Task<Response<object>> ShowOrderStatus(string orderId)
        {
            var spec = new OrderWithRequestsSpecification(orderId);

            var order = await _orderRepository.GetOrder(spec);
            if (order == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order Not Found",
                    Payload = null
                };

            }
            var status = order.OrderStatus;

            return new Response<object>()
            {
                Status = "succes",
                Message = "success",
                Payload = status,
                isError = false
            };

        }
    }
}


