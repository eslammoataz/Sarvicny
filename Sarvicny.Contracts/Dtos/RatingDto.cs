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
    public class RatingDto
    {

        [Required(ErrorMessage = "Rate is Required")]
        [Range(0, 5, ErrorMessage = "Rate must be between 0 and 5")]
        public int Rate { get; set; }

        [Required(ErrorMessage = "Comment is Required")]
        public string Comment { get; set; }
    }
}
