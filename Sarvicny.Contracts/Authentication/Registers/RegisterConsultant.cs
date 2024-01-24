using System.ComponentModel.DataAnnotations;

namespace Sarvicny.Contracts.Authentication.Registers
{
    public class RegisterConsultant : RegisterDto
    {

        [Required(ErrorMessage = "Salary is Required")]
        public decimal Salary { get; set; }
    }
}
