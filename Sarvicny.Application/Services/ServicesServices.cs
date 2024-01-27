using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Specifications.ProviderServiceSpecifications;
using Sarvicny.Application.Services.Specifications.ServiceSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services;

public class ServicesServices : IServicesService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ServicesServices(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
    {
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
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


    public async Task<Response<object>> GetAllWorkersForService(string serviceId)
    {
        var specPS = new ProviderServiceWithProviderSpecification(serviceId);

        var specS= new ServiceWithProvidersSpecification(serviceId);
        var service = await _serviceRepository.GetServiceById(specS);

        if (service == null)
        {
            return new Response<object>()
            {
                Status = "Fail",
                Message = "Service Not Found",
                Payload = null,
                isError = true,
            };
        }
        return new Response<object>()
        {
            
            Message = "Service Not Found",
            Payload = _serviceRepository.GetAllWorkersForService(specPS),
            isError = false,
        };


    }
}