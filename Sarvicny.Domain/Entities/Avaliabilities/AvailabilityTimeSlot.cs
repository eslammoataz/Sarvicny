using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities.Avaliabilities
{
    public class AvailabilityTimeSlot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TimeSlotID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool isActive { get; set; }


        // Foreign key to relate TimeSlot to ProviderAvailability
        public string ProviderAvailabilityID { get; set; }

        [ForeignKey("ProviderAvailabilityID")]
        public ProviderAvailability ProviderAvailability { get; set; }
    }
}
