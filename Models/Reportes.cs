using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Bookflix.Models {
    public class Reportes {
        
        [Key]
        public int Id { get; set; }
        [Required]
        public int LibroId { get; set; }
        [Required]
        public int NumeroComentario { get; set; }
        [Required]
        public string Motivo { get; set; }

    }
}