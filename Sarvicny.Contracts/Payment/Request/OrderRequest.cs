using Newtonsoft.Json;

namespace Sarvicny.Contracts.Payment
{
    public class OrderRequest
    {
        [JsonProperty("auth_token")]
        public required string AuthToken { get; set; }

        [JsonProperty("delivery_needed")]
        public bool DeliveryNeeded { get; set; }

        [JsonProperty("amount_cents")]
        public required string AmountCents { get; set; }

        [JsonProperty("items")]
        public List<object> Items { get; set; } = new List<object>();

        [JsonProperty("merchant_order_id")]
        public required string MerchantOrderId { get; set; }

        public string TestAddingAttribute { get; set; }

    }

}
