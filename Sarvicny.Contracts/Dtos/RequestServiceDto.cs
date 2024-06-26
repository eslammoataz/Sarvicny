﻿using System;
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
        public string ProviderId { get; set; }

        [Required(ErrorMessage = "Service ID is Required ")]
        public List<String> ServiceIDs { get; set; }
        
        [Required(ErrorMessage = "Slot ID is Required ")] 
        public string SlotID { get; set; }

        [Required(ErrorMessage = "District ID is Required ")]
        public string DistrictID { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Day of Request is Required")] 
        public DateTime RequestDay { get; set; } 
        
       
        public string? ProblemDescription { get; set; }
    }
}
