using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Domain.Specification;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IServiceProviderService
    {
        Task<Response<List<object>>> RegisterServiceAsync(string workerId, string serviceId, decimal Price); //done
        Task<Response<ICollection<object>>> GetRegisteredServices(string workerId); //done

        Task<Response<object>> AddAvailability(AvailabilityDto availabilityDto,string providerId); //done

        Task<Response<List<object>>> getAvailability(string workerId); //done

        Task<Response<Object>> ShowOrderDetails(string orderId); //done //na2s 7ta fy elspecification

        Task<Response<object>> ApproveOrder(string orderId); //done
        Task<Response<object>> RejectOrder(string orderId); //done
        Task<Response<object>> CancelOrder(string orderId); //done
        Task<Response<ICollection<Object>>> GetAllServiceProviders(); //done
       


    }
}
