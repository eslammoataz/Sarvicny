using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class RequestedService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RequestedServiceId { get; set; }

        public string ServiceId { get; set; }

        public Service Service { get; set; }

        public string? OrderId { get; set; }

        public string? CartId { get; set; }


        
    }
}
