using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Payment;
using Sarvicny.Contracts.Payment.Request;
using Sarvicny.Contracts.Payment.Response;
using Sarvicny.Domain.Entities;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Sarvicny.Application.Services.Paymob;

public class PaymobPaymentService : IPaymobPaymentService
{
    private readonly IConfiguration _config;
    private readonly IHandlePayment _handlePayment;

    public PaymobPaymentService(IConfiguration config, IHandlePayment handlePayment)
    {
        _config = config;
        _handlePayment = handlePayment;
    }
    public async Task<string> GetAuthToken()
    {
        var tokenUrl = "https://accept.paymob.com/api/auth/tokens";
        var restClient = new RestClient(tokenUrl);
        var restRequest = new RestRequest("", Method.Post);

        restRequest.AddHeader("Content-Type", "application/json");

        var requestBody = new
        {
            api_key = _config["PayMob:ApiKey"]
        };
        restRequest.AddJsonBody(requestBody);
        var response = await restClient.ExecuteAsync(restRequest);
        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);

            return jsonResponse.Token;
        }

        return "Failed to get token";

    }

    public async Task<OrderResponse> OrderRegistration(Order order)
    {
        var orderUrl = "https://accept.paymob.com/api/ecommerce/orders";
        var restClient = new RestClient(orderUrl);
        var restRequest = new RestRequest("", Method.Post);


        restRequest.AddHeader("Content-Type", "application/json");

        string authToken = await GetAuthToken();

        var orderPriceInCents = order.OrderDetails.Price * 100;

        var orderRequest = new OrderRequest
        {
            AuthToken = authToken,
            DeliveryNeeded = true,
            AmountCents = orderPriceInCents.ToString(),
            Items = new List<object>(),
            MerchantOrderId = order.OrderID
        };

        // Serialize the OrderRequest to JSON
        string requestBody = JsonConvert.SerializeObject(orderRequest);

        restRequest.AddJsonBody(requestBody);

        var response = await restClient.ExecuteAsync(restRequest);
        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<OrderResponse>(response.Content);

            return jsonResponse;
        }

        return new OrderResponse();

    }

    public async Task<Response<object>> Pay(Order order)
    {

        var orderUrl = "https://accept.paymob.com/api/acceptance/payment_keys";
        var restClient = new RestClient(orderUrl);
        var restRequest = new RestRequest("", Method.Post);
        restRequest.AddHeader("Content-Type", "application/json");


        var Order = await OrderRegistration(order);

        int integrationId;
        if (!int.TryParse(_config["PayMob:IntegrationId"], out integrationId))
        {
            return new Response<object>
            {
                isError = true,
                Errors = new List<string> { "Invalid Integration ID" },
                Message = "Failed to get payment key"
            };
        }

        var customer = order.Customer;
        var orderPriceInCents = order.OrderDetails.Price * 100;

        var requestBody = new PaymentKeyRequest()
        {
            auth_token = await GetAuthToken(),
            amount_cents = orderPriceInCents.ToString(),
            expiration = 3600,
            order_id = Order.OrderId,
            billing_data = new BillingData
            {
                first_name = customer.FirstName,
                last_name = customer.LastName,
                email = customer.Email,
                phone_number = customer.PhoneNumber,
                street = customer.Address,
            },
            currency = "EGP",
            integration_id = integrationId,
            lock_order_when_paid = false
        };

        restRequest.AddJsonBody(requestBody);
        var response = await restClient.ExecuteAsync(restRequest);
        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);

            string paymentFrameUrl = _config["PayMob:Frame"];
            string paymentUrl = paymentFrameUrl.Replace("{payment_key_obtained_previously}", jsonResponse.Token);
            return new Response<object>
            {
                isError = false,
                Message = "Success",
                Payload = new
                {
                    PaymentUrl = paymentUrl
                }
            };
        }

        return new Response<object>
        {
            isError = true,
            Errors = new List<string> { "Failed to get payment key" },
            Message = "Failed to get payment key"
        };

    }


    public async Task<Response<object>> TransactionProcessedCallback(dynamic Payload, string hmac)
    {
        var payloadString = Payload.ToString();
        TransactionCallBackBody transaction = JsonConvert.DeserializeObject<TransactionCallBackBody>(payloadString);

        Console.WriteLine(JsonConvert.SerializeObject(transaction.Obj, Formatting.Indented));
        Console.WriteLine(transaction.Type);

        var extractedData = ExtractHmacData(payloadString);

        var isValid = VerifyHmac(extractedData, hmac);

        if (!isValid)
        {
            return new Response<object>
            {
                isError = false,
                Errors = new List<string>()
                {
                    "Invalid HMAC"
                },
                Message = "Failed proccess"
            };
        }

        var orderId = transaction.Obj.order.merchant_order_id;
        var emptySaleId = string.Empty; // needs handling

        await _handlePayment.validateOrder(orderId, transaction.Obj.success, transaction.Obj.TransactionId, emptySaleId, PaymentMethod.Paymob);

        return new Response<object>
        {
            Message = "Success",
            Payload = transaction
        };

    }

    public async Task<Response<object>> TransactionResponseCallback(Dictionary<string, string> Payload, string hmac)
    {
        var isValid = VerifyHmac(Payload, hmac);

        if (!isValid)
        {
            return new Response<object>
            {
                isError = false,
                Errors = new List<string>()
                {
                    "Invalid HMAC"
                },
                Message = "Failed proccess not Secure"
            };
        }

        return new Response<object>
        {
            Message = "Success , Transaction Done Successfully",
            Payload = Payload
        };
    }

    public bool VerifyHmac(Dictionary<string, string> data, string receivedHmac)
    {
        var concatenatedData = string.Join("", data.OrderBy(kv => kv.Key).Select(kv => $"{kv.Value}"));
        var secretKey = _config["PayMob:SecretKey"];

        using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
        {
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedData));
            string calculatedHmac = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            return string.Equals(receivedHmac, calculatedHmac, StringComparison.OrdinalIgnoreCase);
        }
    }

    public Dictionary<string, string> ExtractHmacData(string payloadString)
    {
        JObject payloadObject = JObject.Parse(payloadString);

        // Extract the "obj" property
        JObject obj = payloadObject["obj"].ToObject<JObject>();

        // Deserialize the JSON into TransactionData object
        TransactionData transactionData = obj.ToObject<TransactionData>();

        // Create a dictionary to store the extracted data
        var extractedData = new Dictionary<string, string>
    {
        { "amount_cents", transactionData.amount_cents.ToString() },
        { "created_at", transactionData.created_at.ToString("yyyy-MM-ddTHH:mm:ss.ffffff") },
        { "currency", transactionData.currency },
        { "error_occured", transactionData.error_occured.ToString().ToLower() },
        { "has_parent_transaction", transactionData.has_parent_transaction.ToString().ToLower() },
        { "id", transactionData.TransactionId.ToString() },
        { "integration_id", transactionData.integration_id.ToString() },
        { "is_3d_secure", transactionData.is_3d_secure.ToString().ToLower() },
        { "is_auth", transactionData.is_auth.ToString().ToLower() },
        { "is_capture", transactionData.is_capture.ToString().ToLower() },
        { "is_refunded", transactionData.is_refunded.ToString().ToLower() },
        { "is_standalone_payment", transactionData.is_standalone_payment.ToString().ToLower() },
        { "is_voided", transactionData.is_voided.ToString().ToLower() },
        { "order.id", transactionData.order.Id.ToString() },
        { "owner", transactionData.owner.ToString() },
        { "pending", transactionData.pending.ToString().ToLower() },
        { "source_data.pan", transactionData.source_data.pan },
        { "source_data.sub_type", transactionData.source_data.sub_type },
        { "source_data.type", transactionData.source_data.type },
        { "success", transactionData.success.ToString().ToLower() }
    };

        return extractedData;
    }

    public Dictionary<string, string> ExtractDataFromJson(JObject obj, List<string> keysToExtract)
    {
        var extractedData = new Dictionary<string, string>();

        foreach (var key in keysToExtract)
        {
            var property = obj.SelectToken(key.Replace(".", "['']").Replace("[", ".").Replace("]", ""));

            if (property != null)
            {
                // Handle nested properties
                if (key.StartsWith("source_data."))
                {
                    var nestedObj = property as JObject;
                    if (nestedObj != null)
                    {
                        foreach (var nestedKey in nestedObj.Properties().Select(p => $"source_data.{p.Name}"))
                        {
                            extractedData[nestedKey] = nestedObj[nestedKey.Split('.').Last()].ToString();
                        }
                    }
                }
                else
                {
                    extractedData[key] = property.ToString();
                }
            }
        }

        return extractedData;
    }

    private Dictionary<string, string> ObjectToDictionary(object obj)
    {
        var dictionary = new Dictionary<string, string>();

        foreach (PropertyInfo prop in obj.GetType().GetProperties())
        {
            // Convert property value to string and add to dictionary
            dictionary.Add(prop.Name, prop.GetValue(obj)?.ToString());
        }

        return dictionary;
    }

    public Task<Response<object>> Refund(Order order, decimal amount)
    {
        throw new NotImplementedException();
    }
}