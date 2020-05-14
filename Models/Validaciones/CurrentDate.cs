using System.ComponentModel.DataAnnotations;
using System;

namespace Bookflix.Models.Validaciones
{
    public class CurrentDateAttribute : ValidationAttribute
    {
        public DateTime Date { get; }

        public string GetErrorMessage() =>
            $"Tenes que haber nacido antes del {Date}.";

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value != null)
            {
                if ((DateTime)value > Date)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            return ValidationResult.Success;
        }
    }
}