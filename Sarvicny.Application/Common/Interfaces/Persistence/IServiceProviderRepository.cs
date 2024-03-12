using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IServiceProviderRepository
    {
        Task<Provider?> FindByIdAsync(ISpecifications<Provider> specifications);
        Task<bool> WorkerExists(string workerId);
        Task AddProviderService(ProviderService workerService);
        Task<bool> IsServiceRegisteredForWorker(string workerId, string serviceId);
        Task<ICollection<object>> GetRegisteredServices(ISpecifications<Provider> specifications);
        Task<ProviderAvailability> AddAvailability(AvailabilityDto availabilityDto, ISpecifications<Provider> specifications);
        Task <List<ProviderAvailability>> getAvailability( ISpecifications<Provider> spec);
        Task<List<TimeSlot>> getAvailabilitySlots(ISpecifications<ProviderAvailability> spec);


        Task<ICollection<Provider>> GetAllServiceProviders(ISpecifications<Provider> spec);
        Task<ICollection<Provider>> GetProvidersRegistrationRequest(ISpecifications<Provider> spec);

        
       
    }
}
