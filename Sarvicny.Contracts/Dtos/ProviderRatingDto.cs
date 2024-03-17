using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Contracts.Dtos
{
    public class ProviderRatingDto
    {
        public string serviceRequestID { get; set; }

        public int ServiceProviderRating { get; set; }
        public String Comment { get; set; }
    }
}
