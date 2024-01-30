using Sarvicny.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class Cart
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CartID { get; set; }


        public string CustomerID { get; set; }

        public List<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        //public DateTime? AddedTime { get; set; }

        public DateTime? LastChangeTime { get; set; }


        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

    }
}
