using Sarvicny.Application.Common.Helper;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.CartSpecifications;
using Sarvicny.Application.Services.Specifications.CustomerSpecification;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceRequestSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
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
        private readonly IServiceProviderService _providerService;
        private readonly IDistrictRepository _districtRepository;
        private readonly IAdminService _adminService;
        private readonly IEmailService _emailService;
        private readonly ITransactionPaymentRepository _transactionPaymentRepository;




        public CustomerService(IServiceProviderRepository providerRepository
            , IUnitOfWork unitOfWork, IServiceRepository serviceRepository, ICustomerRepository customerRepository,
            IUserRepository userRepository, IOrderRepository orderRepository, ICartRepository cartRepository,
            IOrderService orderService, IServiceProviderService serviceProvider, IPaymentService paymentService,
            IDistrictRepository districtRepository, IAdminService adminService, IEmailService emailService, ITransactionPaymentRepository transactionPaymentRepository)
        {
            _providerRepository = providerRepository;
            _unitOfWork = unitOfWork;
            _serviceRepository = serviceRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _orderService = orderService;
            _providerService = serviceProvider;
            _districtRepository = districtRepository;
            _paymentService = paymentService;
            _adminService = adminService;
            _emailService = emailService;
            _transactionPaymentRepository = transactionPaymentRepository;
        }


        public async Task<Response<object>> addToCart(RequestServiceDto requestServiceDto, string customerId)
        {
            if (requestServiceDto.RequestDay < DateTime.Today)
            {
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Date in the past" },
                    Status = "Error",
                    Message = "Failed",
                };
            }
            var spec = new ProviderWithDetailsSpecification(requestServiceDto.ProviderId);
            var provider = await _providerRepository.FindByIdAsync(spec);

            if (provider == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Provider Not Found" },
                    Status = "Error",
                    Message = "Failed",
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
            if (slotExist.isActive == false)
            {
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Error with Date or Slot" },
                    Status = "Error",
                    Message = "Slot is not available ,may be reserved",
                };
            }


            var startTime = slotExist.StartTime;

            var allowedRange = DateTime.UtcNow.AddHours(2).TimeOfDay;
            if (requestServiceDto.RequestDay == DateTime.Today && startTime <= allowedRange)
            {
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "you can't schedule in the same day with slot within less than 2 hours from now" },
                    Status = "Error",
                    Message = "Failed",
                };
            }

            List<Service> services = new List<Service>();
            decimal price = 0;


            foreach (var Id in requestServiceDto.ServiceIDs)
            {
                var serviceSpec = new BaseSpecifications<Service>(s => s.ServiceID == Id);
                var service = await _serviceRepository.GetServiceById(serviceSpec);

                if (service == null)
                    return new Response<object>
                    {
                        isError = true,
                        Errors = new List<string> { "Service Not Found" },
                        Status = "Error",
                        Message = "Failed",
                    };
                if (service.ParentServiceID == null)
                    return new Response<object>
                    {
                        isError = true,
                        Errors = new List<string> { "This Service is a parent Service (not valid)" },
                        Status = "Error",
                        Message = "Failed",
                    };

                var providerService =
                provider.ProviderServices.SingleOrDefault(ps => ps.ServiceID == service.ServiceID);
                if (providerService == null)
                    return new Response<object>
                    {
                        isError = true,
                        Errors = new List<string> { "This Worker is not Registered for the Service" },
                        Status = "Error",
                        Message = "Failed",
                    };

                services.Add(service);
                price += providerService.Price;


            }

            //decimal rate = 0.12m;
            //price = price+price * rate;
            price = Math.Ceiling(price);

            if (services.Count() != requestServiceDto.ServiceIDs.Count())
            {
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Error With Service" },
                    Status = "Error",
                    Message = "Failed",
                };

            }
            var requestedServices = new RequestedService();
            requestedServices.Services = services;

            await _serviceRepository.AddRequestedService(requestedServices);


            var district = await _districtRepository.GetDistrictById(requestServiceDto.DistrictID);
            if (district == null)
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "district not found" },
                    Status = "Error",
                    Message = "Failed",
                };
            var providerDistrict = provider.ProviderDistricts.SingleOrDefault(d => d.DistrictID == requestServiceDto.DistrictID && d.enable == true);
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





            var dayofweek = requestServiceDto.RequestDay.DayOfWeek.ToString();

            if (dayofweek != slotExist.ProviderAvailability.DayOfWeek)
            {
                return new Response<object>
                {
                    isError = true,
                    Errors = new List<string> { "Error with Date" },
                    Status = "Error",
                    Message = "This date is inconsistent with the provided slot",
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
            var cartRequest = cart.CartServiceRequests.ToList();
            foreach (var request in cartRequest)  //check not already in cart
            {

                if (requestServiceDto.ProviderId == request.ProviderID && slotExist.TimeSlotID == request.SlotID && requestServiceDto.RequestDay == request.RequestedDate)
                {
                    return new Response<object>
                    {
                        isError = true,

                        Status = "Error",
                        Message = "Provider is already Found in the cart",
                    };
                }

            }

            var Address = requestServiceDto.Address;

            if (Address is null)
            {
                Address = customer.Address;
            }

            var newRequest = new CartServiceRequest
            {
                RequestedDate = requestServiceDto.RequestDay,
                Provider = provider,
                ProviderID = provider.Id,
                RequestedServices = requestedServices,
                providerDistrict = providerDistrict,
                Slot = slotExist,
                SlotID = slotExist.TimeSlotID,
                CartID = customer.Cart.CartID,
                Cart = customer.Cart,

                Price = price,
                ProblemDescription = requestServiceDto.ProblemDescription,
                Address = Address,
            };



            await _customerRepository.AddRequest(newRequest);
            _unitOfWork.Commit();

            var output = new
            {
                RequestId = newRequest.CartServiceRequestID,
                RequestDay = requestServiceDto.RequestDay,
                DayOfWeek = slotExist.ProviderAvailability.DayOfWeek,
                RequestTime = slotExist.StartTime,
                District = providerDistrict.District.DistrictName,
                Address = Address,
                ProviderName = provider.FirstName + " " + provider.LastName,
                Services = requestedServices.Services.Select(s => new
                {
                    s.ServiceID,
                    s.ServiceName

                }).ToList<object>(),

                Price = price,
                ProblemDescription = requestServiceDto.ProblemDescription
            };

            return new Response<object>
            { isError = false, Message = "Service Request is added successfully to the cart", Payload = output };
        }

        public async Task<Response<object>> AddToCartSampleData()
        {
            return new Response<object>
            {
                isError = false,
                Message = "Success",
                Payload = null
            };
        }


        public async Task<Response<object>> RemoveFromCart(string customerId, string requestId)
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
            var spec2 = new CartServiceRequestWithDetailsSpecification(requestId);
            var serviceRequest = await _customerRepository.GetCartServiceRequestById(spec2);

            if (serviceRequest == null)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Request is not found",
                    Errors = new List<string> { "Error with service requested" },
                    isError = true

                };
            }

            var requestInCart = cart.CartServiceRequests;

            if (requestInCart.Any(s => s.CartServiceRequestID == requestId))
            {

                var requestAsObject = new
                {
                    ServiceRequestID = requestId,

                    ProviderID = serviceRequest.Provider.Id,
                    ProviderName = serviceRequest.Provider.FirstName + " " + serviceRequest.Provider.LastName,
                    Services = serviceRequest.RequestedServices.Services.Select(s => new
                    {
                        s.ServiceID,
                        s.ServiceName

                    }).ToList<object>(),
                    SlotId = serviceRequest.SlotID,
                    RequestedDate = serviceRequest.RequestedDate,
                    DayOfWeek = serviceRequest.Slot != null ? serviceRequest.Slot.ProviderAvailability.DayOfWeek : null,
                    StartTime = serviceRequest.Slot != null ? serviceRequest.Slot.StartTime : (TimeSpan?)null,
                    EndTime = serviceRequest.Slot != null ? serviceRequest.Slot.EndTime : (TimeSpan?)null,

                    DistrictId = serviceRequest.ProviderDistrictID,
                    District = serviceRequest.providerDistrict.District.DistrictName,
                    Address = serviceRequest.Address,
                    Price = serviceRequest.Price,
                    ProblemDescription = serviceRequest.ProblemDescription


                };
                await _customerRepository.RemoveRequest(serviceRequest);

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

            var requestedServices = cart.CartServiceRequests.Select(s => new
            {
                s.CartServiceRequestID,
                providerId = s.Provider.Id,
                s.Provider.FirstName,
                s.Provider.LastName,
                Services = s.RequestedServices.Services.Select(s => new
                {
                    s.ServiceID,
                    s.ServiceName,
                    s.ParentServiceID,
                    parentServiceName = s.ParentService?.ServiceName,
                    s.CriteriaID,
                    s.Criteria?.CriteriaName,

                }).ToList<object>(),


                s.SlotID,
                s.RequestedDate,
                s.Slot?.ProviderAvailability.DayOfWeek,
                s.Slot?.StartTime,
                s.providerDistrict.DistrictID,
                s.providerDistrict.District.DistrictName,
                s.Address,
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

        public async Task<Response<object>> OrderCart(string customerId, PaymentMethod paymentMethod)
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
            var cartServiceRequests = cart.CartServiceRequests;
            if (cartServiceRequests.Count() == 0)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Cart is empty",
                    Errors = new List<string> { "Error with cart" },
                    isError = true

                };
            }
            var deleted = cartServiceRequests.Any(r => r.Slot == null);
            if (deleted)
            {
                return new Response<object>
                {
                    isError = true,
                    Payload = null,
                    Status = "Error",
                    Message = "Request slots seams to be not found anymore ,may be deleted",
                };
            }
            var reserved = cartServiceRequests.Any(r => r.Slot.isActive == false);

            if (reserved)
            {
                return new Response<object>
                {
                    isError = true,
                    Payload = null,
                    Status = "Error",
                    Message = "Request seams to be reserved by another user",
                };
            }

            #endregion


            var totalPrice = cartServiceRequests.Sum(s => s.Price);

            List<object> result = new List<object>();
            List<Order> orders = new List<Order>();

            try
            {
                foreach (var request in cartServiceRequests)
                {
                    var startTime = request.Slot.StartTime;
                    var allowedRange = DateTime.UtcNow.AddHours(2).TimeOfDay;
                    if (request.RequestedDate == DateTime.Today && startTime <= allowedRange)
                    {
                        return new Response<object>
                        {
                            isError = true,
                            Errors = new List<string> { "you can't order in the same day with slot within less than 2 hours from now" },
                            Status = "Error",
                            Message = "Failed",
                        };
                    }
                    var requestedSlot = new RequestedSlot
                    {
                        RequestedDay = request.RequestedDate,
                        DayOfWeek = request.Slot.ProviderAvailability.DayOfWeek,
                        StartTime = request.Slot.StartTime

                    };

                    var newRequestedSlot = await _orderRepository.AddRequestedSlot(requestedSlot);

                    var originalSlot = await _providerService.getOriginalSlot(requestedSlot, request.ProviderID);
                    if (originalSlot != null)
                    {
                        originalSlot.isActive = false;
                    }



                    var newOrderDetails = new OrderDetails
                    {
                        ProviderID = request.ProviderID,
                        Provider = request.Provider,
                        RequestedServicesID = request.RequestedServicesID,
                        RequestedServices = request.RequestedServices,
                        Price = request.Price,
                        RequestedSlot = newRequestedSlot,
                        RequestedSlotID = newRequestedSlot.RequestedSlotId,
                        providerDistrict = request.providerDistrict,
                        ProviderDistrictID = request.ProviderDistrictID,
                        Address = request.Address,
                        ProblemDescription = request.ProblemDescription,

                    };
                    var newOrder = new Order
                    {
                        Customer = customer,
                        CustomerID = customerId,
                        OrderDate = DateTime.UtcNow,

                        OrderDetails = newOrderDetails,
                        OrderDetailsId = newOrderDetails.OrderDetailsID,

                        //PaymentMethod = paymentMethod,
                        //PaymentExpiryTime = DateTime.UtcNow.AddHours(2),

                    };

                    var order = await _orderRepository.AddOrder(newOrder);
                    newOrderDetails.OrderId = order.OrderID;
                    // newOrderDetails.Order = newOrder;
                    var orderDetails = await _orderRepository.AddOrderDetails(newOrderDetails);

                    orders.Add(order);
                    var orderAsObject = new
                    {
                        OrderId = order.OrderID,
                        CustomerId = order.CustomerID,
                        OrderDate = order.OrderDate,
                        ProviderID = orderDetails.ProviderID,
                        ProviderFName = orderDetails.Provider.FirstName,
                        ProviderLName = orderDetails.Provider.LastName,
                        RequestedServicesID = orderDetails.RequestedServicesID,
                        RequestedServices = orderDetails.RequestedServices.Services.Select(s => new
                        {
                            s.ServiceID,
                            s.ServiceName
                        }).ToList<object>(),
                        Price = orderDetails.Price,
                        RequestedSlotID = orderDetails.RequestedSlotID,
                        ProviderDistrictID = orderDetails.ProviderDistrictID,
                        Address = orderDetails.Address,
                        ProblemDescription = orderDetails.ProblemDescription,
                    };
                    result.Add(orderAsObject);

                }

                await _cartRepository.ClearCart(cart);

                var rate = 0.12m;
                TransactionPayment transactionPayment = new TransactionPayment
                {
                    Amount = totalPrice + (totalPrice * rate),
                    OrderList = orders,
                    PaymentMethod = paymentMethod

                };

                transactionPayment = await _transactionPaymentRepository.AddTransactionPaymentAsync(transactionPayment);

                foreach (var order in orders)
                {
                    order.TransactionPaymentId = transactionPayment.TransactionPaymentID;
                    order.TransactionPayment = transactionPayment;
                }



                ///var response = await _paymentService.PayOrder(order, PayemntMethod);

                _unitOfWork.Commit();

                foreach (var order in orders)
                {
                    var specO = new OrderWithDetailsSpecification(order.OrderID);
                    var detailedOrder = await _orderRepository.GetOrder(specO);
                    var orderDetailsForProvider = HelperMethods.GenerateOrderDetailsMessageForProvider(detailedOrder);
                    var message = new EmailDto(order.OrderDetails.Provider.Email!, "Sarvicny: new Request Alert", $" A new request is ordered, Here is some of its details ,\n\nOrder Details:\n{orderDetailsForProvider} , check the website or the application fo more details.");
                    _emailService.SendEmail(message);
                }

                var output = new
                {
                    TransactiopnID = transactionPayment.TransactionPaymentID,
                    orders = result,
                    OrdersTotalPrice = transactionPayment.Amount,
                };

                return new Response<object>()
                {
                    Payload = output,
                    Message = "Orders are requested successfully",
                    isError = false
                };
            }
            catch (Exception ex)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = $"An error occurred: {ex.Message}",
                    isError = true,
                    Errors = new List<string> { ex.StackTrace }
                };
            }
        }

        //public async Task<Response<object>> PayOrder(string orderId, PaymentMethod PayemntMethod)
        //{
        //    var spec = new OrderWithDetailsSpecification(orderId);
        //    var order = await _orderRepository.GetOrder(spec);

        //    if (order == null)
        //    {
        //        return new Response<object>()
        //        {
        //            Status = "failed",
        //            Message = "Order Not Found",
        //            Payload = null,
        //            isError = true

        //        };
        //    }
        //    var response = await _paymentService.PayOrder(order, PayemntMethod);

        //    return response;

        //}

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
                customer.Address,
                customer.PhoneNumber

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

        public async Task<Response<object>> UpdateCustomerProfile(UpdateCustomerDto updateCustomerDto, string customerId)
        {
            var spec = new BaseSpecifications<Customer>(c => c.Id == customerId);
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
            var usrname = updateCustomerDto.UserName;
            var email = updateCustomerDto.Email;
            var phone = updateCustomerDto.PhoneNumber;
            var address = updateCustomerDto.Address;


            if (usrname != null)
            {
                var user = await _userRepository.GetUserByUserNameAsync(usrname);
                if (user != null && user != customer) //another user rather than the updated
                {
                    return new Response<object>()
                    {
                        Status = "failed",
                        Message = "userName already Found",
                        Payload = null,
                        isError = true

                    };
                }
                customer.UserName = usrname;

            }
            if (email != null)
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user != null && user != customer) //another user rather than the updated
                {
                    return new Response<object>()
                    {
                        Status = "failed",
                        Message = "email already Found",
                        Payload = null,
                        isError = true

                    };

                }
                customer.Email = email;
            }

            if (phone != null)
            {
                customer.PhoneNumber = phone;
            }
            if (address != null)
            {
                customer.Address = address;

            }

            await _userRepository.UpdateUserAsync(customer);
            _unitOfWork.Commit();

            var customerAsObject = new
            {
                userName = customer.UserName,
                email = customer.Email,

                phone = customer.PhoneNumber,
                address = customer.Address,


            };

            return new Response<object>()
            {
                Payload = customerAsObject,
                Message = "customer updated successfuly",
                isError = false,

            };


        }

        public async Task<Response<object>> MarkOrderComplete(string orderId)
        {
            var spec = new OrderWithDetailsSpecification(orderId);
            var order = await _orderRepository.GetOrder(spec);

            if (order == null)
            {
                return new Response<object>()

                {
                    isError = true,
                    Payload = null,
                    Message = "Order Not Found",
                    Errors = new List<string>() { "Error with order" },

                };


            }
            if (order.OrderStatus != OrderStatusEnum.Done)
            {
                return new Response<object>()

                {
                    isError = false,
                    Payload = null,
                    Message = "provider does't mark order done",
                    Errors = new List<string>() { "Error with order" },

                };


            }
            order.OrderStatus = OrderStatusEnum.Completed;

            var originalSlot = await _providerService.getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
            if (originalSlot != null)
            {
                originalSlot.isActive = true;
            }


            _unitOfWork.Commit();

            var details = _orderService.ShowAllOrderDetailsForCustomer(orderId);
            return new Response<object>()
            {
                isError = true,
                Payload = details,
                Message = "Action Done Successfully",
                Errors = null,
            };
        }

        public async Task<Response<object>> AddProviderToFav(string providerId, string customerId)
        {
            var spec = new CustomerWithFavouritesSpecification(customerId);
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
            var providerSpec = new BaseSpecifications<Provider>(p => p.Id == providerId);
            var provider = await _providerRepository.FindByIdAsync(providerSpec);
            if (provider == null)
            {

                return new Response<object>()
                {
                    Payload = null,
                    Message = "Provider is not found",
                    isError = true,
                    Errors = new List<string> { "Error with provider" },
                };
            }
            if (!provider.IsVerified || provider.IsBlocked)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Provider may be not verified or blocked",
                    isError = true,
                    Errors = new List<string> { "Error with provider" },
                };
            }
            var customerFav = customer.Favourites;
            if (customerFav.Any(f => f.providerId == providerId))
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Provider is already favourite",
                    isError = true,
                    Errors = new List<string> { "Error with provider" },
                };
            }



            var newFav = new FavProvider
            {
                customerId = customerId,
                providerId = providerId,
            };
            customer.Favourites.Add(newFav);
            await _customerRepository.AddFavProvider(newFav);
            _unitOfWork.Commit();

            return new Response<object>()
            {

                Message = "Action Done Successfully",
                isError = false,
                Errors = null,

            };
        }

        public async Task<Response<object>> RemoveFavProvider(string customerId, string providerId)
        {
            var spec = new CustomerWithFavouritesSpecification(customerId);
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
            var providerSpec = new BaseSpecifications<Provider>(p => p.Id == providerId);
            var provider = await _providerRepository.FindByIdAsync(providerSpec);
            if (provider == null)
            {

                return new Response<object>()
                {
                    Payload = null,
                    Message = "Provider is not found",
                    isError = true,
                    Errors = new List<string> { "Error with provider" },
                };
            }
            if (!provider.IsVerified || provider.IsBlocked)
            {
                return new Response<object>()
                {
                    Payload = null,
                    Message = "Provider may be not verified or blocked",
                    isError = true,
                    Errors = new List<string> { "Error with provider" },
                };
            }


            var fav = customer.Favourites.FirstOrDefault(f => f.providerId == providerId);
            customer.Favourites.Remove(fav);
            await _customerRepository.RemoveFavProvider(fav);
            _unitOfWork.Commit();

            return new Response<object>()
            {

                Message = "Action Done Successfully",
                isError = false,
                Errors = null,

            };

        }
        public async Task<Response<List<object>>> getCustomerFavourites(string customerId)
        {
            var spec = new CustomerWithFavouritesSpecification(customerId);
            var customer = await _customerRepository.GetCustomerById(spec);
            if (customer == null)
            {
                return new Response<List<object>>()
                {
                    Payload = null,
                    Message = "Customer is not found",
                    isError = true,
                    Errors = new List<string> { "Error with customer" },
                };
            }
            var fav = customer.Favourites;
            List<object> result = new List<object>();
            foreach (var provider in fav)
            {
                var favAsObj = new
                {
                    providerId = provider.providerId
                };
                result.Add(favAsObj);
            }


            return new Response<List<object>>()
            {
                Payload = result,
                Message = "Action Done Successfully",
                isError = false,
                Errors = null
            };

        }

        public async Task<Response<object>> Refund(string orderId)
        {
            var order = await _orderRepository.GetOrder(new OrderWithDetailsSpecification(orderId));

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
            var orderPrice = order.OrderDetails.Price;


            if (order.OrderStatus != OrderStatusEnum.Paid)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "Order Status is not Paid",
                    Payload = null,
                    isError = true

                };
            }

            var response = await _paymentService.RefundOrder(order, orderPrice);

            _unitOfWork.Commit();

            return response;
        }

        public async Task<Response<object>> GetCustomerCanceledOrders(string customerId)
        {
            var customer = await _customerRepository.GetCustomerById(new CustomerWithOrdersSpecification(customerId));
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
            var orders = customer.Orders.Where(o => o.OrderStatus == OrderStatusEnum.CanceledByProvider);


            List<object> result = new List<object>();
            foreach (var order in orders)
            {
                var orderDetails = await _orderService.ShowAllOrderDetailsForCustomer(order.OrderID);
                result.Add(orderDetails);
            }


            if (result.Count == 0)
            {
                return new Response<object>()
                {
                    Status = "failed",
                    Message = "No Orders Found",
                    Payload = null,
                    isError = true
                };
            }

            return new Response<object>()
            {
                Status = "Success",
                Message = "Action Done Successfully",
                Payload = result,
                isError = false
            };


        }
    }

}