using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Collections.Generic;
using Bookflix.Models;
using Bookflix.Models.Validaciones;

namespace Bookflix.Models {
    public class Libro {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="El ISBN es un campo obligatorio")]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true)]
        public decimal ISBN { get; set; }
        
        public string Portada { get; set; }
       
        [Required(ErrorMessage="El Titulo es un campo obligatorio")]
        public string Titulo { get; set; }

        // [Required(ErrorMessage="El Contenido es un campo obligatorio")] esto no va comentado, es para probar
        public string Contenido { get; set; }


        [Required(ErrorMessage="La Descripcion es un campo obligatorio")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El libro debe tener un autor")]
        public int AutorId { get; set; }
        public Autor Autor { get; set; }
        [Required(ErrorMessage = "El libro debe tener un género")]
        public int GeneroId { get; set; }
        public Genero Genero { get; set; }
        [Required(ErrorMessage = "El libro debe tener una editorial")]
        public int EditorialId { get; set; }
        public Editorial Editorial { get; set; }

        #nullable enable
        public Trailer? Trailer { get; set; }

        #nullable disable
        public bool EstadoCompleto { get; set; }

        //Propiedades para las relaciones de la DB
        public List<Perfil_Comenta_Libro> Perfil_Comenta_Libros { get; set; }
        public List<Perfil_Favea_Libro> Perfil_Favea_Libros { get; set; }
        public List<Perfil_Lee_Libro> Perfil_Lee_Libros { get; set; }
        public List<Perfil_Puntua_Libro> Perfil_Puntua_Libros { get; set; }


        public List<Capitulo> Capitulos { get; set; }
    }
}