using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Abstractions;

public interface IServiceService
{
    Task<Response<ICollection<object>>> GetAllServices();
    Task<Response<Service>> GetServiceById(string serviceId);
    Task<Response<Service>> UpdateService(Service service);
    Task<Response<Service>> DeleteService(string serviceId);
    Task<Response<Service>> AddServiceAsync(Service newService);
}