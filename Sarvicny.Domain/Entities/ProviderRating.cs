using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class ProviderRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RatingId { get; set; }

        [ForeignKey("ServiceRequestID")]
        public OrderServiceRequest serviceRequest { get; set; }

        public string ServiceRequestID { get; set; }
        public string providerId { get; set; }
        public string orderId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}
