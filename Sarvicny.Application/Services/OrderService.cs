using MailKit.Search;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Contracts.Payment;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
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
        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository, IEmailService emailService,IServiceRepository serviceRepository) { 
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
            if(rate != null)
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

            var customerRate = new OrderRating
            {
                Order = order,
                OrderID = orderID,
                Rate = ratingDto.Rate,
                Comment = ratingDto.Comment,

            };


            var rating = await _orderRepository.AddRating(customerRate);

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

        public string GenerateOrderDetailsMessage(Order order)
        {
           
                // Construct the order details message here
                // Ensure each line ends with Environment.NewLine or \n for new lines
                return $"Order ID: {order.OrderID}{Environment.NewLine}" +
                       $"Provider: {order.OrderDetails.Provider.FirstName}{Environment.NewLine}" +
                       $"Service: {string.Join(", ", order.OrderDetails.RequestedServices.Services.Select(s => s.ServiceName))}{Environment.NewLine}" +
                       $"Requested Day: {order.OrderDetails.RequestedSlot.RequestedDay}{Environment.NewLine}" +
                       $"Day Of Week: {order.OrderDetails.RequestedSlot.DayOfWeek}{Environment.NewLine}" +
                       $"Requested Slot: {order.OrderDetails.RequestedSlot.StartTime}{Environment.NewLine}" +
                       $"Order Status: {order.OrderStatus}{Environment.NewLine}" +
                       $"Price: {order.OrderDetails.Price:C}";
            
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
            var CRate= order.CRate;

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
            var services = order.OrderDetails.RequestedServices.Services;
            var orderAsObject = new
            {
                orderId = order.OrderID,
                orderDate = order.OrderDate,
                ExpiryDate =order.ExpiryDate, 
                OrderStatus = order.OrderStatus,
                PaymentExpirytime = order.PaymentExpiryTime,


                customerId = order.CustomerID,
                customerFN = customer.FirstName,
                customerLastName = customer.LastName,

                providerId= provider.Id,
                providerFN=  provider.FirstName,
                providerLN= provider.LastName,
           
                orderPrice = order.OrderDetails.Price,
                
                orderService = services.Select(s => new
                {
                    
                    s.ServiceID,
                    s.ServiceName,
                    s.ParentServiceID,
                    parentServiceName = s.ParentService?.ServiceName,
                    s.CriteriaID,
                    s.Criteria?.CriteriaName,
                   
                }).ToList<object>(),

                RequestedSlotID=order.OrderDetails.RequestedSlotID,
                RequestedDay=order.OrderDetails.RequestedSlot.RequestedDay,
                DayOfWeek=order.OrderDetails.RequestedSlot.DayOfWeek,
                StartTime = order.OrderDetails.RequestedSlot.StartTime,

                DistrictID=order.OrderDetails.providerDistrict.DistrictID,
                DistrictName=order.OrderDetails.providerDistrict.District.DistrictName,
                Address=order.OrderDetails.Address,
                Price=order.OrderDetails.Price,
                Problem=order.OrderDetails.ProblemDescription
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
            var services = order.OrderDetails.RequestedServices.Services;
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

                    s.ServiceID,
                    s.ServiceName,
                    s.ParentServiceID,
                    parentServiceName = s.ParentService?.ServiceName,
                    s.CriteriaID,
                    s.Criteria?.CriteriaName,

                }).ToList<object>(),

                RequestedSlotID = order.OrderDetails.RequestedSlotID,
                RequestedDay = order.OrderDetails.RequestedSlot.RequestedDay,
                DayOfWeek = order.OrderDetails.RequestedSlot.DayOfWeek,
                StartTime = order.OrderDetails.RequestedSlot.StartTime,

                DistrictID = order.OrderDetails.providerDistrict.DistrictID,
                DistrictName = order.OrderDetails.providerDistrict.District.DistrictName,
                Address = order.OrderDetails.Address,
                Price = order.OrderDetails.Price,
                Problem = order.OrderDetails.ProblemDescription
            };

            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = orderAsObject
            };
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
           
            var services = order.OrderDetails.RequestedServices.Services;
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

                    s.ServiceID,
                    s.ServiceName,
                    s.ParentServiceID,
                    parentServiceName = s.ParentService?.ServiceName,
                    s.CriteriaID,
                    s.Criteria?.CriteriaName,

                }).ToList<object>(),

                RequestedSlotID = order.OrderDetails.RequestedSlotID,
                RequestedDay = order.OrderDetails.RequestedSlot.RequestedDay,
                DayOfWeek = order.OrderDetails.RequestedSlot.DayOfWeek,
                StartTime = order.OrderDetails.RequestedSlot.StartTime,

                DistrictID = order.OrderDetails.providerDistrict.DistrictID,
                DistrictName = order.OrderDetails.providerDistrict.District.DistrictName,
                Address = order.OrderDetails.Address,
                Price = order.OrderDetails.Price,
                Problem = order.OrderDetails.ProblemDescription
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
            var spec = new BaseSpecifications<Order>(or=>or.OrderID==orderId);

            var order= await _orderRepository.GetOrder(spec);
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

        public async Task<Response<object>> ReAssignOrder(string orderId)
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
            var requestedSlot = order.OrderDetails.RequestedSlot;
            var services = order.OrderDetails.RequestedServices.Services;
            List<string> servicesIds = new List<string>();
            foreach (var service in services)
            {
                servicesIds.Add(service.ServiceID);
            }

            var startTime = requestedSlot.StartTime;
            var dayOfweek = requestedSlot.DayOfWeek;
            var district = order.OrderDetails.providerDistrict.DistrictID;
            var customer = order.Customer;



            var matchedProviders = await _providerRepository.GetAllMatchedProviders(servicesIds, startTime, dayOfweek, district,customer.Id);

            order.OrderStatus = OrderStatusEnum.Removed;

            _unitOfWork.Commit();

       
            var orderDetails = await ShowAllOrderDetailsForCustomer(orderId);

            
            if (matchedProviders == null)
            {

                var orderDetailsForCustomer = GenerateOrderDetailsMessage(order);
                var message = new EmailDto(customer.Email!, "Sarvicny: No Other Matched Providers Found", $"Unfortunately! Your Order is Canceled, Please try again with another time availabilities ,We hope better experiencenext time, see you soon. \n\nOrder Details:\n{orderDetailsForCustomer}");
                _emailService.SendEmail(message);

                return new Response<object>()
                {
                    Status = "Success",
                    Message = " No Matched providers is Found (orderStatus = removed & send email successfully)",
                    Payload = null,
                    isError = false

                };



            }
            var result = new
            {
                orderDetails = orderDetails,
                matchedProviders = matchedProviders,

            };
            var message2 = new EmailDto(customer.Email!, "Sarvicny:Matched Providers are Found", " New Recmmondations are found !! \n Please Select new provider from our recommendations.");
            _emailService.SendEmail(message2);
            return new Response<object>()
            { 
                Status = "Success",
                Message = "Matched providers are Found",
                Payload = result,
                isError = false
            };
        }

        public  async Task<Response<List<object>>> GetAllMatchedProviderSortedbyFav(MatchingProviderDto matchingProviderDto)
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
            var MatchedProviderSortedbyFav = await _providerRepository.GetAllMatchedProviders(matchingProviderDto.services, startTime,matchingProviderDto.dayOfWeek,matchingProviderDto.districtId,matchingProviderDto.customerId);
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
            
            List<object> result = new List<object>() ;
            foreach(var provider in MatchedProviderSortedbyFav)
            {
                var providerAsObj = new
                {
                    providerId = provider.Id,
                    firstname = provider.FirstName,
                    lastname = provider.LastName,
                    email= provider.Email,
                    
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


