using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Domain.Entities
{
    public class District
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DistrictID { get; set; }

        public string DistrictName { get; set; }

        public List<ProviderDistrict>? ProviderDistricts { get; set; }

        public bool Availability { get; set; }
    }
}
