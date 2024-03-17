using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Domain.Entities
{
    public class OrderRating

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RatingId { get; set; }

        [ForeignKey("serviceRequestID")]
        public ServiceRequest Request { get; set; }

        public string serviceRequestID { get; set; }

        public string? CustomerId { get; set; }
        public string? ProviderId { get; set; }
        public string OrderId { get; set; }
        public int? customerRating { get; set; }
        public int? ServiceProviderRating { get; set; }

        public String Comment { get; set; }
    }
}
