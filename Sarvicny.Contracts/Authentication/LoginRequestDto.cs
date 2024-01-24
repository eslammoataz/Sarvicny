using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;

namespace Sarvicny.Contracts.Authentication
{
    public class LoginRequestDto
    {
        [EmailAddress]
        [Required(ErrorMessage = "email is Required to login")]
        public string Email { get; set; }

        [Required(ErrorMessage = "password is Required to login")]
        public string password { get; set; }
        
        
    }
    
}