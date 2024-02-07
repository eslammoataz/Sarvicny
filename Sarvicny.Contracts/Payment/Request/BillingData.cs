namespace Sarvicny.Contracts.Payment.Request
{
    public class BillingData
    {
        public BillingData()
        {
            first_name = "NA";
            last_name = "NA";
            email = "NA";
            phone_number = "NA";
            apartment = "NA";
            floor = "NA";
            street = "NA";
            building = "NA";
            city = "NA";
            state = "NA";
            country = "NA";
            postal_code = "NA";
        }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string apartment { get; set; }
        public string floor { get; set; }
        public string street { get; set; }
        public string building { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
    }
}
