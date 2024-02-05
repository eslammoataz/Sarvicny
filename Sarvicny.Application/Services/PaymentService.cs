using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Sarvicny.Application.Services.Abstractions;

namespace Sarvicny.Application.Services;

public class PaymentService: IPaymentService
{
    private readonly IConfiguration _config;

    public PaymentService(IConfiguration config)
    {
        _config = config;
    }
    public async Task<string> GetAuthToken()
    {
        var tokenUrl = "https://accept.paymob.com/api/auth/tokens";
        var restClient = new RestClient(tokenUrl);
        var restRequest = new RestRequest("",Method.Post);
        
        restRequest.AddHeader("Content-Type","application/json");
       
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

    public async Task<object> OrderRegistration()
    {
        var tokenUrl = "https://accept.paymob.com/api/ecommerce/orders";
        var restClient = new RestClient(tokenUrl);
        var restRequest = new RestRequest("",Method.Post);
        
        
        restRequest.AddHeader("Content-Type","application/json");

        string authToken = await GetAuthToken();
        var orderRequest = new OrderRequest
        {
            AuthToken = authToken,
            DeliveryNeeded = true,
            AmountCents = 100,
            Items = new List<object>()
        };
        
        // Serialize the OrderRequest to JSON
        string jsonRequest = JsonConvert.SerializeObject(orderRequest);
        
        restRequest.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);
        
        var response = await restClient.ExecuteAsync(restRequest);
        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<OrderResponse>(response.Content);

            return jsonResponse;
        }

        return "Failed";

    }


    public class ApiResponse
    {
        public string Token { get; set; }
    }
    
    public class OrderRequest
    {
        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }

        [JsonProperty("delivery_needed")]
        public bool DeliveryNeeded { get; set; }

        [JsonProperty("amount_cents")]
        public int AmountCents { get; set; }
        
        [JsonProperty("items")]
        public List<object> Items { get; set; }

    }

    public class OrderResponse
    {
        [JsonProperty("Id")]
        public string OrderId { get; set; }
    }

}