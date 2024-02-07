using Newtonsoft.Json;

namespace Sarvicny.Contracts.Payment
{
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

}
