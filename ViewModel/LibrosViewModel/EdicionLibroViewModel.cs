using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Bookflix.Models;

namespace Bookflix.ViewModel {
    public class EdicionLibroViewModel
    {      
        public int Id { get; set; }

        [Required(ErrorMessage="El ISBN es un campo obligatorio"), RegularExpression(@"^[0-9]{10,13}$", ErrorMessage = "El {0} debe contener entre 10 y 13 dígitos")]
        [DisplayFormat(DataFormatString = "{0:F0}", ApplyFormatInEditMode = true)]
        public decimal ISBN { get; set; }
        
        public IFormFile Portada { get; set; }
       
        [Required(ErrorMessage="El Titulo es un campo obligatorio")]
        public string Titulo { get; set; }

        
        public IFormFile Contenido { get; set; }

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

        public List<Capitulo> Capitulos { get; set;}

        //Propiedades para las relaciones de la DB
        public List<Perfil_Comenta_Libro> Perfil_Comenta_Libros { get; set; }
        public List<Perfil_Favea_Libro> Perfil_Favea_Libros { get; set; }
        public List<Perfil_Lee_Libro> Perfil_Lee_Libros { get; set; }
        public List<Perfil_Puntua_Libro> Perfil_Puntua_Libros { get; set; }
    }
}