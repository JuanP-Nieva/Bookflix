using System;
using System.ComponentModel.DataAnnotations;

namespace Bookflix.Models {
    public class Pago {
        [Key]
        public int Id { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Monto { get; set; }

        //Propiedades para las relaciones de la DB
        public BookflixUser Usuario { get; set; }
        public Tarjeta Tarjeta { get; set; }

    }
}