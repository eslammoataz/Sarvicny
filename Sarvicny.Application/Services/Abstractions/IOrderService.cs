using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IOrderService
    {
        Task<Response<object>> ShowAllOrderDetailsForProvider(string orderId);
        Task<Response<object>> ShowAllOrderDetailsForAdmin(string orderId);
        
        Task<Response<object>> ShowAllOrderDetailsForCustomer(string orderId);
        Task<Response<object>> ShowOrderStatus(string orderId);
        Task<Response<object>> AddCustomerRating(RatingDto ratingDto, string orderID);
        Task<Response<object>> AddProviderRating(RatingDto ratingDto, string orderID);

        Task<Response<object>> GetProviderRatingForOrder(string orderID);
        Task<Response<object>> GetCustomerRatingForOrder(string orderID);

    }
}
