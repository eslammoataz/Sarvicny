using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IOrderService
    {
        Task<Response<object>> ShowOrderDetailsForProvider(string orderId);
        Task<Response<object>> ShowAllOrderDetails(string orderId);
        Task<Response<object>> ShowAllOrderDetailsForCustomer(string orderId);
        Task<Response<object>> ShowOrderStatus(string orderId);

        Task<Response<object>> AddRatingCustomer(OrderRating rating);
        Task<Response<object>> AddRatingServiceProvider(OrderRating rating);
    }
}
