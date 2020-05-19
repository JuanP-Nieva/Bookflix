// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Identity;
// using System.Threading.Tasks;

// namespace Bookflix.Models
// {
//     public abstract class Categoria
//     {
//         [Key]
//         public string Rol { get; private set; }

//         //Lista para la db
//         public List<BookflixUser> Usuarios { get; set; }

//        //Métodos 
//         public Categoria(string rol) => this.Rol = rol;

//         public async Task AgregarRolAsync(BookflixUser usuario, UserManager<BookflixUser> userManager, RoleManager<IdentityRole> roleManager)
//         {
//             await userManager.RemoveFromRolesAsync(usuario, await userManager.GetRolesAsync(usuario));
//             await roleManager.CreateAsync(new IdentityRole(this.Rol));
//             await userManager.AddToRoleAsync(usuario, this.Rol);
//             this.Usuarios.Add(usuario);
//         }
//     }

//     public class Administrador : Categoria
//     {
//         public Administrador () : base("Administrador"){}
//     }

//     public abstract class Suscripcion : Categoria
//     {
//         public Suscripcion(string rol) : base(rol){} 
//     }

//     public class Normal : Suscripcion
//     {
//         public Normal () : base("Normal"){}
//     }

//     public class Premium : Suscripcion
//     {
//         public Premium() : base("Premium"){}
//     }
// }