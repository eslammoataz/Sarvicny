using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Application.Services;

public class AdminService : IAdminService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IUserRepository userRepository , IServiceRepository serviceRepository , IUnitOfWork unitOfWork)
    {
        _serviceRepository = serviceRepository;
       _userRepository = userRepository;
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
        
        var servicesAsObjects = services.Select(s => new
        {
            s.ServiceID,
            s.ServiceName,
            s.AvailabilityStatus,
            s.Criteria.CriteriaName
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
        throw new NotImplementedException();
    }
}