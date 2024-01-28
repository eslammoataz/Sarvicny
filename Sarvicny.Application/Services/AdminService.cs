﻿using Microsoft.AspNetCore.Identity;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
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

    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IUserRepository userRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepositor, IAdminRepository adminRepository)
    {
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _providerRepository = serviceProviderRepositor;
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
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
        _unitOfWork.Commit();

        //Add Token to Verify the email....
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(provider);


        var message = new EmailDto(provider.Email!, "Sarvicny: Worker Approved Successfully", "Congratulations you are accepted");

        _emailService.SendEmail(message);
        return new Response<Provider>()
        {
            Status = "Success",
            Message = "Worker Approved Successfully , Verification Email sent to provider's email ",
            Payload = await _adminRepository.ApproveServiceProviderRegister(workerId),

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

}