using Sarvicny.Domain.Entities.Users;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Sarvicny.Domain.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderID { get; set; }
        public string CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        public string OrderDetailsId { get; set; }


        [ForeignKey("OrderDetailsId")]
        public OrderDetails OrderDetails { get; set; }

        public string? customerRatingId { get; set; }

        public OrderRating? CRate { get; set; }


        public string? providerRatingId { get; set; }

        public OrderRating? PRate { get; set; }

        public DateTime OrderDate { get; set; }

        public bool IsPaid { get; set; }

        public string TransactionPaymentId { get; set; }

        public TransactionPayment TransactionPayment { get; set; }

        public DateTime? CancelDate { get; set; }

        [NotMapped]
        public OrderStatusEnum OrderStatus { get; set; } = OrderStatusEnum.Pending;

        // Property for database persistence
        public string OrderStatusString
        {
            get => OrderStatus.ToString();
            set => OrderStatus = Enum.Parse<OrderStatusEnum>(value);
        }


    }

    public enum PaymentMethod
    {
        [EnumMember(Value = "Paypal")]
        Paypal = 1,

        [EnumMember(Value = "Paymob")]
        Paymob = 2,

        [EnumMember(Value = "Cash")]
        Cash = 3
    }
    // Define the enum for order statuses
    public enum OrderStatusEnum
    {
        [Description("Pending")] //intial state
        Pending = 1,

        [Description("Paid")]
        Paid = 2,

        [Description("Start")]
        Start = 3,

        [Description("Preparing")]
        Preparing = 4,

        [Description("On The Way")]
        OnTheWay = 5,

        [Description("In Progress")]
        InProgress = 6,

        [Description("Done")]
        Done = 7,

        [Description("Completed")]
        Completed = 8,


        [Description("Removed")]  //8lta customer
        Removed = 9,

        [Description("CanceledByProvider")]
        CanceledByProvider = 10,

        [Description("ReAssigned")]
        ReAssigned = 11,

        [Description("Canceled")] //by customer
        Canceled = 12,

        [Description("Refunded")]
        Refunded = 13,

        [Description("RemovedWithRefund")]
        RemovedWithRefund = 14

    }


}

