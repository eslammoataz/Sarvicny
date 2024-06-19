using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface ICustomerService
    {
        public Task<Response<object>> RequestService(RequestServiceDto requestServiceDto, string customerId);

        public Task<Response<object>> CancelRequestService(string customerId, string requestId);

        public Task<Response<object>> GetCustomerCart(string customerId);

        public Task<Response<object>> OrderCart(string customerId, PaymentMethod PaymentMethod);
        public Task<Response<object>> ShowCustomerProfile(string customerId);
        public Task<Response<object>> UpdateCustomerProfile(UpdateCustomerDto updateCustomerDto,string customerId);
        public Task<Response<object>> ViewLogRequest(string customerId); // kol al orders ali tlbha ya3ni



    }
}
