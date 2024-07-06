using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Sarvicny.Application.Common.Helper;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Payment;
using Sarvicny.Contracts.Payment.Request;
using Sarvicny.Contracts.Payment.Response;
using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Emails;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Sarvicny.Application.Services.Paymob;

public class PaymobPaymentService : IPaymobPaymentService
{
    private readonly IConfiguration _config;
    private readonly IHandlePayment _handlePayment;
    private readonly ITransactionPaymentRepository _transactionPaymentRepository;
    private readonly ILogger<PaymobPaymentService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    public PaymobPaymentService(IConfiguration config, IHandlePayment handlePayment, ITransactionPaymentRepository transactionPaymentRepository, ILogger<PaymobPaymentService> logger, IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _config = config;
        _handlePayment = handlePayment;
        _transactionPaymentRepository = transactionPaymentRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
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

    public async Task<OrderResponse> OrderRegistration(TransactionPayment order)
    {
        var orderUrl = "https://accept.paymob.com/api/ecommerce/orders";
        var restClient = new RestClient(orderUrl);
        var restRequest = new RestRequest("", Method.Post);


        restRequest.AddHeader("Content-Type", "application/json");

        string authToken = await GetAuthToken();

        var orderPriceInCents = order.Amount * 100;

        var orderRequest = new OrderRequest
        {
            AuthToken = authToken,
            DeliveryNeeded = true,
            AmountCents = orderPriceInCents.ToString(),
            Items = new List<object>(),
            MerchantOrderId = order.TransactionPaymentID
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

    public async Task<Response<object>> Pay(TransactionPayment order)
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


        var customer = await _transactionPaymentRepository.GetCustomerByTransactionPaymentId(order.TransactionPaymentID);

        var orderPriceInCents = order.Amount * 100;

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

        var isRefundTransaction = transaction.Obj.is_refunded;

        if (isRefundTransaction)
        {
            return await HandleRefund(transaction);
        }
        else
        {
            return await _handlePayment.validateOrder(orderId, transaction.Obj.success, transaction.Obj.TransactionId, emptySaleId, PaymentMethod.Paymob);
        }

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

    public async Task<Response<object>> Refund(TransactionPayment transactionPayment, decimal amount)
    {
        var token = _config["PayMob:Rkey"];

        if (string.IsNullOrEmpty(token))
        {
            return new Response<object>
            {
                isError = true,
                Message = "Failed to get auth token",
                Payload = null
            };
        }

        // Create the refund request
        var client = new RestClient("https://accept.paymob.com");
        var request = new RestRequest("/api/acceptance/void_refund/refund", Method.Post);
        request.AddHeader("Authorization", $"Token {token}");
        request.AddHeader("Content-Type", "application/json");

        var refundRequest = new
        {
            transaction_id = transactionPayment.TransactionID,
            amount_cents = amount * 100, // Convert to integer cents
            metadata = new
            {
                order_id = "ord_123456",
                customer_note = "Refund requested due to product issue"
            }
        };

        request.AddJsonBody(refundRequest);

        var response = await client.ExecuteAsync(request);

        // Log the request and response for debugging
        _logger.LogInformation($"Request: {JsonConvert.SerializeObject(refundRequest)}");
        _logger.LogInformation($"Response: {response.Content}");

        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content);

            if (jsonResponse != null)
            {
                return new Response<object>
                {
                    isError = false,
                    Message = "Refund processed successfully",
                    Payload = new { RefundAmount = amount }
                };
            }
            else
            {
                return new Response<object>
                {
                    isError = true,
                    Message = "Refund failed",
                    Payload = null
                };
            }
        }
        else
        {
            return new Response<object>
            {
                isError = true,
                Message = $"Refund processing error: {response.ErrorMessage} - Status: {response.StatusCode} - Content: {response.Content}",
                Payload = null
            };
        }
    }

    public async Task<Response<object>> HandleRefund(TransactionCallBackBody transaction)
    {
        var transactionId = transaction.Obj.TransactionId;
        var transactionPayment = await _transactionPaymentRepository.GetTransactionByTransactionID(transactionId);

        if (transactionPayment is null)
        {
            return new Response<object>
            {
                isError = true,
                Message = "Transaction not found",
                Payload = null
            };
        }

        var refundableOrders = transactionPayment.OrderList
                .Where(o => o.OrderStatus == OrderStatusEnum.Canceled || o.OrderStatus == OrderStatusEnum.ReAssigned || o.OrderStatus == OrderStatusEnum.RemovedWithRefund)
                .ToList();

        StringBuilder allOrdersDetails = new StringBuilder();

        foreach (var order in refundableOrders)
        {
            order.OrderStatus = OrderStatusEnum.Refunded;
            var orderDetailsForCustomer = HelperMethods.GenerateOrderDetailsMessageForCustomer(order);
            allOrdersDetails.AppendLine(orderDetailsForCustomer);
        }

        // Create the email message
        var message = new EmailDto(
            refundableOrders.First().OrderDetails.Provider.Email!,
            "Sarvicny: New Refund Alert",
            $"A refund has been sent to your PayPal account. Here are the details:\n\n{allOrdersDetails}"
        );


        _emailService.SendEmail(message);

        _unitOfWork.Commit();

        return new Response<object>
        {
            isError = false,
            Message = "Refund is done successfully money sent to customer",
            Payload = refundableOrders
        };


    }


}