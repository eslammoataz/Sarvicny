using Sarvicny.Contracts.Authentication.Registers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Contracts.Dtos
{
    public class RegisterWorkerDto : RegisterDto
    {
        [Required(ErrorMessage = "National ID is Required")]
        public string NationalID { get; set; }

        [Required(ErrorMessage = "Criminal Record is Required")]
        public string CriminalRecord { get; set; }
        //image (fesh we tashbeh)
        //service 
    }
}
