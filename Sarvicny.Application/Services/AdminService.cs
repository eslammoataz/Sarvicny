using Microsoft.AspNetCore.Identity;
using Sarvicny.Application.Common.Helper;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.DistrictSpecification;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services;

public class AdminService : IAdminService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IServiceProviderRepository _providerRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly IOrderRepository _orderRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ITransactionPaymentRepository _transactionPaymentRepository;



    private readonly IUnitOfWork _unitOfWork;

    private readonly IOrderService _orderService;
    private readonly IServiceProviderService _providerService;

    public AdminService(UserManager<User> userManager, IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepositor, IAdminRepository adminRepository, IOrderRepository orderRepository, IEmailService emailService, IOrderService orderService, IDistrictRepository districtRepository, IServiceProviderService providerService, ICustomerRepository customerRepository, ITransactionPaymentRepository transactionPaymentRepository)
    {
        _userManager = userManager;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _providerRepository = serviceProviderRepositor;
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _emailService = emailService;
        _orderService = orderService;
        _districtRepository = districtRepository;
        _providerService = providerService;
        _customerRepository = customerRepository;
        _transactionPaymentRepository = transactionPaymentRepository;
    }

    public async Task<Response<ICollection<object>>> GetAllCustomers()
    {
        var customers = await _userRepository.GetAllCustomers();


        var customersAsObjects = customers.Select(c => new
        {
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email,
            c.Address
        }).ToList<object>();


        return new Response<ICollection<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = customersAsObjects
        };
    }

    public async Task<Response<ICollection<object>>> GetAllServiceProviders()
    {
        var spec = new ServiceProviderWithService_DistrictSpecificationcs();
        var serviceProviders = await _providerRepository.GetAllServiceProviders(spec);

        var serviceProvidersAsObjects = serviceProviders.Select(c => new
        {
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email,
            c.IsVerified,
            c.IsBlocked,
            services = c.ProviderServices.Select(p => new
            {
                p.ServiceID,
                p.Service.ServiceName

            }).ToList<object>(),
            districts = c.ProviderDistricts.Select(d => new
            {
                d.DistrictID,
                d.District.DistrictName,
                d.enable


            }).ToList(),

        }).ToList<object>();

        return new Response<ICollection<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = serviceProvidersAsObjects
        };

    }
    public async Task<Response<ICollection<object>>> GetAllServices()
    {
        var spec = new ServiceWithCriteriaSpecification();
        var services = await _serviceRepository.GetAllServices(spec);

        //var criteria = services.Where(s=>s.Criteria!=null).Select(s=>s.Criteria.CriteriaName);
        var servicesAsObjects = services.Select(s => new
        {
            s.ServiceID,
            s.ServiceName,
            CriteriaName = s.Criteria?.CriteriaName

        }).ToList<object>();


        return new Response<ICollection<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = servicesAsObjects
        };
    }

    public async Task<Response<object>> ApproveProviderRegisteration(string workerId)
    {
        var spec = new ProviderWithDetailsSpecification(workerId);
        var provider = await _providerRepository.FindByIdAsync(spec);
        if (provider == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Provider Not Found",
                Payload = null,
                isError = true,

            };

        }


        //Add Token to Verify the email....
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(provider);
        var approved = await _adminRepository.ApproveServiceProviderRegister(spec);
        var outputAsObject = new
        {
            approved.Id,
            approved.FirstName,
            approved.LastName,

            isVerified = approved.IsVerified,
            services = approved.ProviderServices.Select(s => new
            {
                s.ServiceID,
                s.Service.ServiceName,

                s.Service.ParentServiceID,
                parentServiceName = s.Service.ParentService?.ServiceName,
                s.Service.CriteriaID,
                s.Service.Criteria?.CriteriaName
            }).ToList(),

            districts = approved.ProviderDistricts.Select(d => new
            {
                d.DistrictID,
                d.District.DistrictName,

            }).ToList(),

        };

        _unitOfWork.Commit();

        var message = new EmailDto(provider.Email!, "Sarvicny: Worker Approved Successfully", "Congratulations you are accepted");

        _emailService.SendEmail(message);
        return new Response<object>()
        {
            Status = "Success",
            Message = "Worker Approved Successfully , Verification Email sent to provider's email ",
            Payload = outputAsObject,

        };


    }



    public async Task<Response<object>> RejectProviderRegisteration(string workerId)
    {
        var provider = await _userRepository.GetUserByIdAsync(workerId);
        if (provider == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Provider Not Found",
                Payload = null,
                isError = true

            };
        }
        _unitOfWork.Commit();

        //Add Token to Verify the email....
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(provider);


        var message = new EmailDto(provider.Email!, "Sarvicny: Worker Rejected", "Sorry you are rejected");

        _emailService.SendEmail(message);
        var rejected = await _adminRepository.RejectServiceProviderRegister(workerId);
        var outputAsObject = new
        {
            rejected.Id,
            rejected.FirstName,
            rejected.LastName,

            isVerified = rejected.IsVerified,
            services = rejected.ProviderServices.Select(s => new
            {
                s.ServiceID,
                s.Service.ServiceName,

                s.Service.ParentServiceID,
                parentServiceName = s.Service.ParentService?.ServiceName,
                s.Service.CriteriaID,
                s.Service.Criteria?.CriteriaName
            }).ToList(),

            districts = rejected.ProviderDistricts.Select(d => new
            {
                d.DistrictID,
                d.District.DistrictName,

            }).ToList(),

        };
        return new Response<object>()
        {
            Status = "Success",
            Message = "Worker Rejected Successfully , Verification Email sent to provider's email ",
            Payload = outputAsObject,
            isError = false

        };
    }

    public async Task<Response<object>> ApproveServiceForProvider(string providerId, string providerServiceID)
    {
        var spec = new ProviderWithDetailsSpecification(providerId);

        var provider = await _providerRepository.FindByIdAsync(spec);
        if (provider == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Provider Not Found",
                Payload = null,
                isError = true,
            };

        }
        var providerService = provider.ProviderServices.FirstOrDefault(p => p.ProviderServiceID == providerServiceID);

        if (providerService == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Service Provider Not Found",
                Payload = null,
                isError = true,

            };
        }
        if (providerService.isVerified == true)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Service is already verified ",
                Payload = null,
                isError = true,

            };
        }
        _adminRepository.ApproveProviderService(providerServiceID);

        _unitOfWork.Commit();

        var message = new EmailDto(provider.Email!, "Sarvicny: Service Approved Successfully", "Congratulations you are request of applying to the service accepted");

        _emailService.SendEmail(message);

        return new Response<object>()
        {
            Status = "Success ",
            Message = "Service Provider is verified ",
            Payload = null


        };

    }

    public async Task<Response<object>> RejectServiceForProvider(string providerId, string providerServiceID)
    {
        var spec = new ProviderWithDetailsSpecification(providerId);

        var provider = await _providerRepository.FindByIdAsync(spec);
        if (provider == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Provider Not Found",
                Payload = null,

            };

        }
        var providerService = provider.ProviderServices.FirstOrDefault(p => p.ProviderServiceID == providerServiceID);

        if (providerService == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Service For the Provider Not Found",
                Payload = null,
                isError = true,

            };
        }
        if (providerService.isVerified == true)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Service is already verified ",
                Payload = null,
                isError = true,

            };
        }

        _adminRepository.RejectProviderService(providerServiceID);
        _unitOfWork.Commit();

        var message = new EmailDto(provider.Email!, "Sarvicny: Service Addition Rejected", "Unfortunatly! you are request of applying to the service is rejected");

        _emailService.SendEmail(message);

        return new Response<object>()
        {
            Status = "Success ",
            Message = "Service Provider is removed ",
            Payload = null,


        };
    }

    public async Task<Response<List<object>>> GetProvidersAddtionalServiceRequests()
    {
        var spec = new ProviderWithDetailsSpecification();

        var unHandledProviders = await _providerRepository.GetProvidersServiceRegistrationRequest(spec);

        var unHandeledProvidersAsObjects = unHandledProviders.Select(p => new
        {

            p.Id,
            p.FirstName,
            p.LastName,

            isVerified = p.IsVerified,
            services = p.ProviderServices.Where(ps => ps.isVerified == false).Select(s => new
            {
                s.ProviderServiceID,
                s.ServiceID,
                s.Service.ServiceName,

                s.Service.ParentServiceID,
                parentServiceName = s.Service.ParentService?.ServiceName,
                s.Service.CriteriaID,
                s.Service.Criteria?.CriteriaName
            }).ToList(),

        }).ToList<object>();


        var response = new Response<List<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = unHandeledProvidersAsObjects,
            isError = false
        };

        return response;
    }



    public async Task<Response<List<object>>> GetProvidersRegistrationRequests()
    {
        var spec = new ProviderWithDetailsSpecification();

        var unHandledProviders = await _providerRepository.GetProvidersRegistrationRequest(spec);

        var unHandeledProvidersAsObjects = unHandledProviders.Select(p => new
        {

            p.Id,
            p.FirstName,
            p.LastName,
            p.UserName,
            p.Email,
            p.PasswordHash,
            p.PhoneNumber,
            isVerified = p.IsVerified,
            services = p.ProviderServices.Select(s => new
            {
                s.ServiceID,
                s.Service.ServiceName,

                s.Service.ParentServiceID,
                parentServiceName = s.Service.ParentService?.ServiceName,
                s.Service.CriteriaID,
                s.Service.Criteria?.CriteriaName
            }).ToList(),

            districts = p.ProviderDistricts.Select(d => new
            {
                d.DistrictID,
                d.District.DistrictName,

            }).ToList(),

        }).ToList<object>();


        var response = new Response<List<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = unHandeledProvidersAsObjects,
            isError = false
        };

        return response;
    }

    public async Task<Response<List<object>>> getAllOrders()
    {
        var spec = new OrderWithDetailsSpecification();

        var orders = await _orderRepository.GetAllOrders(spec);



        List<object> result = new List<object>();
        foreach (var order in orders)
        {
            var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);

            result.Add(orderDetails);

        }

        if (result.Count == 0)
        {
            return new Response<List<object>>()
            {
                Status = "Success",
                Message = "No Orders Found",
                Payload = null,
                isError = false
            };
        }

        return new Response<List<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = result,

        };



    }

    public async Task<Response<List<object>>> getAllPendingOrPaidOrders()
    {
        var spec = new OrderWithDetailsSpecification();
        var pending = await _orderRepository.GetAllPendingOrPaidOrders(spec);

        List<object> result = new List<object>();
        foreach (var order in pending)
        {
            var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
            result.Add(orderDetails);
        }

        if (result.Count == 0)
        {
            return new Response<List<object>>()
            {
                Status = "Success",
                Message = "No pending Orders Found",
                Payload = null,
                isError = false
            };
        }

        return new Response<List<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = result,
            isError = false
        };


    }

    //public async Task<Response<List<object>>> getAllApprovedOrders()
    //{

    //    var spec = new OrderWithDetailsSpecification();
    //    var approvedOrders = await _orderRepository.GetAllApprovedOrders(spec);

    //    List<object> result = new List<object>();
    //    foreach (var order in approvedOrders)
    //    {
    //        var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
    //        result.Add(orderDetails);
    //    }

    //    if (result.Count == 0)
    //    {
    //        return new Response<List<object>>()
    //        {
    //            Status = "failed",
    //            Message = "No Approved Orders Found",
    //            Payload = null,
    //            isError = true
    //        };
    //    }

    //    return new Response<List<object>>()
    //    {
    //        Status = "Success",
    //        Message = "Action Done Successfully",
    //        Payload = result,
    //        isError = false
    //    };


    //}

    //public async Task<Response<List<object>>> getAllRejectedOrders()
    //{
    //    var spec = new OrderWithDetailsSpecification();
    //    var Rejected = await _orderRepository.GetAllRejectedOrders(spec);

    //    List<object> result = new List<object>();
    //    foreach (var order in Rejected)
    //    {
    //        var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
    //        result.Add(orderDetails);
    //    }

    //    if (result.Count == 0)
    //    {
    //        return new Response<List<object>>()
    //        {
    //            Status = "failed",
    //            Message = "No Rejected Orders Found",
    //            Payload = null,
    //            isError = true
    //        };
    //    }

    //    return new Response<List<object>>()
    //    {
    //        Status = "Success",
    //        Message = "Action Done Successfully",
    //        Payload = result,
    //        isError = false
    //    };
    //}

    //public async Task<Response<List<object>>> getAllExpiredOrders()
    //{
    //    var spec = new OrderWithDetailsSpecification();
    //    var Expired = await _orderRepository.GetAllExpiredOrders(spec);

    //    List<object> result = new List<object>();
    //    foreach (var order in Expired)
    //    {
    //        var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
    //        result.Add(orderDetails);
    //    }

    //    if (result.Count == 0)
    //    {
    //        return new Response<List<object>>()
    //        {
    //            Status = "failed",
    //            Message = "No Expired Orders Found",
    //            Payload = null,
    //            isError = true
    //        };
    //    }

    //    return new Response<List<object>>()
    //    {
    //        Status = "Success",
    //        Message = "Action Done Successfully",
    //        Payload = result,
    //        isError = false
    //    };
    ////}
    //public async Task<Response<List<object>>> RemoveAllPaymentExpiredOrders()
    //{
    //    var spec = new OrderWithDetailsSpecification();
    //    var expired = await _orderRepository.getAllPaymentExpiredOrders(spec);


    //    List<object> result = new List<object>();

    //    foreach (var order in expired)
    //    {
    //        order.OrderStatus = OrderStatusEnum.Removed;
    //        var originalSlot = await _providerService.getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
    //        if (originalSlot != null)
    //        {
    //            originalSlot.isActive = true;
    //        }
    //        var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
    //        result.Add(orderDetails);
    //    }
    //    _unitOfWork.Commit();




    //    if (result.Count == 0)
    //    {
    //        return new Response<List<object>>()
    //        {
    //            Status = "failed",
    //            Message = "No Expired Orders Found",
    //            Payload = null,
    //            isError = true
    //        };
    //    }
    //    return new Response<List<object>>()
    //    {
    //        Status = "Success",
    //        Message = "Action Done Successfully",
    //        Payload = result,
    //        isError = false
    //    };
    //}


    public async Task<Response<List<object>>> getAllCanceledByProviderOrders()
    {
        var spec = new OrderWithDetailsSpecification();
        var Canceled = await _orderRepository.GetAllCanceledByProviderOrders(spec);

        List<object> result = new List<object>();
        foreach (var order in Canceled)
        {
            var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
            result.Add(orderDetails);
        }

        if (result.Count == 0)
        {
            return new Response<List<object>>()
            {
                Status = "Success",
                Message = "No Canceled Orders Found",
                Payload = null,
                isError = false
            };
        }

        return new Response<List<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = result,
            isError = false
        };

    }
    public async Task<Response<bool>> BlockServiceProvider(string workerId)
    {
        var provider = await _providerRepository.FindByIdAsync(new BaseSpecifications<Provider>(p => p.Id == workerId));

        if (provider == null)
        {
            return new Response<bool>()
            {
                Status = "Fail",
                Message = "Provider Not Found",
                Payload = false,
                isError = true,
                Errors = new List<string>() { "Provider Not Found" }

            };
        }

        provider.IsBlocked = true;
        _unitOfWork.Commit();
        return new Response<bool>()
        {
            Status = "Success",
            Message = "Provider Blocked Successfully",
            Payload = true,
            isError = false

        };
    }

    public async Task<Response<bool>> UnBlockServiceProvider(string workerId)
    {
        var provider = await _providerRepository.FindByIdAsync(new BaseSpecifications<Provider>(p => p.Id == workerId));

        if (provider == null)
        {
            return new Response<bool>()
            {
                Status = "Fail",
                Message = "Provider Not Found",
                Payload = false,
                isError = true,
                Errors = new List<string>() { "Provider Not Found" }

            };
        }

        provider.IsBlocked = false;
        _unitOfWork.Commit();
        return new Response<bool>()
        {
            Status = "Success",
            Message = "Provider UnBlocked Successfully",
            Payload = true,
            isError = false

        };

    }

    public async Task<Response<District>> AddDistrict(District district)
    {
        var spec = new BaseSpecifications<District>();
        var districts = await _districtRepository.GetAllDistricts(spec);
        if (districts.Any(d => d.DistrictName == district.DistrictName))
        {

            return new Response<District>()
            {
                Status = "fail",
                Message = "District already found",
                Payload = null,
                isError = true

            };
        }
        var added = await _districtRepository.AddDistrict(district);
        _unitOfWork.Commit();

        return new Response<District>()
        {
            Status = "Success",
            Message = "District added succesfully",
            Payload = added,
            isError = false

        };

    }
    public async Task<Response<List<object>>> GetAllAvailableDistricts()
    {
        var spec = new DistrictWithProvidersSpecification();
        var districts = await _districtRepository.GetAllDistricts(spec);
        var available = districts.Where(d => d.Availability == true).Select(d => new
        {
            d.DistrictID,
            d.DistrictName,
            providers = d.ProviderDistricts.Select(p => new
            {
                p.ProviderID,
                p.enable
            }).ToList<object>()

        }).ToList<object>();
        if (available == null)
        {
            return new Response<List<object>>()
            {
                Status = "failed",
                Message = "no Available Districts",
                Payload = null,
                isError = true

            };
        }
        return new Response<List<object>>()
        {
            Status = "Success",

            Payload = available,
            isError = false

        };


    }



    public async Task<Response<object>> GetMatched(Order order, List<string> services, Provider provider)
    {
        decimal newPrice = 0;
        var specProvider = new ProviderWithDetailsSpecification(provider.Id);
        var filteredProvider = await _providerRepository.FindByIdAsync(specProvider);


        var originalSlot = await _providerService.getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);

        List<RequestedService> requestedServices = new List<RequestedService>();
        foreach (var serviceId in services)
        {
            var ps = provider.ProviderServices.FirstOrDefault(p => p.ServiceID == serviceId);
            newPrice += ps.Price * 1.12m;

            var spec = new BaseSpecifications<Service>(s => s.ServiceID == serviceId);
            var service = await _serviceRepository.GetServiceById(spec);
            var requestedService = new RequestedService
            {
                ServiceId = serviceId,
                Service = service,

                CartId = order.Customer.CartID
            };
            await _serviceRepository.AddRequestedService(requestedService);

            requestedServices.Add(requestedService);
        }


        order.OrderStatus = OrderStatusEnum.ReAssigned;







        var newRequest = new CartServiceRequest
        {
            CartID = order.Customer.CartID,
            Cart = order.Customer.Cart,
            Provider = provider,
            ProviderID = provider.Id,
            RequestedServices = requestedServices,
            providerDistrict = order.OrderDetails.providerDistrict,
            ProviderDistrictID = order.OrderDetails.ProviderDistrictID,
            RequestedDate = order.OrderDetails.RequestedSlot.RequestedDay,
            Slot = originalSlot,
            SlotID = originalSlot.TimeSlotID,
            Address = order.OrderDetails.Address,
            ProblemDescription = order.OrderDetails.ProblemDescription,
            Price = newPrice,
            ReAssigned = true
        };


        await _customerRepository.AddRequest(newRequest);


        var message = new EmailDto(order.Customer.Email!, "Sarvicny: Order is ReAssigned  ", $"We provided new suitable provider to serve your early canceled order, please check your cart so you can order again");

        _emailService.SendEmail(message);
        _unitOfWork.Commit();

        var output = new
        {
            providerId = provider.Id,
            newCartRequestId = newRequest.CartServiceRequestID

        };
        return new Response<object>()
        {
            Message = "Order Is reAssigned with new Provider",
            Payload = output,
            isError = false

        };

    }
    public async Task<Response<object>> GetSuggestion1(Order order, List<string> services, Provider provider)
    {
        decimal newPrice = 0;

        var specProvider = new ProviderWithDetailsSpecification(provider.Id);
        var filteredProvider = await _providerRepository.FindByIdAsync(specProvider);

        var today = DateTime.UtcNow.DayOfWeek;

        // Find the next availability that is after today
        var selectedAvailability = filteredProvider.Availabilities.FirstOrDefault(); //already al youm ali ana 3aizah

        var selectedSlot = selectedAvailability.Slots.Where(s => s.isActive == true).OrderBy(s => s.StartTime).FirstOrDefault();


        List<RequestedService> requestedServices = new List<RequestedService>();
        foreach (var serviceId in services)
        {
            var ps = provider.ProviderServices.FirstOrDefault(p => p.ServiceID == serviceId);
            newPrice += ps.Price * 1.12m;

            var spec = new BaseSpecifications<Service>(s => s.ServiceID == serviceId);
            var service = await _serviceRepository.GetServiceById(spec);
            var requestedService = new RequestedService
            {
                ServiceId = serviceId,
                Service = service,

                CartId = order.Customer.CartID
            };
            await _serviceRepository.AddRequestedService(requestedService);

            requestedServices.Add(requestedService);
        }


        order.OrderStatus = OrderStatusEnum.ReAssigned;


        var newRequest = new CartServiceRequest
        {
            CartID = order.Customer.CartID,
            Cart = order.Customer.Cart,
            Provider = provider,
            ProviderID = provider.Id,
            RequestedServices = requestedServices,
            providerDistrict = order.OrderDetails.providerDistrict,
            ProviderDistrictID = order.OrderDetails.ProviderDistrictID,
            RequestedDate = order.OrderDetails.RequestedSlot.RequestedDay,
            Slot = selectedSlot,
            SlotID = selectedSlot.TimeSlotID,
            Address = order.OrderDetails.Address,
            ProblemDescription = order.OrderDetails.ProblemDescription,
            Price = newPrice,
            ReAssigned = true
        };


        await _customerRepository.AddRequest(newRequest);


        var message = new EmailDto(order.Customer.Email!, "Sarvicny: Order is ReAssigned  ", $"We provided new suitable provider to serve your early canceled order, please check your cart so you can order again");

        _emailService.SendEmail(message);
        _unitOfWork.Commit();

        var output = new
        {
            providerId = provider.Id,
            newCartRequestId = newRequest.CartServiceRequestID

        };
        return new Response<object>()
        {
            Message = "Order Is reAssigned with new Provider",
            Payload = output,
            isError = false

        };

    }

    public async Task<Response<object>> GetSuggestion2(Order order, List<string> services, Provider provider)
    {
        decimal newPrice = 0;
        var specProvider = new ProviderWithDetailsSpecification(provider.Id);
        var filteredProvider = await _providerRepository.FindByIdAsync(specProvider);

        var today = DateTime.UtcNow.DayOfWeek;

        // Find the next availability that is after today

        var requestDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), order.OrderDetails.RequestedSlot.DayOfWeek);

        var selectedAvailability = filteredProvider.Availabilities
         .OrderBy(a =>
         {
             var availabilityDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), a.DayOfWeek);
             var difference = ((int)availabilityDay - (int)requestDay + 7) % 7;

             return difference;
         }).FirstOrDefault();
        var selectedSlot = selectedAvailability.Slots.Where(s => s.isActive == true).OrderBy(s => s.StartTime).FirstOrDefault();

        // Calculate the nearest date matching the availability's day of the week
        var availabilityDay = ((DayOfWeek)Enum.Parse(typeof(DayOfWeek), selectedAvailability.DayOfWeek));
        var daysUntilNextAvailability = ((int)availabilityDay - (int)requestDay + 7) % 7;
        var nearestAvailabilityDate = order.OrderDetails.RequestedSlot.RequestedDay.AddDays(daysUntilNextAvailability);


        List<RequestedService> requestedServices = new List<RequestedService>();
        foreach (var serviceId in services)
        {
            var ps = provider.ProviderServices.FirstOrDefault(p => p.ServiceID == serviceId);
            newPrice += ps.Price * 1.12m;

            var spec = new BaseSpecifications<Service>(s => s.ServiceID == serviceId);
            var service = await _serviceRepository.GetServiceById(spec);
            var requestedService = new RequestedService
            {
                ServiceId = serviceId,
                Service = service,

                CartId = order.Customer.CartID
            };
            await _serviceRepository.AddRequestedService(requestedService);

            requestedServices.Add(requestedService);
        }


        order.OrderStatus = OrderStatusEnum.ReAssigned;

        var newRequest = new CartServiceRequest
        {
            CartID = order.Customer.CartID,
            Cart = order.Customer.Cart,
            Provider = provider,
            ProviderID = provider.Id,
            RequestedServices = requestedServices,
            providerDistrict = order.OrderDetails.providerDistrict,
            ProviderDistrictID = order.OrderDetails.ProviderDistrictID,
            RequestedDate = nearestAvailabilityDate,
            Slot = selectedSlot,
            SlotID = selectedSlot.TimeSlotID,
            Address = order.OrderDetails.Address,
            ProblemDescription = order.OrderDetails.ProblemDescription,
            Price = newPrice,
            ReAssigned = true
        };

        await _customerRepository.AddRequest(newRequest);


        var message = new EmailDto(order.Customer.Email!, "Sarvicny: Order is ReAssigned  ", $"We provided new suitable provider to serve your early canceled order, please check your cart so you can order again");

        _emailService.SendEmail(message);
        _unitOfWork.Commit();

        var output = new
        {
            providerId = provider.Id,
            newCartRequestId = newRequest.CartServiceRequestID

        };
        return new Response<object>()
        {
            Message = "Order Is reAssigned with new Provider",
            Payload = output,
            isError = false

        };

    }

    public bool ProviderIsCheaper(Order order, List<string> services, Provider provider)
    {
        var orderPrice = order.OrderDetails.Price;

        var totalProviderPrice = provider.ProviderServices
                         .Where(ps => services.Contains(ps.ServiceID))
                         .Sum(ps => ps.Price);

        if (totalProviderPrice > orderPrice)
        {

            return false;
        }
        return true;

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
        if (order.OrderStatus != OrderStatusEnum.CanceledByProvider)
        {
            return new Response<object>()
            {
                Status = "failed",
                Message = "Order is Not Canceled by Provider",
                Payload = null,
                isError = true

            };
        }

        var services = order.OrderDetails.RequestedServices.Select(r => new
        {
            r.ServiceId,
            r.Service.ServiceName
        }).ToList();

        List<string> servicesIds = new List<string>();
        foreach (var service in services)
        {
            servicesIds.Add(service.ServiceId);
        }

        var requestedSlot = order.OrderDetails.RequestedSlot;
        var allowedRange = DateTime.UtcNow.AddHours(2).TimeOfDay;

        var startTime = requestedSlot.StartTime;
        var dayOfweek = requestedSlot.DayOfWeek;
        var district = order.OrderDetails.providerDistrict.DistrictID;
        var customer = order.Customer;

        var provider = order.OrderDetails.ProviderID;


        var suggestion2 = await _providerRepository.SuggestionLevel2(servicesIds, district, customer.Id);
        var filteredSuggest2Providers = suggestion2.Where(p => p.Id != provider).ToList();

        if (!filteredSuggest2Providers.Any())
        {
            order.OrderStatus = OrderStatusEnum.RemovedWithRefund;

            var orderDetailsForCustomer = HelperMethods.GenerateOrderDetailsMessageForCustomer(order);
            var message = new EmailDto(customer.Email!, "Sarvicny: Order is removed ", $"Sorry your order is removed due to failure in finding new suitable provider applicable to your order, wait for your Refund. \n\nOrder Details:\n{orderDetailsForCustomer}");


            _emailService.SendEmail(message);
            _unitOfWork.Commit();
            return new Response<object>()
            {
                Status = "failed",
                Message = "No suitable Provider Found",
                Payload = null,
                isError = true

            };
        }

        var favoriteProviderIds = customer.Favourites?.Select(f => f.providerId).ToHashSet();

        if (order.OrderDetails.RequestedSlot.RequestedDay == DateTime.Today && startTime <= allowedRange)
        {
            var provider2 = filteredSuggest2Providers.FirstOrDefault(); // may be fav

            if (ProviderIsCheaper(order, servicesIds, provider2))
            {
                return await GetSuggestion2(order, servicesIds, provider2);
            }

            if (favoriteProviderIds.Contains(provider2.Id))
            {

                var another = filteredSuggest2Providers.FirstOrDefault(p => !favoriteProviderIds.Contains(p.Id)); // not fav
                                                                                                                  // Handle case where there is no non-favorite provider available
                if (another != null)
                {
                    return await GetSuggestion2(order, servicesIds, another);
                }
                return await GetSuggestion2(order, servicesIds, provider2); // a8la bs mafesh 8er fav

            }
            return await GetSuggestion2(order, servicesIds, provider2); // a8la and not fav

        }


        var suggestion1 = await _providerRepository.SuggestionLevel1(servicesIds, dayOfweek, district, customer.Id);
        var filteredSuggest1Providers = suggestion1.Where(p => p.Id != provider).ToList();


        if (!filteredSuggest1Providers.Any()) // not found in the same dayofWeek
        {
            var provider2 = filteredSuggest2Providers.FirstOrDefault();
            if (ProviderIsCheaper(order, servicesIds, provider2))
            {
                return await GetSuggestion2(order, servicesIds, provider2);
            }

            if (favoriteProviderIds.Contains(provider2.Id))
            {

                var another = filteredSuggest2Providers.FirstOrDefault(p => !favoriteProviderIds.Contains(p.Id)); // not fav
                                                                                                                  // Handle case where there is no non-favorite provider available
                if (another != null)
                {
                    return await GetSuggestion2(order, servicesIds, another);
                }
                return await GetSuggestion2(order, servicesIds, provider2); // a8la bs mafesh 8er fav

            }
            return await GetSuggestion2(order, servicesIds, provider2); // a8la and not fav

        }

        var matched = await _providerRepository.GetAllMatchedProviders(servicesIds, startTime, dayOfweek, district, customer.Id);
        var FilteredMatchedProviders = matched.Where(p => p.Id != provider).ToList();

        if (!FilteredMatchedProviders.Any())
        {
            var provider1 = filteredSuggest1Providers.FirstOrDefault();
            if (ProviderIsCheaper(order, servicesIds, provider1))
            {
                return await GetSuggestion2(order, servicesIds, provider1);
            }

            if (favoriteProviderIds.Contains(provider1.Id))
            {

                var another = filteredSuggest2Providers.FirstOrDefault(p => !favoriteProviderIds.Contains(p.Id)); // not fav
                                                                                                                  // Handle case where there is no non-favorite provider available
                if (another != null)
                {
                    return await GetSuggestion2(order, servicesIds, another);
                }
                return await GetSuggestion2(order, servicesIds, provider1); // a8la bs mafesh 8er fav

            }
            return await GetSuggestion2(order, servicesIds, provider1); // a8la and not fav

        }


        var provider0 = FilteredMatchedProviders.FirstOrDefault();
        if (ProviderIsCheaper(order, servicesIds, provider0))
        {
            return await GetSuggestion2(order, servicesIds, provider0);
        }

        if (favoriteProviderIds.Contains(provider0.Id))
        {

            var another = filteredSuggest2Providers.FirstOrDefault(p => !favoriteProviderIds.Contains(p.Id)); // not fav
                                                                                                              // Handle case where there is no non-favorite provider available
            if (another != null)
            {
                return await GetSuggestion2(order, servicesIds, another);
            }
            return await GetSuggestion2(order, servicesIds, provider0); // a8la bs mafesh 8er fav

        }
        return await GetSuggestion2(order, servicesIds, provider0); // a8la and not fav



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

        if (order.OrderStatus == OrderStatusEnum.Completed)
        {
            return new Response<object>()

            {
                isError = false,
                Payload = null,
                Message = "Order Already Completed",
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

        var provider = order.OrderDetails.Provider;
        var wallet = provider.Wallet;



        if (wallet is null)
        {
            provider.Wallet = new ProviderWallet
            {
                ProviderId = provider.Id,
            };
            wallet = provider.Wallet;
        }
        if (order.TransactionPayment.PaymentMethod == PaymentMethod.Paypal || order.TransactionPayment.PaymentMethod == PaymentMethod.Paymob)
        {
            wallet.PendingBalance = Math.Ceiling(order.OrderDetails.Price / (1 + 0.12m));

        }
        else
        {
            wallet.HandedBalance = Math.Ceiling(order.OrderDetails.Price / (1 + 0.12m));
        }

        _unitOfWork.Commit();

        return new Response<object>()
        {
            isError = false,
            Message = "Action Done Successfully",
            Errors = null,
        };
    }

    public async Task<Response<object>> MarkFraud(string orderId)
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
        order.OrderStatus = OrderStatusEnum.CanceledByProvider;

        var originalSlot = await _providerService.getOriginalSlot(order.OrderDetails.RequestedSlot, order.OrderDetails.ProviderID);
        if (originalSlot != null)
        {
            originalSlot.isActive = true;
        }
        var message = new EmailDto(order.OrderDetails.Provider.Email!, "Sarvicny: Fraud Detected", $" We have noticed that you marked order whose ID is: {order.OrderID} Done without actually doing the job this may results in being blocked");

        _emailService.SendEmail(message);
        _unitOfWork.Commit();
        var result = new
        {
            providerId = order.OrderDetails.ProviderID,
            order = order.OrderID
        };

        return new Response<object>()
        {
            isError = false,
            Message = "Order is canceled and ready To be reassigned",
            Errors = null,
            Payload = result
        };

    }

    public async Task<Response<object>> GetCriminalRecordFileForWorker(string providerId)
    {
        var worker = await _providerRepository.FindWorkerByIdAsync(providerId);

        if (worker == null)
        {
            return new Response<object>
            {
                isError = true,
                Errors = null,
                Payload = null,
                Message = "failed",
            };
        }
        if (worker.CriminalRecord == null)
        {
            return new Response<object>
            {
                isError = true,
                Errors = null,
                Payload = null,
                Message = "No file found",
            };
        }
        try
        {
            var providerFolder = Path.Combine(Environment.CurrentDirectory, "App_Data", "WorkersData", providerId);
            var filePath = Path.Combine(providerFolder, "CriminalRecord");

            if (File.Exists(filePath))
            {
                // Read the file as bytes
                byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

                // Convert bytes to base64 string
                string base64String = Convert.ToBase64String(fileBytes);

                var result = new
                {
                    base64String
                };
                return new Response<object>
                {
                    isError = false,
                    Payload = base64String,
                    Message = "success",
                };
            }
            else
            {
                return new Response<object>
                {
                    isError = true,

                    Payload = null,
                    Message = "file does not exist",
                };
            }
        }
        catch (Exception ex)
        {
            return new Response<object>
            {
                isError = true,

                Payload = null,
                Message = $"{ex.Message}",
            };

        }
    }

    public async Task<Response<List<object>>> getAllTransactionsNeedRefund()
    {
        var spec = new TransactionPaymentWithDetailsSpecification();

        var transactions= await _transactionPaymentRepository.getAllRefundableTransactions(spec);

        List<object> result = new List<object>();

        foreach (var transaction in transactions)
        {
            List<object> ordersDetails = new List<object>();

            var orders = transaction.OrderList.Where(o => o.OrderStatus == OrderStatusEnum.Canceled || o.OrderStatus == OrderStatusEnum.RemovedWithRefund || o.OrderStatus == OrderStatusEnum.ReAssigned).ToList();
            foreach (var order in orders)
            {
                var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
                ordersDetails.Add(orderDetails);
            }

            var transactionWithOrders = new 
            {
                TransactionPaymentID = transaction.TransactionPaymentID,
                OrdersDetails = ordersDetails
            };
            

            result.Add(transactionWithOrders);
          

        }

        return new Response<List<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = result,

        };
    }
}