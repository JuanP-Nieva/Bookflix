using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Bookflix.Models.Validaciones;

namespace Bookflix.ViewModel
{
    public class EdicionCapituloViewModel
    {
        
        public int Id { get; set; }
        public int LibroId { get; set; }
        [Required(ErrorMessage="Debe ingresar el numero de capitulo")]
        [DisplayName("Número Capitulo"), RegularExpression(@"^[1-9]{1,3}$", ErrorMessage = "El numero de capitulo debe estar entre 1 y 999")]
        public int NumeroCapitulo { get; set; }
        
        public IFormFile Contenido { get; set; }
        [Required(ErrorMessage="Debe ingresar el titulo")]
        public string Titulo { get; set; }
        
        [DataType(DataType.Date), Display(Name = "Fecha de expiración"), DesdeFechaActual(ErrorMessage = "La fecha de expiración debe ser posterior al día de hoy.")]
        [Required(ErrorMessage = "Debe ingresar la fecha de expiración del capitulo.")]
        public DateTime? FechaDeVencimiento { get; set; }

    }    
}