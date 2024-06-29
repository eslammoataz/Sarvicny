using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Domain.Entities
{
    public class ProviderService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ProviderServiceID { get; set; }
        public string ProviderID { get; set; }
        public string ServiceID { get; set; }

        public decimal Price { get; set; }

        public bool isVerified { get; set; }

        [ForeignKey("ProviderID")]
        public Provider Provider { get; set; }

        [ForeignKey("ServiceID")]
        public Service Service { get; set; }
    }
}
