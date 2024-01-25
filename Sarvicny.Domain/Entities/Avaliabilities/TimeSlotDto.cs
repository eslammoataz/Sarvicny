using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities.Avaliabilities
{
    public class TimeSlotDto
    {
        [Required(ErrorMessage = "Start Time is Required ")]
        public TimeSpan StartTime { get; set; }

        [Compare(nameof(StartTime), ErrorMessage = "End time must be greater than start time.")]
        [Required(ErrorMessage = "End Time is Required ")]
        public TimeSpan EndTime { get; set; }
    }
}

