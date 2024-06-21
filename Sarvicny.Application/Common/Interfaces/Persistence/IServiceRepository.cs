using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence;

public interface IServiceRepository
{
    Task<ICollection<Service>> GetAllServices(ISpecifications<Service> spec);
    Task<ICollection<Service>> GetAllParentServices(ISpecifications<Service> spec);
    Task<ICollection<Provider>> GetServiceById(string serviceId);
    Task<Service?> GetServiceById(ISpecifications<Service> specifications);
    Task<Service> UpdateService(Service service);
    Task<Service> DeleteService(string serviceId);
    Task AddServiceAsync(Service newService);
    Task AddRequestedService(RequestedService requestedService);
}