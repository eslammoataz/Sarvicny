using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities
{
    public class FavProvider
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string favId { get; set; }
        public string customerId { get; set; }
         public string providerId { get; set; }

    }
}
