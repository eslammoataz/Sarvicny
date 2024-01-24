using System.ComponentModel.DataAnnotations;

namespace Sarvicny.Contracts.Authentication.Registers
{
    public class RegisterCompany : RegisterDto
    {
        [Required(ErrorMessage = "licence is Required")]
        public string License { get; set; }
        //upload sora lel license
    }
}
