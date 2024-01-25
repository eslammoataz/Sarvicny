using Sarvicny.Domain.Entities.Avaliabilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarvicny.Domain.Entities.Requests.AvailabilityRequestsValidations
{
    public class AvailabilityDto
    {
        [Required(ErrorMessage = "Day is Required")]
        [DayOfWeekValidation] // Apply custom validation attribute
        public string DayOfWeek { get; set; }


        public DateTime? AvailabilityDate { get; set; }


        [Required(ErrorMessage = "Time slots is Required ")]
        public List<TimeRange> Slots { get; set; } = new List<TimeRange>();
    }
    public class DayOfWeekValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string)
            {
                string day = value.ToString();
                if (!Enum.IsDefined(typeof(DayOfWeek), day))
                {
                    return new ValidationResult("The 'Day' property must be a valid day of the week.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
