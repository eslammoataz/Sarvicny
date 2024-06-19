using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sarvicny.Domain.Entities.Avaliabilities;

namespace Sarvicny.Domain.Entities
{
    public class CartServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CartServiceRequestID { get; set; }

        public string? CartID { get; set; }

        [ForeignKey("CartID")]
        public Cart? Cart { get; set; }

        [ForeignKey("ProviderServiceID")]
        public string ProviderServiceID { get; set; }
        public ProviderService providerService { get; set; }

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
