using System.ComponentModel.DataAnnotations;

namespace Sarvicny.Contracts.Criteria;

public class AddCriteriaDto
{

    [Required(ErrorMessage = "Criteria Name is Required ")]
    public string CriteriaName { get; set; }

    [Required(ErrorMessage = "Criteria Description is Required ")]
    public string Description { get; set; }
}