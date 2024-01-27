using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface ICustomerService
    {
        public Task<Response<string>> RequestService(RequestServiceDto requestServiceDto, string customerId);

        public Task<Response<string>> CancelRequestService(string customerId, string requestId);

        public Task<Response<List<object>>> GetCustomerCart(string customerId);

        public Task<Response<string>> OrderService(string customerId);
    }
}
