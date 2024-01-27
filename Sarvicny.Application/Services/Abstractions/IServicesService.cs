using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions;

public interface IServicesService
{
    Task<Response<ICollection<object>>> GetAllServices();
    Task<Response<Service>> GetServiceById(string serviceId);
    Task<Response<Service>> AddServiceAsync(Service newService);
    Task<Response<ICollection<object>>> GetAllWorkersForService(string serviceId);
}