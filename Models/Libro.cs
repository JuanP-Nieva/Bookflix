using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Collections.Generic;

namespace Bookflix.Models {
    public class Libro {
        [Key]
        public int ISBN { get; set; }
        
        public byte[] Portada { get; set; }
       
        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Contenido { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public int AutorId { get; set; }
        [Required(ErrorMessage = "El libro debe tener un autor")]
        public Autor Autor { get; set; }
        public int GeneroId { get; set; }
        [Required(ErrorMessage = "El libro debe tener un género")]
        public Genero Genero { get; set; }
        public int EditorialId { get; set; }
        [Required(ErrorMessage = "El libro debe tener una editorial")]
        public Editorial Editorial { get; set; }

        //Propiedades para las relaciones de la DB
        public List<Perfil_Comenta_Libro> Perfil_Comenta_Libros { get; set; }
        public List<Perfil_Favea_Libro> Perfil_Favea_Libros { get; set; }
        public List<Perfil_Lee_Libro> Perfil_Lee_Libros { get; set; }
        public List<Perfil_Puntua_Libro> Perfil_Puntua_Libros { get; set; }
    }
}