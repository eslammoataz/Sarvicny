using Microsoft.VisualBasic;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceProviderRepository _providerRepository;
        private readonly IEmailService _emailService;
        private readonly IServiceRepository _serviceRepository;
        private IUnitOfWork _unitOfWork;
        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository, IEmailService emailService, IServiceRepository serviceRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _providerRepository = serviceProviderRepository;
            _emailService = emailService;
            _serviceRepository = serviceRepository;

        }

        public async Task<Response<object>> AddCustomerRating(RatingDto ratingDto, string orderID)
        {
            var spec = new OrderWithDetailsSpecification(orderID);
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
            if (order.OrderStatus != OrderStatusEnum.Completed)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order Status is not Completed",
                    Payload = null,
                    isError = true

                };
            }
            var rate = order.CRate;
            if (rate != null)
            {
                return new Response<object>()
                {
                    Status = "Failed",
                    Message = " Order is already rated ",
                    Payload = null,
                    isError = true

                };
            }

            var customerRate = new OrderRating
            {
                Order = order,
                OrderID = orderID,
                Rate = ratingDto.Rate,
                Comment = ratingDto.Comment,

            };


            var rating = await _orderRepository.AddRating(customerRate);

            order.customerRatingId = rating.RatingId;
            order.CRate = rating;

            _unitOfWork.Commit();
            var ratingAsObj = new
            {
                rating.RatingId,
                rating.OrderID,
                rating.Rate,
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

        public async Task<Response<object>> AddProviderRating(RatingDto ratingDto, string orderID)
        {
            var spec = new OrderWithDetailsSpecification(orderID);
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
            if (order.OrderStatus != OrderStatusEnum.Done || order.OrderStatus != OrderStatusEnum.Completed)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order Status is not Completed",
                    Payload = null,
                    isError = true

                };
            }
            var rate = order.PRate;
            if (rate != null)
            {
                return new Response<object>()
                {
                    Status = "Failed",
                    Message = " Order is already rated ",
                    Payload = null,
                    isError = true

                };
            }

            var providerRate = new OrderRating
            {
                Order = order,
                OrderID = orderID,
                Rate = ratingDto.Rate,
                Comment = ratingDto.Comment,

            };


            var rating = await _orderRepository.AddRating(providerRate);

            order.providerRatingId = rating.RatingId;
            order.PRate = rating;

            _unitOfWork.Commit();
            var ratingAsObj = new
            {
                rating.RatingId,
                rating.OrderID,
                rating.Rate,
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

        public async Task<Response<object>> GetCustomerRatingForOrder(string orderID)
        {
            var spec = new OrderWithDetailsSpecification(orderID);
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
            var CRate = order.CRate;

            var rateAsobject = new
            {
                orderID = orderID,
                OrderRateId = CRate.RatingId,
                Rate = CRate.Rate,
                Comment = CRate.Comment,
            };

            return new Response<object>()
            {
                Status = "success",
                Message = "Action done",
                Payload = rateAsobject,
                isError = false

            };

        }

        public async Task<Response<object>> GetProviderRatingForOrder(string orderID)
        {
            var spec = new OrderWithDetailsSpecification(orderID);
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
            var PRate = order.PRate;

            var rateAsobject = new
            {
                orderID = orderID,
                OrderRateId = PRate.RatingId,
                Rate = PRate.Rate,
                Comment = PRate.Comment,
            };

            return new Response<object>()
            {
                Status = "success",
                Message = "Action done",
                Payload = rateAsobject,
                isError = false

            };

        }

        public async Task<Response<object>> ShowAllOrderDetailsForAdmin(string orderId)
        {
            var spec = new OrderWithDetailsSpecification(orderId);
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
            var provider = order.OrderDetails.Provider;
            var services = order.OrderDetails.RequestedServices;

            var orderAsobject = new
            {
                orderId = order.OrderID,
                orderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                //PaymentExpirytime = order.PaymentExpiryTime,


                customerId = order.CustomerID,
                customerFN = customer.FirstName,
                customerLastName = customer.LastName,

                providerId = provider.Id,
                providerFN = provider.FirstName,
                providerLN = provider.LastName,

                orderPrice = order.OrderDetails.Price,

                orderService = services.Select(s => new
                {

                    s.ServiceId,
                    s.Service.ServiceName,
                    s.Service.ParentServiceID,
                    parentServiceName = s.Service.ParentService?.ServiceName,
                    s.Service.CriteriaID,
                    s.Service.Criteria?.CriteriaName,
                    Price = Math.Ceiling((s.Service.ProviderServices.FirstOrDefault()?.Price ?? 0) * 1.12m),

                }).ToList<object>(),

                RequestedSlotID = order.OrderDetails.RequestedSlotID,
                RequestedDay = order.OrderDetails.RequestedSlot.RequestedDay,
                DayOfWeek = order.OrderDetails.RequestedSlot.DayOfWeek,
                StartTime = order.OrderDetails.RequestedSlot.StartTime,

                DistrictID = order.OrderDetails.providerDistrict.DistrictID,
                DistrictName = order.OrderDetails.providerDistrict.District.DistrictName,
                Address = order.OrderDetails.Address,
                Price = order.OrderDetails.Price,
                Problem = order.OrderDetails.ProblemDescription,

                providerRating = order.PRate.Rate,
                providerComment = order.PRate.Comment,
                customerRating = order.CRate.Rate,
                customerComment = order.CRate.Comment,
            };

            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = orderAsobject
            };
        }  //feha tfasel provider

        public async Task<object> ShowAllOrderDetailsForCustomer(string orderId)
        {
            var spec = new OrderWithDetailsSpecification(orderId);
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

            var provider = order.OrderDetails.Provider;

            var services = order.OrderDetails.RequestedServices;
            ;
            var orderAsObject = new
            {
                orderId = order.OrderID,
                orderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,

                providerId = provider.Id,
                providerFN = provider.FirstName,
                providerLN = provider.LastName,

                orderPrice = order.OrderDetails.Price,

                orderService = services.Select(s => new
                {

                    s.ServiceId,
                    s.Service.ServiceName,
                    s.Service.ParentServiceID,
                    parentServiceName = s.Service.ParentService?.ServiceName,
                    s.Service.CriteriaID,
                    s.Service.Criteria?.CriteriaName,
                    Price = Math.Ceiling((s.Service.ProviderServices.FirstOrDefault()?.Price ?? 0) * 1.12m),

                }).ToList<object>(),

                RequestedSlotID = order.OrderDetails.RequestedSlotID,
                RequestedDay = order.OrderDetails.RequestedSlot.RequestedDay,
                DayOfWeek = order.OrderDetails.RequestedSlot.DayOfWeek,
                StartTime = order.OrderDetails.RequestedSlot.StartTime,

                DistrictID = order.OrderDetails.providerDistrict.DistrictID,
                DistrictName = order.OrderDetails.providerDistrict.District.DistrictName,
                Address = order.OrderDetails.Address,
                Price = order.OrderDetails.Price,
                Problem = order.OrderDetails.ProblemDescription,

                providerRating = order.PRate.Rate,
                providerComment = order.PRate.Comment,
                customerRating = order.CRate.Rate,
                customerComment = order.CRate.Comment,
            };

            return orderAsObject;
        } //mfhash customer


        public async Task<Response<object>> ShowAllOrderDetailsForProvider(string orderId) //feha tfasel customer
        {
            var spec = new OrderWithDetailsSpecification(orderId);
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

            var services = order.OrderDetails.RequestedServices;
    
            var orderAsObject = new
            {
                orderId = order.OrderID,
                orderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,

                customerId = order.CustomerID,
                customerFN = customer.FirstName,
                customerLastName = customer.LastName,

                orderPrice = order.OrderDetails.Price,

                orderService = services.Select(s => new
                {

                    s.ServiceId,
                    s.Service.ServiceName,
                    s.Service.ParentServiceID,
                    parentServiceName = s.Service.ParentService?.ServiceName,
                    s.Service.CriteriaID,
                    s.Service.Criteria?.CriteriaName,
                    Price = Math.Ceiling((s.Service.ProviderServices.FirstOrDefault()?.Price ?? 0) * 1.12m),

                }).ToList<object>(),

                RequestedSlotID = order.OrderDetails.RequestedSlotID,
                RequestedDay = order.OrderDetails.RequestedSlot.RequestedDay,
                DayOfWeek = order.OrderDetails.RequestedSlot.DayOfWeek,
                StartTime = order.OrderDetails.RequestedSlot.StartTime,

                DistrictID = order.OrderDetails.providerDistrict.DistrictID,
                DistrictName = order.OrderDetails.providerDistrict.District.DistrictName,
                Address = order.OrderDetails.Address,

                Problem = order.OrderDetails.ProblemDescription,
                profit = Math.Ceiling(order.OrderDetails.Price / (1 + 0.12m))
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
            var spec = new BaseSpecifications<Order>(or => or.OrderID == orderId);

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


        public Task<Response<object>> Refund(string orderId)
        {
            throw new NotImplementedException();
        }
        public async Task<Response<List<object>>> GetAllMatchedProviderSortedbyFav(MatchingProviderDto matchingProviderDto)
        {

            foreach (var Id in matchingProviderDto.services)
            {
                var serviceSpec = new BaseSpecifications<Service>(s => s.ServiceID == Id);
                var service = await _serviceRepository.GetServiceById(serviceSpec);

                if (service == null)
                    return new Response<List<object>>
                    {
                        isError = true,
                        Errors = new List<string> { "Service Not Found" },
                        Status = "Error",
                        Message = "Failed",
                    };
                if (service.ParentServiceID == null)
                    return new Response<List<object>>
                    {
                        isError = true,
                        Errors = new List<string> { "Service Not Found" },
                        Status = "Error",
                        Message = "Failed",
                    };
            }
            TimeSpan startTime = TimeSpan.Parse(matchingProviderDto.startTime);

            var MatchedProviderSortedbyFav = await _providerRepository.GetAllMatchedProviders(matchingProviderDto.services, startTime, matchingProviderDto.dayOfWeek, matchingProviderDto.districtId, matchingProviderDto.customerId);
            if (MatchedProviderSortedbyFav == null)
            {
                return new Response<List<object>>
                {
                    isError = true,
                    Errors = new List<string> { " No MatchedProvider Not Found" },
                    Status = "Error",
                    Message = "Failed",
                };

            }

            List<object> result = new List<object>();
            foreach (var match in MatchedProviderSortedbyFav)
            {
                var spec = new ProviderWithDetailsSpecification(match.Id);
                var provider = await _providerRepository.FindByIdAsync(spec);
                var availability = provider.Availabilities.FirstOrDefault(a => a.DayOfWeek == matchingProviderDto.dayOfWeek);
                var slot = availability.Slots.FirstOrDefault(s => s.StartTime == startTime);


                decimal rate = 0.12m;


                var providerAsObj = new
                {
                    providerId = provider.Id,
                    firstname = provider.FirstName,
                    lastname = provider.LastName,
                    email = provider.Email,
                    slotId = slot.TimeSlotID,
                    services = provider.ProviderServices
                        .Where(ps => matchingProviderDto.services.Contains(ps.ServiceID))
                        .Select(ps => new
                        {
                            ps.ServiceID,
                            price = Math.Ceiling(ps.Price + (ps.Price * rate))

                        }).ToList(),


                };
                result.Add(providerAsObj);
            }

            return new Response<List<object>>
            {
                isError = false,
                Errors = null,
                Status = "Success",
                Payload = result,
                Message = "Action done Successfully",
            };

        }




        public async Task<Response<List<object>>> SuggestNewProvidersIfNoMatchesFoundLevel1(MatchingProviderDto matchingProviderDto)
        {
            foreach (var Id in matchingProviderDto.services)
            {
                var serviceSpec = new BaseSpecifications<Service>(s => s.ServiceID == Id);
                var service = await _serviceRepository.GetServiceById(serviceSpec);
                if (service == null)
                    return new Response<List<object>>
                    {
                        isError = true,
                        Errors = new List<string> { "Service Not Found" },
                        Status = "Error",
                        Message = "Failed",
                    };
                if (service.ParentServiceID == null)
                    return new Response<List<object>>
                    {
                        isError = true,
                        Errors = new List<string> { "Service Not Found" },
                        Status = "Error",
                        Message = "Failed",
                    };

            }

            var Suggestions = await _providerRepository.SuggestionLevel1(matchingProviderDto.services, matchingProviderDto.dayOfWeek, matchingProviderDto.districtId, matchingProviderDto.customerId);
            if (Suggestions == null)
            {

                return new Response<List<object>>
                {
                    isError = true,
                    Message = "No MatchedProvider in This day of week",
                    Payload = null,

                };

            }

            List<object> result = new List<object>();
            foreach (var match in Suggestions)
            {
                var spec = new ProviderWithDetailsSpecification(match.Id);
                var provider = await _providerRepository.FindByIdAsync(spec);
                var availability = provider.Availabilities.FirstOrDefault();

                decimal rate = 0.12m;


                var providerAsObj = new
                {
                    providerId = provider.Id,
                    firstname = provider.FirstName,
                    lastname = provider.LastName,
                    email = provider.Email,
                    availabilities = availability.Slots.Where(s=>s.isActive==true).Select(s => new
                    {
                        s.TimeSlotID,
                        s.StartTime
                    }).ToList(),
                    services = provider.ProviderServices
                        .Where(ps => matchingProviderDto.services.Contains(ps.ServiceID))
                        .Select(ps => new
                        {
                            ps.ServiceID,
                            price = Math.Ceiling(ps.Price + (ps.Price * rate))

                        }).ToList(),



                };
                result.Add(providerAsObj);
            }

            return new Response<List<object>>
            {
                isError = false,
                Errors = null,
                Status = "Success",
                Payload = result,
                Message = "Action done Successfully",
            };
        }
        public async Task<Response<List<object>>> SuggestNewProvidersIfNoMatchesFoundLevel2(MatchingProviderDto matchingProviderDto)
        {
            foreach (var Id in matchingProviderDto.services)
            {
                var serviceSpec = new BaseSpecifications<Service>(s => s.ServiceID == Id);
                var service = await _serviceRepository.GetServiceById(serviceSpec);
                if (service == null)
                    return new Response<List<object>>
                    {
                        isError = true,
                        Errors = new List<string> { "Service Not Found" },
                        Status = "Error",
                        Message = "Failed",
                    };
                if (service.ParentServiceID == null)
                    return new Response<List<object>>
                    {
                        isError = true,
                        Errors = new List<string> { "Service Not Found" },
                        Status = "Error",
                        Message = "Failed",
                    };

            }

            var Suggestions = await _providerRepository.SuggestionLevel2(matchingProviderDto.services, matchingProviderDto.districtId, matchingProviderDto.customerId);
            if (Suggestions == null)
            {

                return new Response<List<object>>
                {
                    isError = true,
                    Message = "No MatchedProvider for your Requirments (district or combination of services)",
                    Payload = null,

                };

            }

            List<object> result = new List<object>();
            foreach (var match in Suggestions)
            {
                var spec = new ProviderWithDetailsSpecification(match.Id);
                var provider = await _providerRepository.FindByIdAsync(spec);


                decimal rate = 0.12m;


                var availabilities = provider.Availabilities.Select(a => a.Slots.Where(s => s.isActive == true).Select(s => new
                {
                    s.TimeSlotID,
                    s.StartTime,
                    s.ProviderAvailability.DayOfWeek
                }
                 )).ToList();


                var providerAsObj = new
                {
                    providerId = provider.Id,
                    firstname = provider.FirstName,
                    lastname = provider.LastName,
                    email = provider.Email,
                    avalability = availabilities,
                    services = provider.ProviderServices
                        .Where(ps => matchingProviderDto.services.Contains(ps.ServiceID))
                        .Select(ps => new
                        {
                            ps.ServiceID,
                            price = Math.Ceiling(ps.Price + (ps.Price * rate))

                        }).ToList(),


                };
                result.Add(providerAsObj);
            }

            return new Response<List<object>>
            {
                isError = false,
                Errors = null,
                Status = "Success",
                Payload = result,
                Message = "Action done Successfully",
            };
        }
    }
}


