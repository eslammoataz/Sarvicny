using Sarvicny.Contracts;
using Sarvicny.Contracts.Dtos;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IOrderService
    {
        Task<Response<object>> ShowAllOrderDetailsForProvider(string orderId);
        Task<Response<object>> ShowAllOrderDetailsForAdmin(string orderId);

        Task<object> ShowAllOrderDetailsForCustomer(string orderId);

        Task<Response<object>> Refund(string orderId);

        Task<Response<object>> ShowOrderStatus(string orderId);
        Task<Response<object>> AddCustomerRating(RatingDto ratingDto, string orderID);
        Task<Response<object>> AddProviderRating(RatingDto ratingDto, string orderID);

        Task<Response<object>> GetProviderRatingForOrder(string orderID);
        Task<Response<object>> GetCustomerRatingForOrder(string orderID);
        public Task<Response<List<object>>> GetAllMatchedProviderSortedbyFav(MatchingProviderDto matchingProviderDto);
        public Task<Response<List<object>>> SuggestNewProvidersIfNoMatchesFoundLevel1(MatchingProviderDto matchingProviderDto);
        public Task<Response<List<object>>> SuggestNewProvidersIfNoMatchesFoundLevel2(MatchingProviderDto matchingProviderDto);
    }
}
