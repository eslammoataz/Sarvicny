using System.ComponentModel.DataAnnotations;

namespace Sarvicny.Contracts.Service;

    
public class AddServiceDto
{
    [Required(ErrorMessage = "Service Name is Required ")]
    public string ServiceName { get; set; }

    [Required(ErrorMessage = "Service Description is Required ")]
    public string Description { get; set; }
        

    public string? ParentServiceID { get; set; }


}