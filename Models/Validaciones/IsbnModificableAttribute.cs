using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bookflix.Data;
using System;
using Bookflix.Models.Validaciones;

public class IsbnModificableAttribute: IsbnUnicoAttribute
{
    protected override bool isbnDuplicado(int isbn)
    {
        using (var db = new BookflixDbContext())
        {
            return base.isbnDuplicado(isbn) && db.Libros.First().ISBN.Equals(isbn);
        }            
    }
}