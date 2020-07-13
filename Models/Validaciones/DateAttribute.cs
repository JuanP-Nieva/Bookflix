using System.ComponentModel.DataAnnotations;
using System;

namespace Bookflix.Models.Validaciones
{
    public abstract class DateAttribute : ValidationAttribute
    {
        public abstract string GetErrorMessage();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && errorDeFecha((DateTime)value))
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

        protected abstract bool errorDeFecha(DateTime fecha);
    }
}