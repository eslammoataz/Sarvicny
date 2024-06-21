using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IServiceProviderService
    {
        Task<Response<object>> RegisterServiceAsync(string workerId, string serviceId, decimal Price); //done
        

        Task<Response<object>> AddAvailability(AvailabilityDto availabilityDto, string providerId); //done

        Task<Response<object>> RemoveAvailability(string availabilityId, string providerId); 



        Task<Response<List<object>>> getAvailability(string workerId); //done

       

        Task<Response<object>> ApproveOrder(string orderRequestId); 
        Task<Response<object>> RejectOrder(string orderRequestId); 
        Task<Response<object>> CancelOrder(string orderRequestId);


        Task<Response<List<object>>> getAllOrdersForProvider(string workerId);

        Task<Response<List<object>>> getAllApprovedOrderForProvider(string workerId);
        Task<Response<List<object>>> getAllPendingOrderForProvider(string workerId);
        Task<Response<object>> getRegisteredServices(string providerId);

        Task<Response<object>> ShowProviderProfile(string workerId);

        Task<Response<object>> AddDistrictToProvider(string providerId,string District);
        Task<Response<object>> DisableDistrictFromProvider(string providerId, string District);
        Task<Response<object>> EnableDistrictToProvider(string providerId, string District);

        Task<Response<object>> RemoveDistrictFromProvider(string providerId, string District);


        Task<Response<List<object>>> GetProviderDistricts(string providerId);
       // Task<Response<object>> RequestNewDistrictToBeAdded(string districtName);
       





    }
}
