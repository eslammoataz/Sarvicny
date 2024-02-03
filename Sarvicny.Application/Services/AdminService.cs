﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Contracts;
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

    private readonly IUnitOfWork _unitOfWork;

    private readonly IOrderService _orderService;

    public AdminService(UserManager<User>userManager ,IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepositor, IAdminRepository adminRepository, IOrderRepository orderRepository, IEmailService emailService,IOrderService orderService)
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
        var spec = new ServiceProviderWithServiceSpecificationcs();
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
            s.AvailabilityStatus,
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
            Payload =output ,

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
        var spec = new ServiceProviderWithServiceSpecificationcs();
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
                parentServiceName=s.Service.ParentService?.ServiceName,
                s.Service.CriteriaID,
                s.Service.Criteria?.CriteriaName
            }).ToList()



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
        var spec = new OrderWithRequestsSpecification();
        var orders = await _orderRepository.GetAllOrders(spec);



        List<object> result = new List<object>();
        foreach (var order in orders)
        {
            var orderDetails = await _orderService.ShowAllOrderDetails(order.OrderID);

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

    public async Task<Response<List<object>>> getAllRequestedOrders()
    {
        var spec = new OrderWithRequestsSpecification();
        var orders = await _orderRepository.GetAllOrders(spec);
       
       

        List<object> result = new List<object>();
        foreach (var order in orders)
        {
            if (order.OrderStatusID == "1") // 1 means set
            {


                var orderDetails = await _orderService.ShowAllOrderDetails(order.OrderID);

                result.Add(orderDetails);



            }
            else { continue; }

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

    public  async Task<Response<List<object>>> getAllApprovededOrders()
    {
        var spec = new OrderWithRequestsSpecification();
        var orders = await _orderRepository.GetAllOrders(spec);
        

        List<object> result = new List<object>();
        foreach (var order in orders)
        {
            if (order.OrderStatusID == "2") // 2 means approved
            {

                var orderDetails = await _orderService.ShowAllOrderDetails(order.OrderID);

                result.Add(orderDetails);



            }
            else { continue; }

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

    public async Task<Response<bool>> BlockServiceProvider(string workerId)
    {
        var provider = await _providerRepository.FindByIdAsync(new BaseSpecifications<Provider>(p=>p.Id == workerId));
        
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
}