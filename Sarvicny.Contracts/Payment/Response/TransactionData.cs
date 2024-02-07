using Newtonsoft.Json;

namespace Sarvicny.Contracts.Payment.Response
{
    public class TransactionData
    {
        [JsonProperty("id")]
        public int TransactionId { get; set; }
        public bool pending { get; set; }
        public bool success { get; set; }
        public bool is_auth { get; set; }
        public bool is_capture { get; set; }
        public int amount_cents { get; set; }
        public bool is_voided { get; set; }
        public bool is_refund { get; set; }
        public int integration_id { get; set; }
        public OrderData? order { get; set; }
        public DateTime created_at { get; set; }
        public string currency { get; set; }
        public bool error_occured { get; set; }
        public bool has_parent_transaction { get; set; }
        public bool is_3d_secure { get; set; }
        public bool is_refunded { get; set; }
        public bool is_standalone_payment { get; set; }
        public int owner { get; set; }
        public SourceData source_data { get; set; }
        public class SourceData
        {
            public string pan { get; set; }
            public string type { get; set; }
            public string sub_type { get; set; }
        }
    }
}
