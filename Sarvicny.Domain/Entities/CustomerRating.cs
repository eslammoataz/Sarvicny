using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class CustomerRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RatingId { get; set; }

        [ForeignKey("ServiceRequestID")]
        public ServiceRequest serviceRequest { get; set; }

        public string ServiceRequestID { get; set; }
        public string customerID { get; set; }
        public string OrderID { get; set; }

        public int Rating {  get; set; }

        public string Comment { get; set; }
       
       
    }
}
