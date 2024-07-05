using Newtonsoft.Json.Linq;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Payment;
using Sarvicny.Contracts.Payment.Response;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Paymob;

public interface IPaymobPaymentService
{
    public Task<string> GetAuthToken();
    public Task<OrderResponse> OrderRegistration(TransactionPayment order);
    public Task<Response<object>> Pay(TransactionPayment order);
    public Task<Response<object>> TransactionProcessedCallback(dynamic Payload, string hmac);

    public Task<Response<object>> TransactionResponseCallback(Dictionary<string, string> data, string hmac);

    public Dictionary<string, string> ExtractDataFromJson(JObject obj, List<string> keysToExtract);

    public Dictionary<string, string> ExtractHmacData(string payloadString);

    public bool VerifyHmac(Dictionary<string, string> data, string receivedHmac);

    public Task<Response<object>> Refund(TransactionPayment transactionPayment, Order order, decimal amount);

    public Task<Response<object>> HandleRefund(TransactionCallBackBody transaction);
}