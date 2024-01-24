using System.ComponentModel.DataAnnotations;

namespace Sarvicny.Contracts.Authentication.Registers
{
    public abstract class RegisterDto
    {
        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }


        [Required(ErrorMessage = "FirstName is Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is Required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is Required")]
        public string PhoneNumber { get; set; }


    }
}
