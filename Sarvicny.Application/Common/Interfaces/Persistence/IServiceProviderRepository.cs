using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IServiceProviderRepository
    {
        Task<Provider> FindByIdAsync(ISpecifications<Provider> specifications);
        Task<bool> WorkerExists(string workerId);
        Task AddProviderService(ProviderService workerService);
        Task<bool> IsServiceRegisteredForWorker(string workerId, string serviceId);
        Task<ICollection<object>> GetRegisteredServices(ISpecifications<Provider> specifications);
        Task<object> AddAvailability(AvailabilityDto availabilityDto, string providerId);
        Task<List<TimeSlot>> ConverttoTimeSlot(List<TimeRange> timeRanges, ProviderAvailability providerAvailability);
        Task<List<object>> getAvailability(string providerId, ISpecifications<Provider> spec);
        Task<object> AddAvailabilitySlots(TimeSlotDto slotDto, string availabilityId);

        Task<ICollection<Provider>> GetProvidersRegistrationRequest();


    }
}
