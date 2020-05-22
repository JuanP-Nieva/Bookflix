using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bookflix.Data;
using System;

namespace Bookflix.Models.Validaciones
{
    public class IsbnUnicoAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => $"El ISBN tiene que ser único";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((value != null) && (isbnDuplicado((int)value)))
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

        protected virtual bool isbnDuplicado(int isbn)
        {
            using (BookflixDbContext db = new BookflixDbContext())
            {
                return (db.Libros.Any(libro => libro.ISBN == isbn));  //Si hay libros con ISBN repetido.
            }
        }
    }
}