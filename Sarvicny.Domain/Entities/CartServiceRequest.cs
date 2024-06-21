using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Domain.Entities
{
    public class CartServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CartServiceRequestID { get; set; }


        public string CartID { get; set; }

        [ForeignKey("CartID")]
        public Cart Cart { get; set; }
        
        public string ProviderID { get; set; }

        [ForeignKey("ProviderID")]
        public Provider Provider{ get; set; }

        public string RequestedServicesID { get; set; }

        [ForeignKey("RequestedServiceID")]
        public RequestedService RequestedServices { get; set; }

        public DateTime RequestedDate { get; set; }

        public string? SlotID { get; set; } = default!;

        [ForeignKey("SlotID")]
        public AvailabilityTimeSlot? Slot { get; set; }

        public string ProviderDistrictID { get; set; } = default!;

        [ForeignKey("ProviderDistrictID")]
        public ProviderDistrict providerDistrict { get; set; }

        public string Address { get; set; }


        public decimal Price { get; set; }

        [MaxLength(255)]
        public string? ProblemDescription { get; set; }



    }
}
