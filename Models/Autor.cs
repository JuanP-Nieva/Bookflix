using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Bookflix.Models
{
    public class Autor
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage="Debe ingresar el nombre")]
        public string Nombre { get; set; }
        [Required(ErrorMessage="Debe ingresar el apellido")]
        public string Apellido { get; set; }

        //Propiedades para las relaciones de la DB
        public List<Libro> Libros { get; set; }
             
    }
}