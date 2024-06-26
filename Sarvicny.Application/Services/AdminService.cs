using MailKit.Search;
using Microsoft.AspNetCore.Identity;
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

    private readonly IUnitOfWork _unitOfWork;

    private readonly IOrderService _orderService;

    public AdminService(UserManager<User> userManager, IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepositor, IAdminRepository adminRepository, IOrderRepository orderRepository, IEmailService emailService, IOrderService orderService, IDistrictRepository districtRepository)
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

    public async Task<Response<Provider>> ApproveServiceProviderRegister(string workerId)
    {

        var provider = await _userRepository.GetUserByIdAsync(workerId);
        if (provider == null)
        {
            return new Response<Provider>()
            {
                Status = "Fail",
                Message = "Provider Not Found",
                Payload = null,

            };

        }


        //Add Token to Verify the email....
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(provider);
        var output = await _adminRepository.ApproveServiceProviderRegister(workerId);
        _unitOfWork.Commit();

        var message = new EmailDto(provider.Email!, "Sarvicny: Worker Approved Successfully", "Congratulations you are accepted");

        _emailService.SendEmail(message);
        return new Response<Provider>()
        {
            Status = "Success",
            Message = "Worker Approved Successfully , Verification Email sent to provider's email ",
            Payload = output,

        };


    }

    public async Task<Response<Provider>> RejectServiceProviderRegister(string workerId)
    {
        var provider = await _userRepository.GetUserByIdAsync(workerId);
        if (provider == null)
        {
            return new Response<Provider>()
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
        return new Response<Provider>()
        {
            Status = "Success",
            Message = "Worker Rejected Successfully , Verification Email sent to provider's email ",
            Payload = await _adminRepository.RejectServiceProviderRegister(workerId),
            isError = false

        };
    }

    public async Task<Response<List<object>>> GetServiceProvidersRegistrationRequests()
    {
        var spec = new ServiceProviderWithService_DistrictSpecificationcs();
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

            //districts=p.ProviderDistricts.Select(d => new
            //{
            //    d.DistrictID, d.District.DistrictName,

            //}).ToList(),
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
                Status = "failed",
                Message = "No Orders Found",
                Payload = null,
                isError = true
            };
        }

        return new Response<List<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = result,

        };



    }

    public async Task<Response<List<object>>> getAllPendingOrders()
    {
        var spec = new OrderWithDetailsSpecification();
        var pending = await _orderRepository.GetAllPendingOrders(spec);

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
                Status = "failed",
                Message = "No Rejected Orders Found",
                Payload = null,
                isError = true
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

    public async Task<Response<List<object>>> getAllApprovedOrders()
    {
        
            var spec = new OrderWithDetailsSpecification();
            var approvedOrders = await _orderRepository.GetAllApprovedOrders(spec);

            List<object> result = new List<object>();
            foreach (var order in approvedOrders)
            {
                var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
                result.Add(orderDetails);
            }

            if (result.Count == 0)
            {
                return new Response<List<object>>()
                {
                    Status = "failed",
                    Message = "No Approved Orders Found",
                    Payload = null,
                    isError = true
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

    public async Task<Response<List<object>>> getAllRejectedOrders()
    {
        var spec = new OrderWithDetailsSpecification();
        var Rejected = await _orderRepository.GetAllRejectedOrders(spec);

        List<object> result = new List<object>();
        foreach (var order in Rejected)
        {
            var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
            result.Add(orderDetails);
        }

        if (result.Count == 0)
        {
            return new Response<List<object>>()
            {
                Status = "failed",
                Message = "No Rejected Orders Found",
                Payload = null,
                isError = true
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

    public async Task<Response<List<object>>> getAllExpiredOrders()
    {
        var spec = new OrderWithDetailsSpecification();
        var Expired = await _orderRepository.GetAllExpiredOrders(spec);

        List<object> result = new List<object>();
        foreach (var order in Expired)
        {
            var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
            result.Add(orderDetails);
        }

        if (result.Count == 0)
        {
            return new Response<List<object>>()
            {
                Status = "failed",
                Message = "No Expired Orders Found",
                Payload = null,
                isError = true
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
    public async Task<Response<List<object>>> RemoveAllPaymentExpiredOrders()
    {
        var spec = new OrderWithDetailsSpecification();
        var expired = await _orderRepository.RemoveAllPaymentExpiredOrders(spec);


        List<object> result = new List<object>();

        foreach (var order in expired)
        {
            order.OrderStatus = OrderStatusEnum.Removed;
            var orderDetails = await _orderService.ShowAllOrderDetailsForAdmin(order.OrderID);
            result.Add(orderDetails);
        }
        _unitOfWork.Commit();


       
        
        if (result.Count == 0)
        {
            return new Response<List<object>>()
            {
                Status = "failed",
                Message = "No Expired Orders Found",
                Payload = null,
                isError = true
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
    public async Task<Response<List<object>>> getAllCanceledOrders()
    {
        var spec = new OrderWithDetailsSpecification();
        var Canceled = await _orderRepository.GetAllCanceledOrders(spec);

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
                Status = "failed",
                Message = "No Canceled Orders Found",
                Payload = null,
                isError = true
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

        var provider = order.OrderDetails.ProviderID;

        var matchedProviders = await _providerRepository.GetAllMatchedProviders(servicesIds, startTime, dayOfweek, district, customer.Id);

        var filteredProviders = matchedProviders.Where(p => p.Id != provider).ToList();
        order.OrderStatus = OrderStatusEnum.Removed;

        _unitOfWork.Commit();


        var orderDetails = await _orderService.ShowAllOrderDetailsForCustomer(orderId);


        if (filteredProviders.Count() == 0)
        {

            var orderDetailsForCustomer = _orderService.GenerateOrderDetailsMessage(order);
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
        List<object> providers = new List<object>();  
        foreach(var matched in filteredProviders)
        {

            var newfiltered = new 
            { 
                providerId = matched.Id,
                firstName = matched.FirstName,
                lastName= matched.LastName,
                
            };
            providers.Add(newfiltered);

        }
        var result = new
        {
           orderDetails = orderDetails,
            matchedProviders = provider,

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

}