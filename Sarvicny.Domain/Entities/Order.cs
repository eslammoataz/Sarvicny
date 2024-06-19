using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Sarvicny.Domain.Entities.Users;

namespace Sarvicny.Domain.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderID { get; set; }
        public string CustomerID { get; set; }

        [NotMapped]
        public OrderStatusEnum OrderStatus { get; set; } = OrderStatusEnum.Pending;

        // Property for database persistence
        public string OrderStatusString
        {
            get => OrderStatus.ToString();
            set => OrderStatus = Enum.Parse<OrderStatusEnum>(value);
        }


        public List<OrderServiceRequest> OrderRequests { get; set; } = new List<OrderServiceRequest>();
        public decimal? TotalPrice { get; set; }

        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        //[ForeignKey("OrderStatusID")]
        //public OrderStatus OrderStatus { get; set; }

        public DateTime OrderDate { get; set; }

        public bool IsPaid { get; set; }



        public string? TransactionID { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

    }

    public enum PaymentMethod
    {
        [EnumMember(Value = "Paypal")]
        Paypal = 1,

        [EnumMember(Value = "Paymob")]
        Paymob = 2
    }

    // Define the enum for order statuses
    public enum OrderStatusEnum
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Approved")]
        Approved = 2,

        [Description("Paid")]
        Paid = 3,

        [Description("Rejected")]
        Rejected = 4,

        [Description("Canceled")]
        Canceled = 5,

        [Description("Completed")]
        Completed = 6

    }

}

