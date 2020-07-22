using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PaulZero.RoutineEnforcer.Models.Validation
{
    public class DaySelectionRequiredAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;

        public DaySelectionRequiredAttribute()
            : base("You must select at least one day for an event to run.")
        {
        }

        public override bool IsValid(object value)
        {
            if (value is DaySelection daySelection)
            {
                return daySelection.GetEnabledDays().Any();
            }

            return false;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!IsValid(value))
            {
                return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
