﻿using Microsoft.AspNetCore.Identity;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.OrderSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;

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

    public AdminService(UserManager<User>userManager ,IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepositor, IAdminRepository adminRepository, IOrderRepository orderRepository, IEmailService emailService)
    {
        _userManager = userManager;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _providerRepository = serviceProviderRepositor;
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _emailService = emailService;
    }

    public async Task<Response<ICollection<object>>> GetAllCustomers()
    {
        var customers = await _userRepository.GetAllCustomers();

        var customersAsObjects = customers.Select(c => new
        {
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email
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
        var serviceProviders = await _userRepository.GetAllServiceProviders();

        var serviceProvidersAsObjects = serviceProviders.Select(c => new
        {
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email,
            c.isVerified
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

    public async Task<Response<ICollection<Provider>>> GetServiceProvidersRegistrationRequests()
    {
        var unHandledProviders = await _providerRepository.GetProvidersRegistrationRequest();

        var response = new Response<ICollection<Provider>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = unHandledProviders,
            isError = false
        };

        return response;
    }

    public async Task<Response<List<object>>> getAllOrders()
    {
        var spec = new OrderWithProviderServiceSpecification();
        var orders = await _orderRepository.GetAllOrders(spec);


        var ordersAsobject = orders.Select(o => new
        {
            o.OrderID,
            o.CustomerID,
            o.Customer.FirstName,
            o.OrderStatus.StatusName,
            providerservice = o.Customer.Cart.ServiceRequests.Select(async s => new
            {

                s.providerService.ProviderID,
                s.providerService.Provider.FirstName,
                s.providerService.Provider.LastName,
                s.providerService.Service.ServiceID,
                s.providerService.Service.ServiceName,
                s.SlotID,
                s.Slot.StartTime

            }).ToList<object>(),
            o.TotalPrice,


        }).ToList<object>();

        return new Response<List<object>>()
        {
            Status = "Success",
            Message = "Action Done Successfully",
            Payload = ordersAsobject,

        };

    }

    public async Task<Response<List<object>>> getAllRequestedOrders()
    {
        var spec = new OrderWithProviderServiceSpecification();
        var orders = await _orderRepository.GetAllOrders(spec);
       
       

        List<object> result = new List<object>();
        foreach (var order in orders)
        {
            if (order.OrderStatusID == "1") // 1 means set
            {

                
                    var ordersAsobject = new
                    {
                        orderId = order.OrderID,
                        customerId = order.Customer.Id,
                        customerFN = order.Customer.FirstName,
                        orderStatus = order.OrderStatus.StatusName,
                        orderPrice = order.TotalPrice,
                        providerservice = order.Customer.Cart.ServiceRequests.Select(s => new
                        {

                            s.providerService.Provider.Id,
                            s.providerService.Provider.FirstName,
                            s.providerService.Provider.LastName,
                            s.providerService.Service.ServiceID,
                            s.providerService.Service.ServiceName,
                            s.SlotID,
                            //s.Slot.StartTime
                        }).ToList<object>(),

                    };
                    result.Add(ordersAsobject);


                
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
        var spec = new OrderWithProviderServiceSpecification();
        var orders = await _orderRepository.GetAllOrders(spec);
        

        List<object> result = new List<object>();
        foreach (var order in orders)
        {
            if (order.OrderStatusID == "2") // 2 means approved
            {


                var ordersAsobject = new
                {
                    orderId = order.OrderID,
                    customerId = order.Customer.Id,
                    customerFN = order.Customer.FirstName,
                    orderStatus = order.OrderStatus.StatusName,
                    orderPrice = order.TotalPrice,
                    providerservice = order.Customer.Cart.ServiceRequests.Select(s => new
                    {

                        s.providerService.Provider.Id,
                        s.providerService.Provider.FirstName,
                        s.providerService.Provider.LastName,
                        s.providerService.Service.ServiceID,
                        s.providerService.Service.ServiceName,
                        s.SlotID,
                        //s.Slot.StartTime
                    }).ToList<object>(),

                };
                result.Add(ordersAsobject);



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
}