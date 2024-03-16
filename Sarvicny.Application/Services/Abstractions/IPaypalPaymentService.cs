using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Abstractions
{
    public interface IPaypalPaymentService
    {
        public Task<string> GetAuthToken();
        public Task<object> CreateOrder(Order order);
        public Task<object> CaptureOrder(string orderId);



    }
}
