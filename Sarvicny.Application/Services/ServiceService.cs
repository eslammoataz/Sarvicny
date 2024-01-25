using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;

using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepository;
    public ServiceService(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Response<ICollection<object>>> GetAllServices()
    {
        var spec = new ServiceWithCriteriaSpecification();
        var services = await  _serviceRepository.GetAllServices(spec);
        
        var servicesAsObjects = services.ToList<object>();
        
        var response = new Response<ICollection<object>>();
        response.Status = "Success";
        response.Message = "Action Done Successfully";
        response.Payload = servicesAsObjects;
        
        return response;
    }

    public async Task<Response<Service>> GetServiceById(string serviceId)
    {
        var spec = new ServiceWithCriteriaSpecification();
        var service = await _serviceRepository.GetServiceById(serviceId, spec);
        var response = new Response<Service>();
        response.Payload = service;
        return response;
    }

    public async Task<Response<ICollection<object>>> GetAllServices(ISpecifications<Service> specifications)
    {
        throw new NotImplementedException();
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
        response.Payload = newService;
        return response;
    }
}