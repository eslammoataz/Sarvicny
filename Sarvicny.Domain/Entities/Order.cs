using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sarvicny.Domain.Entities.Users;

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

        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        [ForeignKey("OrderStatusID")]
        public OrderStatus OrderStatus { get; set; }

        public DateTime OrderDate { get; set; }

        public bool IsPaid { get; set; }
        
        

    }
}
