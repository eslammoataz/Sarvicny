using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

      

        public async Task<Response<object>> AddRatingCustomer(OrderRating rating)
        {
            var spec = new ServiceRequest_RatingSpecification(rating.serviceRequestID);
            var Request  = await _orderRepository.GetServiceRequestByID(spec);
            if(Request == null) {


                return new Response<object>()
                {
                    Status = "Failed",
                    Message = " Request Not Found ",
                    Payload = null
                };

            }
            var rates = await _orderRepository.GetAllOrderRating();

           



            if (rating.CustomerId == null )
            {
                rating.CustomerId = Request.order.CustomerID;
                

                if (rates.Any(s => s.serviceRequestID == rating.serviceRequestID))
                {
                    _unitOfWork.Commit();

                }

                rating.OrderId = Request.order.OrderID;

                var Rating = await _orderRepository.AddCustomerRating(rating);
                _unitOfWork.Commit();
                if (Rating == null)
                {

                    return new Response<object>()
                    {
                        Status = "Failed",
                        isError = true,
                        Message = "Action Failed ",
                        Payload = null
                    };


                }

                var ratingAsobject = new
                {
                    ServiceRequest = rating.serviceRequestID,
                    CustomerRate = rating.customerRating,
                    FeedBack = rating.Comment
                };
                return new Response<object>()
                {
                    Status = "Success",
                    isError = false,
                    Message = "Action Done Successfully",
                    Payload = ratingAsobject
                };
            }
             return new Response<object>()
                {
                    Status = "failed",
                    isError = true,
                    Message = "Dublicate Record",
                    Payload = null
                };
        }

        public async Task<Response<object>> AddRatingServiceProvider(OrderRating rating)
        {
            var spec = new ServiceRequest_RatingSpecification(rating.serviceRequestID);
            var Request = await _orderRepository.GetServiceRequestByID(spec);
            if (Request == null)
            {

                return new Response<object>()
                {
                    Status = "Failed",
                    Message = " Request Not Found ",
                    isError = true,
                    Payload = null
                };



            }

            var rates = await _orderRepository.GetAllOrderRating();



            if (rating.ProviderId == null)
            {
                rating.ProviderId = Request.providerService.ProviderID;


                if (rates.Any(s => s.serviceRequestID == rating.serviceRequestID))
                {
                    var ProviderRate = new OrderRating
                    {
                        ProviderId = rating.ProviderId,    
                        CustomerId = Request.order.CustomerID,
                        customerRating = rating.customerRating,
                        ServiceProviderRating = rating.ServiceProviderRating,
                        Comment = rating.Comment

                    };
                     await _orderRepository.AddServiceProviderRating(ProviderRate);
                    await _orderRepository.RemoveRating(rating);

                    _unitOfWork.Commit();

                }


                
                rating.OrderId = Request.order.OrderID;

                var Rating = await _orderRepository.AddServiceProviderRating(rating);
                _unitOfWork.Commit();
                if (Rating == null)
                {
                    return new Response<object>()
                    {
                        Status = "Failed",
                        isError = true,
                        Message = "Action Failed ",
                        Payload = null
                    };


                }

                var ratingAsobject = new
                {
                    ServiceRequest = rating.serviceRequestID,
                    ProviderRate = rating.ServiceProviderRating,
                    FeedBack = rating.Comment
                };
                return new Response<object>()
                {
                    Status = "Success",
                    isError = false,
                    Message = "Action Done Successfully",
                    Payload = ratingAsobject
                }; }

            return new Response<object>()
            {
                Status = "failed",
                isError = true,
                Message = "Dublicate Record",
                Payload = null
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
                orderStatus = order.OrderStatus.StatusName,
                orderDate= order.OrderDate,
                
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
                orderStatus = order.OrderStatus.StatusName,
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
                    isError=true,
                };
            }

            var customer = order.Customer;
            var orderAsObject = new
            {
                orderId = order.OrderID,
                customerId = order.CustomerID,
                customerFN = customer.FirstName,
                customerLN = customer.LastName,
                Address=customer.Address,


                orderStatus = order.OrderStatus.StatusName,
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
            var status = order.OrderStatus.StatusName;

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


