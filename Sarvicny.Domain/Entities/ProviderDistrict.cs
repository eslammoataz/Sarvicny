using Sarvicny.Domain.Entities.Users.ServicProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class ProviderDistrict
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ProviderDistrictID { get; set; }
        public string ProviderID { get; set; }
        public string DistrictID { get; set; }

        public bool enable {  get; set; }

        [ForeignKey("ProviderID")]
        public Provider Provider { get; set; }

        [ForeignKey("DistrictID")]
        public District District { get; set; }
    }
}
