using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class OrderRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RatingId { get; set; }

        public string OrderID { get; set; }

        [ForeignKey("OrderID")]
        public Order Order { get; set; }

        public int Rate {  get; set; }

        public string Comment { get; set; }




    }
}
