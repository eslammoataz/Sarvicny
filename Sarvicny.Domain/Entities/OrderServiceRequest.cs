using Sarvicny.Domain.Entities.Avaliabilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class OrderServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderServiceRequestID { get; set; }

        public string? OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("ProviderServiceID")]
        public string ProviderServiceID { get; set; }
        public ProviderService providerService { get; set; }

        public string RequestedSlotID { get; set; } = default!;

        [ForeignKey("RequestedSlotID")]
        public RequestedSlot RequestedSlot { get; set; }

        public string ProviderDistrictID { get; set; } = default!;

        [ForeignKey("ProviderDistrictID")]
        public ProviderDistrict providerDistrict { get; set; }

        public string Address { get; set; }


        public decimal Price { get; set; }

        [MaxLength(255)]
        public string? ProblemDescription { get; set; }

        public string? customerRatingId { get; set; }

        [ForeignKey("customerRatingId")]
        public CustomerRating? CRate { get; set; }

        public string? providerRatingId { get; set; }

        [ForeignKey("providerRatingId")]
        public ProviderRating? PRate { get; set; }


    }
}
