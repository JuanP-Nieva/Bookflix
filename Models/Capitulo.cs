using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Bookflix.Models.Validaciones;

namespace Bookflix.Models
{
    public class Capitulo
    {
        [Key]
        public int Id { get; set; }
        public int LibroId { get; set; }
        [Required]
        [DisplayName("Número Capitulo"), Range(0,1000)]
        public int NumeroCapitulo { get; set; }
        public string Contenido { get; set; }
        [Required]
        public string Titulo { get; set; }
        
        [DesdeFechaActualAttribute]
        public DateTime? FechaDeVencimiento { get; set; }

        [NotMapped]
        [DisplayName("Subir Capitulo")]
        [Required]
        public IFormFile pdf { get; set; }
    }    
}