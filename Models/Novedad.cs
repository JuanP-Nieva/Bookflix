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
        public byte[] Imagen { get; set; }
        [Required]
        public string Titulo { get; set; }
    }
}