using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sarvicny.Domain.Entities
{
    public class OrderStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string OrderStatusID { get; set; }
        public string StatusName { get; set; }
    }

    //public enum OrderStatusEnum
    //{
    //    [Description("Pending")]
    //    Pending = 1,

    //    [Description("Approved")]
    //    Approved = 2,

    //    [Description("Paid")]
    //    Paid = 3,

    //    [Description("Rejected")]
    //    Rejected = 4,

    //    [Description("Canceled")]
    //    Canceled = 5,

    //    [Description("Completed")]
    //    Completed = 6

    //}

}
