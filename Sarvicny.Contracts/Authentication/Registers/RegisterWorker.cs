using System.ComponentModel.DataAnnotations;

namespace Sarvicny.Contracts.Authentication.Registers
{
    public class RegisterWorker : RegisterDto
    {
        [Required(ErrorMessage = "National ID is Required")]
        public string NationalID { get; set; }

        [Required(ErrorMessage = "Criminal Record is Required")]
        public string CriminalRecord { get; set; }
        //image (fesh we tashbeh)
        //service  

    }
}
