using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Contracts.Dtos
{

    public enum ProviderFileTypes
    {
        CriminalRecord,
        Image
    }
    public class ImageUploadDto
    { 

        [Required(ErrorMessage = "Image File path is Required ")]
        public string Base64Image { get; set; }
    }
}
