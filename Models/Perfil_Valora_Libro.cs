using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookflix.Models
{
    public class Perfil_Valora_Libro
    {

        //Propiedades para las relaciones de la DB
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
        public int PerfilId { get; set; }
        public Perfil Perfil { get; set; }
        public int Puntaje {get; set; }
       
        #nullable enable
        public string? Comentario { get; set; }
        #nullable disable
        public bool Spoiler { get; set; }
        public bool Visible{ get; set; }
        
    }
}