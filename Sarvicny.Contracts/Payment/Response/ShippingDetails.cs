namespace Sarvicny.Contracts.Payment.Response
{
    public class ShippingDetails
    {
        public int Id { get; set; }
        public int CashOnDeliveryAmount { get; set; }
        public string CashOnDeliveryType { get; set; }
        public object Latitude { get; set; }
        public object Longitude { get; set; }
        public int IsSameDay { get; set; }
        public int NumberOfPackages { get; set; }
        public int Weight { get; set; }
        public string WeightUnit { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string DeliveryType { get; set; }
        public object ReturnType { get; set; }
        public int OrderId { get; set; }
        public string Notes { get; set; }
    }
}
