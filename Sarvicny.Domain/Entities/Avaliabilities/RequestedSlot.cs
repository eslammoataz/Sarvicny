using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities.Avaliabilities
{
    public class RequestedSlot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SlotId { get; set; }

        public DateTime RequestedDay { get; set; }
        public string DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }
        
     


    }
}
