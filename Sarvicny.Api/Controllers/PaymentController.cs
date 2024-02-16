using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Sarvicny.Application.Services.Abstractions;

namespace Sarvicny.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    //[HttpPost]
    //[Route("Pay")]
    //public async Task<IActionResult> Pay()
    //{
    //    try
    //    {
    //        var payment = await _paymentService.Pay();
    //        return Ok(payment);
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError(e.Message);
    //        return StatusCode(500, "An error occurred while processing payment");
    //    }
    //}


    [HttpPost]
    [Route("TransactionProcessedCallback")]
    public async Task<IActionResult> TransactionProcessedCallback(dynamic payload)
    {
        try
        {

            Dictionary<string, string> queryParams = new Dictionary<string, string>(Request.Query.Count);

            foreach (var query in Request.Query)
            {
                queryParams.Add(query.Key, query.Value);
            }
            // Log the query parameters
            foreach (var param in queryParams)
            {
                Console.WriteLine($"Inside Transaction 'Processed' Callback Query parameter '{param.Key}': {param.Value}");
            }

            var response = await _paymentService.TransactionProcessedCallback(payload, queryParams["hmac"]);


            return Ok(response);
        }
        catch (Exception e)
        {

            Console.WriteLine(e.Message);
            return StatusCode(500, "An error occurred while processing payment");
        }
    }


    [HttpGet]
    [Route("TransactionResponseCallback")]
    public async Task<IActionResult> TransactionResponseCallback()
    {
        try
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>(Request.Query.Count);
            foreach (var query in Request.Query)
            {
                queryParams.Add(query.Key, query.Value);
            }


            // Create a new dictionary to store specific values
            var extractedValues = new Dictionary<string, string>
        {
            { "amount_cents", queryParams["amount_cents"] },
            { "created_at", queryParams["created_at"] },
            { "currency", queryParams["currency"] },
            { "error_occured", queryParams["error_occured"] },
            { "has_parent_transaction", queryParams["has_parent_transaction"] },
            { "id", queryParams["id"] },
            { "integration_id", queryParams["integration_id"] },
            { "is_3d_secure", queryParams["is_3d_secure"] },
            { "is_auth", queryParams["is_auth"] },
            { "is_capture", queryParams["is_capture"] },
            { "is_refunded", queryParams["is_refunded"] },
            { "is_standalone_payment", queryParams["is_standalone_payment"] },
            { "is_voided", queryParams["is_voided"] },
            { "order.id", queryParams["order"] },
            { "owner", queryParams["owner"] },
            { "pending", queryParams["pending"] },
            { "source_data.pan", queryParams["source_data.pan"] },
            { "source_data.sub_type", queryParams["source_data.sub_type"] },
            { "source_data.type", queryParams["source_data.type"] },
            { "success", queryParams["success"] }
        };


            foreach (var param in queryParams)
            {
                Console.WriteLine($"Inside Transaction 'Response' Callback Query parameter '{param.Key}': {param.Value}");
            }

            var response = await _paymentService.TransactionResponseCallback(extractedValues, queryParams["hmac"]);

            return Ok(response);
        }
        catch (Exception e)
        {
            // Log the error
            Console.WriteLine(e.Message);
            return StatusCode(500, "An error occurred while processing payment");
        }
    }


    [HttpGet]
    [Route("TestHMAC")]
    public IActionResult Test()
    {
        try
        {
            string jsonData = @"
{
  ""obj"": {
    ""id"": 2556706,
    ""pending"": false,
    ""amount_cents"": 100,
    ""success"": true,
    ""is_auth"": false,
    ""is_capture"": false,
    ""is_standalone_payment"": true,
    ""is_voided"": false,
    ""is_refunded"": false,
    ""is_3d_secure"": true,
    ""integration_id"": 6741,
    ""profile_id"": 4214,
    ""has_parent_transaction"": false,
    ""order"": {
      ""id"": 4778239,
      ""created_at"": ""2020-03-25T18:36:05.494685"",
      ""delivery_needed"": true,
      ""merchant"": {
        ""id"": 4214,
        ""created_at"": ""2019-09-22T18:32:56.764441"",
        ""phones"": [
          ""01032347111""
        ],
        ""company_emails"": [
          ""fnjum@temp-link.net""
        ],
        ""company_name"": ""Accept Payments"",
        ""state"": """",
        ""country"": ""EGY"",
        ""city"": """",
        ""postal_code"": """",
        ""street"": """"
      },
      ""collector"": {
        ""id"": 115,
        ""created_at"": ""2019-06-29T00:48:26.910433"",
        ""phones"": [],
        ""company_emails"": [],
        ""company_name"": ""logix - test"",
        ""state"": ""Heliopolis"",
        ""country"": ""egypt"",
        ""city"": ""cairo"",
        ""postal_code"": ""123456"",
        ""street"": ""Marghany""
      },
      ""amount_cents"": 2000,
      ""shipping_data"": {
        ""id"": 2558893,
        ""first_name"": ""abdulrahman"",
        ""last_name"": ""Khalifa"",
        ""street"": ""Wadi el Nile"",
        ""building"": ""5"",
        ""floor"": ""11"",
        ""apartment"": ""1565162"",
        ""city"": ""Cairo"",
        ""state"": ""Cairo"",
        ""country"": ""EG"",
        ""email"": ""abdulrahman@weaccept.co"",
        ""phone_number"": ""01011994353"",
        ""postal_code"": """",
        ""extra_description"": "" "",
        ""shipping_method"": ""UNK"",
        ""order_id"": 4778239
      },
      ""shipping_details"": {
        ""id"": 1401,
        ""cash_on_delivery_amount"": 0,
        ""cash_on_delivery_type"": ""Cash"",
        ""latitude"": null,
        ""longitude"": null,
        ""is_same_day"": 0,
        ""number_of_packages"": 1,
        ""weight"": 1,
        ""weight_unit"": ""Kilogram"",
        ""length"": 1,
        ""width"": 1,
        ""height"": 1,
        ""delivery_type"": ""PUD"",
        ""return_type"": null,
        ""order_id"": 4778239,
        ""notes"": ""im so tired""
      },
      ""currency"": ""EGP"",
      ""is_payment_locked"": false,
      ""is_return"": false,
      ""is_cancel"": false,
      ""is_returned"": false,
      ""is_canceled"": false,
      ""merchant_order_id"": null,
      ""wallet_notification"": null,
      ""paid_amount_cents"": 100,
      ""notify_user_with_email"": false,
      ""items"": [],
      ""order_url"": ""https://accept.paymobsolutions.com/i/nYWD"",
      ""commission_fees"": 0,
      ""delivery_fees_cents"": 0,
      ""delivery_vat_cents"": 0,
      ""payment_method"": ""tbc"",
      ""api_source"": ""OTHER"",
      ""pickup_data"": null,
      ""delivery_status"": []
    },
    ""created_at"": ""2020-03-25T18:39:44.719228"",
    ""transaction_processed_callback_responses"": [],
    ""currency"": ""EGP"",
    ""source_data"": {
      ""pan"": ""2346"",
      ""type"": ""card"",
      ""sub_type"": ""MasterCard""
    },
    ""api_source"": ""IFRAME"",
    ""terminal_id"": null,
    ""is_void"": false,
    ""is_refund"": false,
    ""data"": {
      ""acq_response_code"": ""00"",
      ""avs_acq_response_code"": ""Unsupported"",
      ""klass"": ""VPCPayment"",
      ""receipt_no"": ""008603626261"",
      ""order_info"": ""claudette09@exa.com"",
      ""message"": ""Approved"",
      ""gateway_integration_pk"": 6741,
      ""batch_no"": ""20200325"",
      ""card_num"": null,
      ""secure_hash"": ""832F4673452F9538CCD57D6B07B74183A0EEB1BEF7CA58704E31B244E8366549"",
      ""avs_result_code"": ""Unsupported"",
      ""card_type"": ""MC"",
      ""merchant"": ""TEST999999EGP"",
      ""created_at"": ""2020-03-25T16:40:37.127504"",
      ""merchant_txn_ref"": ""6741_572e773a5a0f55ff8de91876075d023e"",
      ""authorize_id"": ""626261"",
      ""currency"": ""EGP"",
      ""amount"": ""100"",
      ""transaction_no"": ""2090026774"",
      ""txn_response_code"": ""0"",
      ""command"": ""pay""
    },
    ""is_hidden"": false,
    ""payment_key_claims"": {
      ""lock_order_when_paid"": true,
      ""integration_id"": 6741,
      ""billing_data"": {
        ""email"": ""claudette09@exa.com"",
        ""building"": ""8028"",
        ""apartment"": ""803"",
        ""street"": ""Ethan Land"",
        ""country"": ""CR"",
        ""state"": ""Utah"",
        ""last_name"": ""Nicolas"",
        ""first_name"": ""Clifford"",
        ""postal_code"": ""01898"",
        ""extra_description"": ""NA"",
        ""phone_number"": ""+86(8)9135210487"",
        ""floor"": ""42"",
        ""city"": ""Jaskolskiburgh""
      },
      ""order_id"": 4778239,
      ""user_id"": 4705,
      ""pmk_ip"": ""197.57.37.135"",
      ""exp"": 1585157836,
      ""currency"": ""EGP"",
      ""amount_cents"": 100
    },
    ""error_occured"": false,
    ""is_live"": false,
    ""other_endpoint_reference"": null,
    ""refunded_amount_cents"": 0,
    ""source_id"": -1,
    ""is_captured"": false,
    ""captured_amount"": 0,
    ""merchant_staff_tag"": null,
    ""owner"": 4705,
    ""parent_transaction"": null
  },
  ""type"": ""TRANSACTION""
}";

            // Assuming jsonData is your JObject variable
            JObject jsonObject = JObject.Parse(jsonData);

            // Convert the JObject to a string
            string jsonString = jsonObject.ToString();

            var data = _paymentService.ExtractHmacData(jsonString);

            var hashed = _paymentService.VerifyHmac(data, "a5b4dd6bbc46e65b16301af62bb135fa83fa211f704f3f76cc4658b47f64a105061a8926530c250cb5a1bbed7f865156b359222c39aaf3bf4c6dcf6d0e8ee8f1");

            return Ok(data);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500, "An error occurred while processing payment");
        }
    }

}