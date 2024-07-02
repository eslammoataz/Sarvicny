using Sarvicny.Domain.Entities;
using Sarvicny.Domain.Entities.Avaliabilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Contracts.Dtos
{
    
    public class MatchingProviderDto
    {
        [Required(ErrorMessage = "Services IDs is Required ")]
        public List<string> services { get; set; }

        public string? startTime { get; set; }

        public string? dayOfWeek { get; set; }

  
        public string? districtId { get; set; }

        [Required(ErrorMessage = "Customer ID is Required ")]
        public string customerId { get; set; }
          
    }
}
