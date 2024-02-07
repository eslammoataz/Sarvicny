using Newtonsoft.Json.Linq;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Payment.Response;

namespace Sarvicny.Application.Services.Abstractions;

public interface IPaymentService
{
    public Task<string> GetAuthToken();
    public Task<OrderResponse> OrderRegistration();
    public Task<object> Pay();
    public Task<Response<object>> TransactionProcessedCallback(dynamic Payload, string hmac);

    public Task<Response<object>> TransactionResponseCallback(Dictionary<string, string> data, string hmac);

    public Dictionary<string, string> ExtractDataFromJson(JObject obj, List<string> keysToExtract);

    public Dictionary<string, string> ExtractHmacData(string payloadString);

    public bool VerifyHmac(Dictionary<string, string> data, string receivedHmac);
}