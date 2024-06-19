using Sarvicny.Domain.Entities.Users.ServicProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarvicny.Domain.Entities.Avaliabilities;
using System.Text.Json.Serialization;

namespace Sarvicny.Domain.Entities
{
    public class ProviderAvailability
    {
        public ProviderAvailability()
        {
            Slots = new List<AvailabilityTimeSlot>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ProviderAvailabilityID { get; set; }

        public string ServiceProviderID { get; set; }

        public string DayOfWeek { get; set; }

        public List<AvailabilityTimeSlot> Slots { get; set; }

        [ForeignKey("ServiceProviderID")]
        [JsonIgnore]
     
        public Provider ServiceProvider { get; set; }

    }
}
