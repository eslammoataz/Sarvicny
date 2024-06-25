using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Sarvicny.Domain.Entities.Avaliabilities;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using static Sarvicny.Domain.Entities.OrderDetails;

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

        public DateTime? ExpiryDate { get; set; }

        public DateTime? PaymentExpiryTime { get; set; }    

        public bool IsPaid { get; set; }

        public string? TransactionID { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }


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

        [Description("Start")]
        Start = 6,

        [Description("Preparing")]
        Preparing = 7,

        [Description("On The Way")]
        OnTheWay = 8,

        [Description("In Progress")]
        InProgress = 9,

        [Description("Done")]
        Done = 10,

        [Description("Completed")]
        Completed = 11,

        [Description("Removed")]
        Removed = 12

    }


}

