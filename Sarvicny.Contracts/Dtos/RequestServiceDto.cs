using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Contracts.Dtos
{
    public class RequestServiceDto
    {
        [Required(ErrorMessage = "Provider ID is Required ")]
        public string providerId { get; set; }

        [Required(ErrorMessage = "Service ID is Required ")]
        public string serviceId { get; set; }

        public string scheduledHour { get; set; }
    }
}
