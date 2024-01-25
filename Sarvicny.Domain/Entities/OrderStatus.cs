using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class OrderStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string OrderStatusID { get; set; }
        public string StatusName { get; set; }
    }
}
