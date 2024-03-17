using Sarvicny.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Contracts.Dtos
{
    public class CustomerRatingDto
    {
        public string serviceRequestID { get; set; }
       
        public int customerRating { get; set; }
        public String Comment { get; set; }
    }
}
