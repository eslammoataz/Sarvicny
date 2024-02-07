using Newtonsoft.Json;
using Sarvicny.Contracts.Payment.Response;

namespace Sarvicny.Contracts.Payment
{

    public class TransactionCallBackBody
    {
        [JsonProperty("obj")]
        public TransactionData Obj { get; set; }

        public string Type { get; set; }
    }

    public class Merchant
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Phones { get; set; }

        [JsonProperty("company_emails")]
        public List<string> CompanyEmails { get; set; }

        [JsonProperty("company_name")]
        public string CompanyName { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
        public string Street { get; set; }
    }

    public class Collector
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Phones { get; set; }

        [JsonProperty("company_emails")]
        public List<string> CompanyEmails { get; set; }

        [JsonProperty("company_name")]
        public string CompanyName { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
    }

    public class ShippingData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string ExtraDescription { get; set; }
        public string ShippingMethod { get; set; }
        public int OrderId { get; set; }
    }

}
