namespace Sarvicny.Contracts.Payment.Response
{
    public class OrderData
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool DeliveryNeeded { get; set; }
        public Merchant Merchant { get; set; }
        public Collector Collector { get; set; }
        public int AmountCents { get; set; }
        public ShippingData ShippingData { get; set; }
        public ShippingDetails ShippingDetails { get; set; }
        public string Currency { get; set; }
        public bool IsPaymentLocked { get; set; }
        public bool IsReturn { get; set; }
        public bool IsCancel { get; set; }
        public bool IsReturned { get; set; }
        public bool IsCanceled { get; set; }
        public object MerchantOrderId { get; set; }
        public object WalletNotification { get; set; }
        public int PaidAmountCents { get; set; }
        public bool NotifyUserWithEmail { get; set; }
        public List<object> Items { get; set; }
        public string OrderUrl { get; set; }
        public int CommissionFees { get; set; }
        public int DeliveryFeesCents { get; set; }
        public int DeliveryVatCents { get; set; }
        public string PaymentMethod { get; set; }
        public object MerchantStaffTag { get; set; }
        public string ApiSource { get; set; }
        public object PickupData { get; set; }
        public List<object> DeliveryStatus { get; set; }
    }
}
