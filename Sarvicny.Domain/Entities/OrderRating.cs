using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Domain.Entities
{
    public class OrderRating

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RatingId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? customer { get; set; }

        [ForeignKey("ProviderId")]
        public Provider? provider { get; set; }

        [ForeignKey("OrderId")]
        public Order order { get; set; }

        public string OrderId { get; set; }
        public string? CustomerId { get; set; }
        public string? ProviderId { get; set; }
        public int? CustomerRating { get; set; }
        public int? ServiceProviderRating { get; set; }
        public string Comment { get; set; }
    }
}
