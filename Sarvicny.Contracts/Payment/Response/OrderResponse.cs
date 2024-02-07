using Newtonsoft.Json;

namespace Sarvicny.Contracts.Payment.Response
{
    public class OrderResponse
    {
        [JsonProperty("Id")]
        public string OrderId { get; set; }
    }
}
