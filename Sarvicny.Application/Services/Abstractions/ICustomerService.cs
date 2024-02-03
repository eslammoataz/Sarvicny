using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface ICustomerService
    {
        public Task<Response<object>> RequestService(RequestServiceDto requestServiceDto, string customerId);

        public Task<Response<object>> CancelRequestService(string customerId, string requestId);

        public Task<Response<object>> GetCustomerCart(string customerId);

        public Task<Response<object>> OrderCart(string customerId);

        public Task<Response<object>> ShowCustomerProfile(string customerId);
        public Task<Response<object>> ViewLogRequest(string customerId); // kol al orders ali tlbha ya3ni



    }
}
