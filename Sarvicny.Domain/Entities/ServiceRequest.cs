using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class ServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ServiceRequestID { get; set; }

        public string CartID { get; set; }

        [ForeignKey("CartID")]
        public Cart Cart { get; set; }

        public ProviderService providerService { get; set; }
        public DateTime? AddedTime { get; set; }

    }
}
