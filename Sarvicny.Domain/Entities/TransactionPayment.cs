using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sarvicny.Domain.Entities
{
    public class TransactionPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TransactionPaymentID { get; set; }

        public string? TransactionID { get; set; }

        public decimal Amount { get; set; }

        public string? SaleID { get; set; }

        public required List<Order> OrderList { get; set; }


        public PaymentMethod? PaymentMethod { get; set; }

        // update when payment is made
        public TransactionPaymentStatusEnum TransactionPaymentStatus { get; set; } = TransactionPaymentStatusEnum.Pending;

        public string TransactionPaymentStatusString
        {
            get => TransactionPaymentStatus.ToString();
            set => TransactionPaymentStatus = Enum.Parse<TransactionPaymentStatusEnum>(value);
        }

        public DateTime? PaymentDate { get; set; }  // update when payment is made

        //public DateTime? PaymentExpiryTime { get; set; } = DateTime.UtcNow.AddHours(1);
    }

    public enum TransactionPaymentStatusEnum
    {
        Pending,
        Success,
        Failed
    }
}
