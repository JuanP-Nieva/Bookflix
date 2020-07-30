using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bookflix.Data;
using System.Linq;

namespace Bookflix.Models
{
    public class Perfil_Valora_Libro
    {

        //Propiedades para las relaciones de la DB
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
        public int PerfilId { get; set; }
        public Perfil Perfil { get; set; }
        public int Puntaje { get; set; }

#nullable enable
        public string? Comentario { get; set; }
#nullable disable
        public bool Spoiler { get; set; }
        public bool Visible { get; set; }

        public BookflixUser obtenerUsuario()
        {
            using (var db = new BookflixDbContext())
            {
                var usuarios = db.Users.ToList();
                var perfil = db.Perfiles.FirstOrDefault(p => p.Id == this.PerfilId);

                foreach (var user in usuarios)
                {
                    if (user.Perfiles.Exists(p => p.Id == perfil.Id))
                    {
                        return user;
                    }
                }
            }

            return null;
        }

    }
}