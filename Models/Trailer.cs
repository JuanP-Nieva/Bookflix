using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Bookflix.Models
{
    public class Trailer
    {
        [Key]
        public int Id { get; set; }
        public string Imagen { get; set; }

        public string Descripcion { get; set; }

        [NotMapped]
        [DisplayName("Subir Trailer")]
        [Required]
        public IFormFile Img { get; set; }

        [Required]
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
    }
}