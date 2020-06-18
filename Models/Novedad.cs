using System;
using System.ComponentModel.DataAnnotations;

namespace Bookflix.Models
{
    public class Novedad
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage="Debe ingresar la descripcion")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage="Debe ingresar una imagen")]
        public string Imagen { get; set; }
        [Required(ErrorMessage="Debe ingresar el titulo")]
        public string Titulo { get; set; }
    }
}