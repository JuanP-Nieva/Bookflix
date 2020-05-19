using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bookflix.Data;
using System;

namespace Bookflix.Models.Validaciones
{
    public class DniUnicoAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => $"El DNI tiene que ser único";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ( (value != null) && (dniDuplicado((int)value)) )
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }
        
        private bool dniDuplicado(int dni)
        {
            using(BookflixDbContext db = new BookflixDbContext())
                {
                    return (db.Users.Any(user => user.Dni == dni));  //Si hay usuarios con DNI repetido.
                }
        }
    }
}