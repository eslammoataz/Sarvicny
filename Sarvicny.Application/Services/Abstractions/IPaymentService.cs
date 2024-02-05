namespace Sarvicny.Application.Services.Abstractions;

public interface IPaymentService
{
    public Task<string> GetAuthToken();
    public Task<object> OrderRegistration();
}