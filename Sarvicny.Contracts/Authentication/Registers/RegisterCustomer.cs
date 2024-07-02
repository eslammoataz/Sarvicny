using System.ComponentModel.DataAnnotations;

namespace Sarvicny.Contracts.Authentication.Registers
{
    public class RegisterCustomer : RegisterDto
    {

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "District Name is Required")]
        public string DistrictName { get; set; }

    }
}