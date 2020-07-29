using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookflix.Models
{
    public class Perfil_Lee_Capitulo
    {
        //Propiedades para las relaciones de la DB
        
        public int PerfilId { get; set; }        
        public Perfil Perfil { get; set; }
        public int CapituloId { get; set; }
        public Capitulo Capítulo { get; set; }
    }
}