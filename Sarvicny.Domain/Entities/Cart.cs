using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sarvicny.Domain.Entities.Users;

namespace Sarvicny.Domain.Entities
{
    public class Cart
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CartID { get; set; }


        public string CustomerID { get; set; }

        public List<CartServiceRequest>? ServiceRequests { get; set; } = new List<CartServiceRequest>();

        //public DateTime? AddedTime { get; set; }

        public DateTime? LastChangeTime { get; set; }


        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

    }
}
