using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bookflix.Data;
using System.Linq;

namespace Bookflix.Models 
{
    public class Perfil_Comenta_Libro {
        [Required]
        public int NumeroComentario { get; set; }
        [Required]
        public string Comentario { get; set; }

        public string MarcaSpoiler { get; set; }

        //Propiedades para las relaciones de la DB
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
        
        public int PerfilId { get; set; }
        public Perfil Perfil { get; set; }

        public BookflixUser obtenerUsuario()
        {
            using (var db = new BookflixDbContext())
            {
                var usuarios = db.Users.ToList();
                var perfil = db.Perfiles.FirstOrDefault(p => p.Id == this.PerfilId);
                                
                foreach(var user in usuarios)
                {
                    if(user.Perfiles.Contains(perfil))
                    {
                        return user;
                    }
                }                 
            }

            return null;
        }

        public bool esSpoiler()
        {
            return MarcaSpoiler != "NoSpoiler";
        }
    }
}