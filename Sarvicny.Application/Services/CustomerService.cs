using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.CartSpecifications;
using Sarvicny.Application.Services.Specifications.CustomerSpecification;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceProviderRepository _providerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IServiceProviderService _serviceProvider;
        private readonly IDistrictRepository _districtRepository;


        public CustomerService(IServiceProviderRepository providerRepository
            , IUnitOfWork unitOfWork, IServiceRepository serviceRepository, ICustomerRepository customerRepository,
            IUserRepository userRepository, IOrderRepository orderRepository, ICartRepository cartRepository,
            IOrderService orderService, IServiceProviderService serviceProvider, IPaymentService paymentService,
            IDistrictRepository districtRepository)
        {
            _providerRepository = providerRepository;
            _unitOfWork = unitOfWork;
            _serviceRepository = serviceRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _orderService = orderService;
            _serviceProvider = serviceProvider;
            _districtRepository = districtRepository;
            _paymentService = paymentService;
        }
        public async Task<Response<object>> RequestService(RequestServiceDto requestServiceDto, string customerId)
        {
            requestServiceDto.RequestDay = requestServiceDto.RequestDay.Date;

            var spec = new ProviderWithServices_Districts_AndAvailabilitiesSpecification(requestServiceDto.ProviderId);
            var provider = await _providerRepository.FindByIdAsync(spec);

            if (provider == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Provider Not Found" },
                    Status = "Error",
                    Message = "Failed",
                };

            var serviceSpec = new BaseSpecifications<Service>(s => s.ServiceID == requestServiceDto.ServiceId);
            var service = await _serviceRepository.GetServiceById(serviceSpec);

            if (service == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Service Not Found" },
                    Status = "Error",
                    Message = "Failed",
                };

            var providerService =
                provider.ProviderServices.SingleOrDefault(ps => ps.ServiceID == requestServiceDto.ServiceId);

            if (providerService == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "This Worker is not Registered for the Service" },
                    Status = "Error",
                    Message = "Failed",
                };

            var district = await _districtRepository.GetDistrictById(requestServiceDto.DistrictID);
            if (district == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "district not found" },
                    Status = "Error",
                    Message = "Failed",
                };
            var providerDistrict = provider.ProviderDistricts.SingleOrDefault(d => d.DistrictID == requestServiceDto.DistrictID && d.enable==true);
            if (providerDistrict == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "This Worker is not Registered for the district" },
                    Status = "Error",
                    Message = "Failed",
                };

            var customerSpec = new CustomerWithCartSpecification(customerId);
            var customer = await _customerRepository.GetCustomerById(customerSpec);

            if (customer == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Customer Not Found" },
                    Status = "Error",
                    Message = "Customer Not Found",
                };

            var slots = provider.Availabilities.SelectMany(p => p.Slots);

            var slotExist = slots.SingleOrDefault(s => s.TimeSlotID == requestServiceDto.SlotID);

            if (slotExist == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Error with Date or Slot" },
                    Status = "Error",
                    Message = "An Error Occured",
                };
            if (slotExist.enable == false)
            {
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Error with Date or Slot" },
                    Status = "Error",
                    Message = "Slot is not available",
                };
            }

            var cart = customer.Cart;

            if (cart is null)
            {
                customer.Cart = new Cart
                {
                    CustomerID = customer.Id,
                    LastChangeTime = DateTime.UtcNow,
                    Customer = customer
                };
            }
            var cartRequest = cart.ServiceRequests.ToList();
            foreach (var request in cartRequest)  //check not already in cart
            {


                if (requestServiceDto.ServiceId == request.providerService.ServiceID && requestServiceDto.ProviderId == request.providerService.ProviderID && slotExist.TimeSlotID == request.SlotID)
                {
                    return new Response<object>
                    {
                        isError = true,

                        Status = "Error",
                        Message = "Request is already Found in the cart",
                    };
                }

            }

            var newRequest = new ServiceRequest
            {
                AddedTime = DateTime.UtcNow,
                providerService = providerService,
                providerDistrict = providerDistrict,
                Slot = slotExist,
                SlotID = slotExist.TimeSlotID,
                CartID = customer.Cart.CartID,
                Cart = customer.Cart,
                Price = providerService.Price,
                ProblemDescription = requestServiceDto.ProblemDescription
            };

            //slotExist.enable = false;

            await _customerRepository.AddRequest(newRequest);
            _unitOfWork.Commit();

            var output = new
            {
                RequestId = newRequest.ServiceRequestID,
                RequestDay = requestServiceDto.RequestDay,
                RequestTime = slotExist.StartTime,
                District = providerDistrict.District.DistrictName,
                ServiceName = service.ServiceName,
                ProviderName = provider.FirstName + " " + provider.LastName,
                Price = providerService.Price,
                ProblemDescription = requestServiceDto.ProblemDescription
            };

            return new Response<object>
            { isError = false, Message = "Service Request is added successfully to the cart", Payload = output };
        }

        public async Task<Response<object>> CancelRequestService(string customerId, string requestId)
        {
            var spec = new CustomerWithCartSpecification(customerId);
            var customer = await _customerRepository.GetCustomerById(spec);
            if (customer == null)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Customer is not found",
                    isError = true,
                    Errors = new List<string> { "Error with customer" },
                };
            }
            var cart = customer.Cart;
            if (cart == null)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Cart is not found",
                    Errors = new List<string> { "Error with cart" },
                    isError = true

                };
            }
            var spec2 = new ServiceRequestWithSlotSpecification(requestId);
            var serviceRequest = await _customerRepository
                .GetServiceRequestById(spec2);

            if (serviceRequest == null)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Service is not found",
                    Errors = new List<string> { "Error with service requested" },
                    isError = true

                };
            }

            var requestInCart = cart.ServiceRequests;

            if (requestInCart.Any(s => s.ServiceRequestID == requestId))
            {
                var requestAsObject = new
                {
                    ServiceRequestID = requestId,
                    serviceRequest.SlotID,
                    serviceRequest.AddedTime



                };
                await _customerRepository.RemoveRequest(serviceRequest);


                serviceRequest.Slot.enable = true;


                _unitOfWork.Commit();



                return new Response<object>()
                {
                    Payload = requestAsObject,
                    Message = "Sucess",
                    isError = false

                };
            }
            else
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Request is not found in the cart",
                    Errors = new List<string> { "Error with cart" },
                    isError = true

                };

            }






        }

        public async Task<Response<object>> GetCustomerCart(string customerId)
        {

            var customer = await _customerRepository.GetCustomerById(new CustomerWithCartSpecification(customerId));

            if (customer == null)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Customer is not found",
                    isError = true,
                    Errors = new List<string> { "Error with customer" },
                };
            }

            var cart = await _cartRepository.GetCart(new CartWithServiceRequestsSpecification(customer.CartID));

            if (cart == null)
            {
                customer.Cart = new Cart
                {
                    CustomerID = customer.Id,
                    LastChangeTime = DateTime.UtcNow,
                    Customer = customer
                };
                _unitOfWork.Commit();
            }

            // var requestedServices = cart.ServiceRequests.Select(s => s.providerService)
            //     .Select(ps => new
            //     {
            //         ps.ProviderID,
            //         ps.ServiceID,
            //         ps.Price
            //     });

            var requestedServices = cart.ServiceRequests.Select(s => new
            {
                s.ServiceRequestID,
                providerId = s.providerService.Provider.Id,
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
                s.ProblemDescription,


            });



            var CartAsObject = new
            {

                cart.CartID,
                requestedServices,
            };


            return new Response<object>()
            {
                Payload = CartAsObject,
                Message = "Success",
                isError = false

            };
        }

        public async Task<Response<object>> OrderCart(string customerId, PaymentMethod PayemntMethod)
        {
            #region validation_for_Data
            var spec = new CustomerWithCartSpecification(customerId);
            var customer = await _customerRepository.GetCustomerById(spec);
            if (customer == null)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Customer is not found",
                    isError = true,
                    Errors = new List<string> { "Error with customer" },
                };
            }
            var cart = customer.Cart;
            if (cart == null)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Cart is not found",
                    Errors = new List<string> { "Error with cart" },
                    isError = true

                };
            }
            var serviceRequests = cart.ServiceRequests;
            if (serviceRequests.Count() == 0)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Cart is empty",
                    Errors = new List<string> { "Error with cart" },
                    isError = true

                };
            }
            #endregion

            var totalPrice = serviceRequests.Sum(s => s.providerService.Price);

            var newOrder = new Order
            {
                Customer = customer,
                CustomerID = customerId,
                OrderStatus = OrderStatusEnum.Pending,
                TotalPrice = totalPrice,
                OrderDate = DateTime.UtcNow
            };

            var order = await _orderRepository.AddOrder(newOrder);
            var orderRequests = serviceRequests;

            foreach (var request in orderRequests)
            {
                request.OrderId = order.OrderID;
            }

            order.ServiceRequests = orderRequests;

            _unitOfWork.Commit();

            var response = await _paymentService.PayOrder(order, PayemntMethod);

            return response;

            //foreach (var serviceRequest in serviceRequests)
            //{
            //    var newRequest = new ServiceRequest();
            //    newRequest.ProviderServiceID = serviceRequest.ProviderServiceID;
            //    newRequest.providerService = serviceRequest.providerService;
            //    newRequest.Price = serviceRequest.Price;
            //    newRequest.Slot = serviceRequest.Slot;
            //    newRequest.SlotID = serviceRequest.SlotID;
            //    newRequest.AddedTime = serviceRequest.AddedTime;
            //    newRequest.OrderId = order.OrderID;
            //    newRequest.CartID = null;
            //    newRequest.Cart = null;
            //    newRequest.ProblemDescription = serviceRequest.ProblemDescription;


            //    await _customerRepository.AddRequest(newRequest);
            //    orderRequests.Add(newRequest);

            //    var specProvider = new ProviderWithAvailabilitesSpecification(serviceRequest.providerService.ProviderID);
            //    var provider = await _providerRepository.FindByIdAsync(specProvider);

            //    var slots = provider.Availabilities.SelectMany(p => p.Slots);

            //    var slotExist = slots.SingleOrDefault(s => s.TimeSlotID == serviceRequest.SlotID);
            //    slotExist.enable = false;

            //    await _customerRepository.RemoveRequest(serviceRequest);

            //}

            // serviceRequests = await _orderRepository.SetOrderToServiceRequest(serviceRequests, order);



            //await _customerRepository.EmptyCart(cart);



            //cart.ServiceRequests = null;
            //var spec2 = new OrderWithRequestsSpecification(order.OrderID);

            //var orders = await _orderRepository.GetOrder(spec2);


            //var result = new
            //{
            //    order.OrderID,
            //    order.CustomerID,
            //    order.OrderStatusID,
            //    order.OrderStatus.StatusName,
            //    order.TotalPrice,
            //    details = order.ServiceRequests.Select(s => new
            //    {
            //        s.ServiceRequestID,
            //        s.SlotID,
            //        s.Slot.StartTime,
            //        s.providerService.Provider.Id,
            //        Provider = s.providerService.Provider.FirstName,
            //        s.providerService.Service.ServiceID,
            //        s.providerService.Service.ServiceName,
            //        s.Price,
            //        s.ProblemDescription,
            //    }),
            //};
        }

        public async Task<Response<object>> ShowCustomerProfile(string customerId)
        {
            var spec = new BaseSpecifications<Customer>(c => c.Id == customerId);
            var customer = await _customerRepository.GetCustomerById(spec);
            if (customer == null)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Customer Not Found",
                    Payload = null,
                    isError = true

                };
            }
            var profile = new
            {
                customer.FirstName,
                customer.LastName,
                customer.UserName,
                customer.Email,
                customer.Address

            };
            return new Response<object>()
            {
                Payload = profile,
                isError = false,
                Message = "Success"

            };


        }

        public async Task<Response<object>> ViewLogRequest(string customerId)
        {
            var spec = new CustomerWithOrdersSpecification(customerId);
            var customer = await _customerRepository.GetCustomerById(spec);

            if (customer == null)
            {
                return new Response<object>()
                {
                    isError = true,
                    Payload = null,
                    Message = "Customer Not found",
                    Status = "Failed"

                };

            }
            var orders = customer.Orders;
            if (orders == null)
            {
                return new Response<object>()
                {
                    isError = true,
                    Payload = null,
                    Message = "No Orders found",
                    Status = "Failed"

                };
            }

            List<object> details = new List<object>();

            foreach (var order in orders)
            {

                var orderDetails = await _orderService.ShowAllOrderDetailsForCustomer(order.OrderID);
                details.Add(orderDetails);
            };
            return new Response<object>()
            {
                Payload = details,
                isError = false,
                Status = "success",
                Message = "success"


            };





        }
    }
}