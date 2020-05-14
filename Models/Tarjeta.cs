using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Bookflix.Models
{
    public class Tarjeta
    {
        [Key]
        public int Numero { get; set; }
        [Required, DisplayName("Código de seguridad")]
        public int Clave { get; set; }
        [Required]
        public string Titular { get; set; }
        [Required]
        public string Tipo { get; set;}

        [Required, DataType(DataType.Date), DisplayName("Fecha de expiración")]
        public DateTime FechaDeVencimiento { get; set; } 

        //Propiedades para las relaciones de la DB
        public List<Pago> Pagos { get; set; }
    }
}