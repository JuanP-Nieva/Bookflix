using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Bookflix.Data;
using System.Linq;

namespace Bookflix.Models
{
    public class BookflixUser : IdentityUser
    {
        [PersonalData, Required]
        public string Nombre { get; set; }

        [PersonalData, Required]
        public string Apellido { get; set; }

        [PersonalData, Required]
        public int Dni { get; set; }

        [Required]
        public bool Habilitado { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [PersonalData, Required, DataType(DataType.DateTime)]
        public DateTime FechaDeNacimiento { get; set; }

        //Propiedades para las relaciones de la DB
        public List<Usuario_Recibe_Notificacion> Usuario_Recibe_Notificaciones { get; set; }
        public List<Perfil> Perfiles { get; set; }
        public List<Pago> Pagos { get; set; }

        //Métodos

        public async Task ChangeRole(UserManager<BookflixUser> userManager, string newRole, string previousRole)
        {
            await userManager.RemoveFromRoleAsync(this, previousRole);
            await userManager.AddToRoleAsync(this, newRole);
        }   

        public bool tienePerfil(int id)
        {
            using (var db = new BookflixDbContext())
            {
                return db.Perfiles.Any(p => p.Usuario.Id == this.Id && p.Id == id);
            }
        }
    }
}