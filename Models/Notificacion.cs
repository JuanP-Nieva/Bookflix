using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Bookflix.Models
{
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage="La notificación debe tener contenido")]
        public string Contenido { get; set; }

        //Propiedades para las relaciones de la DB
        public List<Usuario_Recibe_Notificacion> Usuario_Recibe_Notificaciones { get; set; }
    }
}