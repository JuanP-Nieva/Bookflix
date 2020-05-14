using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Bookflix.Models.Validaciones;
using System.Collections.Generic;

namespace Bookflix.Models
{
    public class BookflixUser : IdentityUser
    {
        [PersonalData, Required]
        public string Nombre { get; set; }
       
        [PersonalData, Required]
        public string Apellido { get; set; }

        [PersonalData, Required]
        public int Dni { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [PersonalData, Required, CurrentDate, DataType(DataType.DateTime)]
        public DateTime FechaDeNacimiento { get; set; }


        //Propiedades para las relaciones de la DB
        public List<Perfil> Perfiles { get; set; }
        public List<Pago> Pagos { get; set; }
    }
}