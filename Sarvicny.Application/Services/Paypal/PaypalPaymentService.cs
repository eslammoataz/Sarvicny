using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Paypal
{
    public class PaypalPaymentService : IPaypalPaymentService
    {
        private readonly ILogger<PaypalPaymentService> _logger;
        private readonly IConfiguration _config;

        public PaypalPaymentService(ILogger<PaypalPaymentService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<string> GetAuthToken()
        {
            var clientId = _config["PayPal:ClientId"];
            var clientSecret = _config["PayPal:ClientSecret"];
            var tokenUrl = "https://api-m.sandbox.paypal.com/v1/oauth2/token";

            var restClient = new RestClient(tokenUrl);
            var restRequest = new RestRequest("", Method.Post);


            restRequest.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret)));


            restRequest.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);

            var response = await restClient.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
            {
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return jsonResponse.access_token;
            }

            return "Failed to get token";

        }

        public async Task<object> CreateOrder(Order order)
        {
            try
            {
                string token = await GetAuthToken();

                var createOrderUrl = "https://api-m.sandbox.paypal.com/v2/checkout/orders";
                var restClient = new RestClient(createOrderUrl);
                var restRequest = new RestRequest("", Method.Post);


                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("Authorization", "Bearer " + token);
                restRequest.AddHeader("PayPal-Request-Id", Guid.NewGuid().ToString());


                var requestBody = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                    new
                    {
                        items = new[]
                        {
                            new
                            {
                                name = "T-Shirt",
                                description = "Green XL",
                                quantity = "1",
                                unit_amount = new
                                {
                                    currency_code = "USD",
                                    value = "1900.00"
                                }
                            }
                        },
                    amount = new
                    {
                        currency_code = "USD",
                        value = "1900.00",
                        breakdown = new
                        {
                            item_total = new
                            {
                                currency_code = "USD",
                                value = "1900.00"
                            }
                        }
                    }
                }
            },
                    application_context = new
                    {
                        return_url = "https://f0b2-156-193-249-165.ngrok-free.app/api/Payment/TransactionResponseCallback",
                        cancel_url = "https://example.com/cancel"
                    }
                };


                restRequest.AddJsonBody(requestBody);

                var response = await restClient.ExecuteAsync(restRequest);

                if (response.IsSuccessful)
                {
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);

                    string approveUrl = null;

                    foreach (var link in jsonResponse.links)
                    {
                        if (link["rel"].ToString() == "approve")
                        {
                            approveUrl = link["href"].ToString();
                            break;
                        }
                    }

                    var result = new
                    {
                        orderId = jsonResponse.id.ToString(),
                        approveUrl,
                    };

                    _logger.LogInformation($"{jsonResponse.links}");

                    return result;
                }
                else
                {
                    _logger.LogError($"Error capturing order: {response.StatusCode} - " +
                  $"{response.StatusDescription}, Content: {response.Content}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception creating order: " + ex.Message);
                return null;
            }
        }

        public async Task<object> CaptureOrder(string orderId)
        {
            try
            {

                string token = await GetAuthToken();


                var captureOrderUrl = $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}/capture";


                var restClient = new RestClient(captureOrderUrl);
                var restRequest = new RestRequest("", Method.Post);


                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("Authorization", "Bearer " + token);
                restRequest.AddHeader("PayPal-Request-Id", Guid.NewGuid().ToString());


                var response = await restClient.ExecuteAsync(restRequest);


                if (response.IsSuccessful)
                {
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);

                    var status = jsonResponse.status.ToString();
                    return new
                    {
                        orderId,
                        status
                    };
                }
                else
                {
                    _logger.LogError($"Error capturing order: {response.StatusCode} - " +
                        $"{response.StatusDescription}, Content: {response.Content}");

                    object errorResponseObject = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    return errorResponseObject;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("Exception capturing order: " + ex.Message);
                return null;
            }
        }




    }
}
