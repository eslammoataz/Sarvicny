using Sarvicny.Application.Services;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IServiceProviderRepository
    {
        Task<Worker> FindByIdAsync(string workerId);
        Task<bool> WorkerExists(string workerId);
        Task AddProviderService(ProviderService workerService);
        Task<bool> IsServiceRegisteredForWorker(string workerId, string serviceId);
        Task<ICollection<object>> GetRegisteredServices(string workerId);
        Task<string> AddAvailability(AvailabilityDto availabilityDto, string providerId);
        Task<List<TimeSlot>> ConverttoTimeSlot(List<TimeRange> timeRanges, ProviderAvailability providerAvailability);
        Task<List<object>> getAvailability(string providerId);
        Task<object> ShowOrderDetails(string orderId);
        Task<string> ApproveOrder(string orderId);
        Task<string> RejectOrder(string orderId);
        Task<string> CancelOrder(string orderId);

    }
}
