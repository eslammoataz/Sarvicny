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
            Slots = new List<TimeSlot>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ProviderAvailabilityID { get; set; }

        public string ServiceProviderID { get; set; }

        // Date or week associated with the availability data
        //to be able to reserve and retrieve previous data
        public DateTime? AvailabilityDate { get; set; }

        public string DayOfWeek { get; set; }


        public List<TimeSlot> Slots { get; set; }

        [ForeignKey("ServiceProviderID")]
        [JsonIgnore]
     
        public Provider ServiceProvider { get; set; }

    }
}
