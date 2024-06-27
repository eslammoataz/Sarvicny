using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface ICustomerService
    {
        public Task<Response<object>> addToCart(RequestServiceDto requestServiceDto, string customerId);

        public Task<Response<object>> RemoveFromCart(string customerId, string requestId);

        public Task<Response<object>> GetCustomerCart(string customerId);

        public Task<Response<object>> OrderCart(string customerId, PaymentMethod paymentMethod);

        public Task<Response<object>> PayOrder(string orderId, PaymentMethod PayemntMethod);

        public Task<Response<object>> ShowCustomerProfile(string customerId);
        public Task<Response<object>> UpdateCustomerProfile(UpdateCustomerDto updateCustomerDto, string customerId);
        public Task<Response<object>> ViewLogRequest(string customerId); // kol al orders ali tlbha ya3ni

        public Task<Response<object>> MarkOrderComplete(string orderId);

        public Task<Response<object>> AddProviderToFav(string providerId, string customerId);

        public Task<Response<List<object>>> getCustomerFavourites(string customerId);

        public Task<Response<object>> RemoveFavProvider(string customerId, string providerId);

        Task<Response<object>> Refund(string orderId);
    }
}
