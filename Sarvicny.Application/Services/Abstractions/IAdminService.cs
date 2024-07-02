using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Users.ServicProviders;


namespace Sarvicny.Application.Services.Abstractions;

public interface IAdminService
{
    Task<Response<ICollection<object>>> GetAllCustomers();
    Task<Response<ICollection<object>>> GetAllServiceProviders();
    Task<Response<ICollection<object>>> GetAllServices();
    Task<Response<object>> ApproveProviderRegisteration(string workerId);
    Task<Response<object>> RejectProviderRegisteration(string workerId);

    Task<Response<object>> ApproveServiceForProvider(string provider,string providerServiceID);
    Task<Response<object>> RejectServiceForProvider(string provider,string providerServiceID);

    Task<Response<List<object>>> GetProvidersRegistrationRequests();
    Task<Response<List<object>>> GetProvidersAddtionalServiceRequests();
    Task<Response<List<object>>> getAllOrders();
    Task<Response<List<object>>> getAllPendingOrPaidOrders();
    Task<Response<List<object>>> getAllCanceledByProviderOrders();
    Task<Response<List<object>>> RemoveAllPaymentExpiredOrders();

    Task<Response<object>> MarkOrderComplete(string orderId);

    Task<Response<bool>> BlockServiceProvider(string workerId);
    Task<Response<bool>> UnBlockServiceProvider(string workerId);
    Task<Response<List<object>>> GetAllAvailableDistricts();
    Task<Response<District>> AddDistrict(District district);
    Task<Response<object>> ReAssignOrder(string OrderId);
    Task<Response<object>> MarkFraud(string OrderId);
}