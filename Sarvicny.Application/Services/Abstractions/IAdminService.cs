using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;


namespace Sarvicny.Application.Services.Abstractions;

public interface IAdminService
{
    Task<Response<ICollection<object>>> GetAllCustomers();
    Task<Response<ICollection<object>>> GetAllServiceProviders();
    Task<Response<ICollection<object>>> GetAllServices();
    Task<Response<Provider>> ApproveServiceProviderRegister(string workerId);
    Task<Response<Provider>> RejectServiceProviderRegister(string workerId);
    Task<Response<List<object>>> GetServiceProvidersRegistrationRequests();
    Task<Response<List<object>>> getAllOrders();
    Task<Response<List<object>>> getAllPendingOrders();
    Task<Response<List<object>>> getAllApprovedOrders();

    Task<Response<List<object>>> getAllCanceledOrders();
    Task<Response<List<object>>> getAllRejectedOrders();
    Task<Response<List<object>>> getAllExpiredOrders();
    Task<Response<List<object>>> RemoveAllPaymentExpiredOrders();


    Task<Response<bool>> BlockServiceProvider(string workerId);
    Task<Response<bool>> UnBlockServiceProvider(string workerId);
    Task<Response<List<object>>>GetAllAvailableDistricts();
   

    Task<Response<District>> AddDistrict(District district);

    
}