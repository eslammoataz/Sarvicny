using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Contracts.Dtos
{
    public class RatingDto
    {
        [Required(ErrorMessage = "Order ID is Required ")]
        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        public string? ServiceProviderId { get; set; }

        public int? customerRating { get; set; }
        public int? providerRating { get; set; }
        public String Comment { get; set;}
    }
}
