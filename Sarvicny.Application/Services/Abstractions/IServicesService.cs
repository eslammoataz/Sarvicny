using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions;

public interface IServicesService
{
    Task<Response<List<object>>> GetAllServices();
    Task<Response<object>> GetAllChildsForService(string serviceId);
    Task<Response<Service>> GetServiceById(string serviceId);
    Task<Response<Service>> AddServiceAsync(Service newService);
    Task<Response<object>> GetAllWorkersForService(string serviceId);
}