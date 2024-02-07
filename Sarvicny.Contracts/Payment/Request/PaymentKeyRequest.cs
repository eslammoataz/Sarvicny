namespace Sarvicny.Contracts.Payment.Request
{
    public class PaymentKeyRequest
    {
        public string auth_token { get; set; }
        public string amount_cents { get; set; }
        public int expiration { get; set; }
        public string order_id { get; set; }
        public BillingData billing_data { get; set; }
        public string currency { get; set; }
        public int integration_id { get; set; }
        public bool lock_order_when_paid { get; set; }
    }
}
