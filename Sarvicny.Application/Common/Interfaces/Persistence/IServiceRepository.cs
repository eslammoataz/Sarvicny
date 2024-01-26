using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence;

public interface IServiceRepository
{
    
    Task<ICollection<Service>> GetAllServices(ISpecifications<Service> specifications);
    Task<Service> GetServiceById(ISpecifications<Service> specifications);
    Task<Service> UpdateService(Service service);
    Task<Service> DeleteService(string serviceId);
    Task AddServiceAsync(Service newService);
}