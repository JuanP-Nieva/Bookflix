using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookflix.Models
{
    public class Usuario_Recibe_Notificacion
    {
        //Propiedades para las relaciones de la DB
        public string BookflixUserId { get; set; }
        public BookflixUser BookflixUser { get; set; }

        public int NotificacionId { get; set; }
        public Notificacion Notificacion { get; set; }
    }
}