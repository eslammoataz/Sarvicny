using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IServiceProviderService
    {
        Task<Response<object>> RegisterServiceAsync(string workerId, string serviceId, decimal Price); //done
        Task<Response<ICollection<object>>> GetRegisteredServices(string workerId); //done

        Task<Response<object>> AddAvailability(AvailabilityDto availabilityDto, string providerId); //done

        Task<Response<List<object>>> getAvailability(string workerId); //done

        //Task<Response<Object>> ShowOrderDetails(string orderId); //done  //b2t fe orderService

        Task<Response<object>> ApproveOrder(string orderId); //done
        Task<Response<object>> RejectOrder(string orderId); //done
        Task<Response<object>> CancelOrder(string orderId); //done



        ////Task<Response<ICollection<Object>>> GetAllServiceProviders(); b2t fe al admin bs

        Task<Response<List<object>>> getAllOrders(string workerId);

        Task<Response<List<object>>> getAllApprovedOrders(string workerId);
        Task<Response<List<object>>> getAllRequestedOrders(string workerId);
        Task<Response<object>> getRegisteredServices(string providerId);
    }
}
