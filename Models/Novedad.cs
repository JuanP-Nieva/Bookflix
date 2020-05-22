using System;
using System.ComponentModel.DataAnnotations;

namespace Bookflix.Models
{
    public class Novedad
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string Imagen { get; set; }
        [Required]
        public string Titulo { get; set; }
    }
}