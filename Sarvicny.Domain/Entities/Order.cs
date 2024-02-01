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
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderID { get; set; }
        public string CustomerID { get; set; }
        public string OrderStatusID { get; set; }
        
        public List<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public decimal? TotalPrice { get; set; }

        //public DateTime? OrderDate { get; set; }

        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        [ForeignKey("OrderStatusID")]
        public OrderStatus OrderStatus { get; set; }

    }
}
