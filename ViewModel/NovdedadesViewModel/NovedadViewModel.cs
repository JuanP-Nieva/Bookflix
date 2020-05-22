using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Bookflix.ViewModel
{
    public class NovedadViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Descripcion { get; set; }
        public IFormFile Imagen { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Titulo { get; set; }
    }
}