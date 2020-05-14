using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Bookflix.Models {
    public class Perfil {
        [Key]
        public int Id {get; set;}
        [Required]
        public string Nombre { get; set; }

        //Propiedades para las relaciones de la DB
        public BookflixUser Usuario { get; set; }
        public List<Perfil_Comenta_Libro> Perfil_Comenta_Libros { get; set; }
        public List<Perfil_Favea_Libro> Perfil_Favea_Libros { get; set; }
        public List<Perfil_Lee_Libro> Perfil_Lee_Libros { get; set; }
        public List<Perfil_Puntua_Libro> Perfil_Puntua_Libros { get; set; }

    }
}