using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.ServiceSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services;

public class ServicesServices : IServicesService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    public ServicesServices(IServiceRepository serviceRepository, IUnitOfWork unitOfWork, IServiceProviderRepository serviceProviderRepository)
    {
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
        _serviceProviderRepository = serviceProviderRepository;
    }

    public async Task<Response<ICollection<object>>> GetAllServices()
    {
        var spec = new ServiceWithCriteriaSpecification();
        var services = await _serviceRepository.GetAllServices(spec);

        var servicesAsObjects = services.ToList<object>();

        var response = new Response<ICollection<object>>();
        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = servicesAsObjects;

        return response;
    }

    public async Task<Response<Service>> GetServiceById(string serviceId)
    {
        var spec = new ServiceWithCriteriaSpecification(serviceId);
        var service = await _serviceRepository.GetServiceById(spec);
        var response = new Response<Service>();
        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = service;

        return response;
    }

    public async Task<Response<Service>> GetServiceById(string serviceId, ISpecifications<Service> specifications)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Service>> UpdateService(Service service)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Service>> DeleteService(string serviceId)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Service>> AddServiceAsync(Service newService)
    {
        var response = new Response<Service>();
        await _serviceRepository.AddServiceAsync(newService);
        _unitOfWork.Commit();
        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = newService;

        return response;
    }


    public async Task<Response<ICollection<object>>> GetAllWorkersForService(string serviceId)
    {
        var response = new Response<ICollection<object>>();

        var spec = new ServiceWithProvidersSpecification(serviceId);

        var service = await _serviceRepository.GetServiceById(spec);

        if (service == null)
        {
            response.Status = "Fail";
            response.Message = "Service Not Found";
            response.Payload = new List<object>();
            response.isError = false;
            return response;
        }

        var providers = new List<object>();

        foreach (var p in service.ProviderServices)
        {
            var spec3 = new BaseSpecifications<Provider>(pr => pr.Id == p.ProviderID);
            var provider = await _serviceProviderRepository.FindByIdAsync(spec3);
            var serviceProvidersAsObjects = new
            {
                provider.Id,
                provider.FirstName,
                provider.LastName,
                provider.Email,
                provider.PhoneNumber,
                provider.isVerified,
            };

            providers.Add(serviceProvidersAsObjects);
        }

        if (providers.Count == 0)
        {
            return new Response<ICollection<object>>()
            {
                Status = "Fail",
                Message = "No Providers Found",
                Payload = providers,
                isError = false,
            };
        }

        return new Response<ICollection<object>>()
        {
            Message = "Action Done Successfully",
            Payload = providers,
            isError = false,
        };


    }
}