using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Bookflix.ViewModel
{
    public class EdicionNovedadViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Debe ingresar una descripción")]
        public string Descripcion { get; set; }
        public IFormFile Imagen { get; set; }
        [Required(ErrorMessage = "Debe ingresar un título")]
        public string Titulo { get; set; }
    }
}