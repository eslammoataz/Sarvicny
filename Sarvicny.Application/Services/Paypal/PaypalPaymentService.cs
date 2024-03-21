using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Contracts;
using Sarvicny.Domain.Entities;

namespace Sarvicny.Application.Services.Paypal
{
    public class PaypalPaymentService : IPaypalPaymentService
    {
        private readonly ILogger<PaypalPaymentService> _logger;
        private readonly IConfiguration _config;
        private readonly IHandlePayment _handlePayment;

        public PaypalPaymentService(ILogger<PaypalPaymentService> logger, IConfiguration config, IHandlePayment handlePayment)
        {
            _logger = logger;
            _config = config;
            _handlePayment = handlePayment;
        }

        public async Task<string> GetAuthToken()
        {
            var clientId = _config["PayPal:ClientId"];
            var clientSecret = _config["PayPal:ClientSecret"];
            var tokenUrl = "https://api-m.sandbox.paypal.com/v1/oauth2/token";

            var restClient = new RestClient(tokenUrl);
            var restRequest = new RestRequest("", Method.Post);


            restRequest.AddHeader("Authorization",
                "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret)));


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
                        return_url =
                            "https://f0b2-156-193-249-165.ngrok-free.app/api/Payment/TransactionResponseCallback",
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

        public async Task<Response<object>> Pay(Order order)
        {
            var accessToken = await GetAuthToken();

            string paypalUrl = _config["PayPal:PaypalApiUrl"];

            var client = new RestClient(paypalUrl);


            var request = new RestRequest("/v1/payments/payment", Method.Post);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("Content-Type", "application/json");

            //var invoiceNumber = getInvoiceNumber(accessToken);
            var invoiceNumber = Guid.NewGuid();
            var createPaymentJson = new
            {
                intent = "sale",
                payer = new
                {
                    payment_method = "paypal"
                },
                redirect_urls = new
                {
                    return_url = _config["PayPal:ReturnUrl"],
                    cancel_url = _config["PayPal:CancelUrl"]
                },
                transactions = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency = "USD",
                            total = order.TotalPrice?.ToString("0.00")
                        },
                        description = "this is the payment transaction description",
                        invoice_number = invoiceNumber,
                        custom = order.OrderID,
                    }
    }
            };

            request.AddJsonBody(createPaymentJson);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var createPaymentResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);

                return new Response<object>()
                {
                    isError = false,
                    Payload = GetApprovalLink(createPaymentResponse.links)
                };
            }

            _logger.LogError($"Error creating payment: {response.StatusCode} - " +
                             $"{response.StatusDescription}, Content: {response.Content}");

            return new Response<object>
            {
                isError = true,
                Errors = new List<string>()
                    {
                        "Error creating payment"
                    },
                Message = "Error creating payment"
            };

        }

        public async Task<Response<object>> ExecutePayment(string paymentId, string payerId, string token)
        {
            var accessToken = await GetAuthToken();

            string paypalUrl = _config["PayPal:PaypalApiUrl"];

            var client = new RestClient(paypalUrl);

            var request = new RestRequest($"/v1/payments/payment/{paymentId}/execute", Method.Post);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("Content-Type", "application/json");

            var executePaymentJson = new
            {
                payer_id = payerId
            };

            request.AddJsonBody(executePaymentJson);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var executePaymentResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);

                // Extract invoice number
                string invoiceNumber = executePaymentResponse?.transactions?[0]?.invoice_number?.Value.ToString();

                // Extract custom attribute
                string orderId = executePaymentResponse?.transactions?[0]?.custom?.Value.ToString();


                var handlePaymentResponse = await _handlePayment.validateOrder(orderId, true, invoiceNumber, PaymentMethod.Paypal);

                return handlePaymentResponse;
            }

            _logger.LogError($"Error executing payment: {response.StatusCode} - " +
                             $"{response.StatusDescription}, Content: {response.Content}");
            return new Response<object>
            {
                isError = true,
                Errors = new List<string>()
                {
                    "Error executing payment"
                },
                Message = "Error executing payment"
            };
        }

        public async Task<object> GetPayment(string paymentId)
        {
            var accessToken = await GetAuthToken();

            string paypalUrl = _config["PayPal:ApiUrl"];

            var client = new RestClient(paypalUrl);

            var request = new RestRequest($"/v1/payments/payment/{paymentId}", Method.Get);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var paymentDetails = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return paymentDetails;
            }

            _logger.LogError($"Error getting payment details: {response.StatusCode} - " +
                             $"{response.StatusDescription}, Content: {response.Content}");
            return null;
        }


        private string GetApprovalLink(dynamic links)
        {
            foreach (var link in links)
            {
                if (link.rel == "approval_url")
                {
                    return link.href;
                }
            }
            throw new Exception("Approval link not found in the PayPal API response.");
        }

        private async Task<string> getInvoiceNumber(string authToken)
        {
            string paypalUrl = _config["PayPal:PaypalApiUrl"];
            var client = new RestClient(paypalUrl);

            var request = new RestRequest("/v2/invoicing/generate-next-invoice-number", Method.Get);
            request.AddHeader("Authorization", $"Bearer {authToken}");
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var invoiceNumber = response.Content;
                return invoiceNumber;
            }
            else
            {
                throw new Exception("Error generating invoice number from PayPal API: " + response.ErrorMessage);
            }
        }
    }
}
