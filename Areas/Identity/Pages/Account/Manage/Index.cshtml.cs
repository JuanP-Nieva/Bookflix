using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookflix.Models.Validaciones;
using Bookflix.Models;
using Bookflix.Data;

namespace Bookflix.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<BookflixUser> _userManager;
        private readonly SignInManager<BookflixUser> _signInManager;

        public IndexModel(
            UserManager<BookflixUser> userManager,
            SignInManager<BookflixUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Debe ingresar su nombre."), RegularExpression(@"^[A-Za-z]*\s?()[A-Za-z]*$", ErrorMessage = "El {0} no puede empezar con espacios ni contener números.")]
            [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} caracteres y {1} como máximo.", MinimumLength = 1)]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "Debe ingresar su apellido."), RegularExpression(@"^[A-Za-z]*\s?()[A-Za-z]*$", ErrorMessage = "El {0} no puede empezar con espacios ni contener números.")]
            [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} caracteres y {1} como máximo.", MinimumLength = 1)]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            [Required(ErrorMessage = "Debe ingresar su DNI."), RegularExpression(@"^[0-9]{7,8}$", ErrorMessage = "El {0} debe contener entre 7 y 8 dígitos.")]
            [Display(Name = "DNI")]
            public int Dni { get; set; }

            [Required(ErrorMessage = "Debe ingresar su fecha de nacimiento."), HastaFechaActual(ErrorMessage = "La fecha de nacimiento debe ser anterior al día de hoy.")]
            [Display(Name = "Fecha de nacimiento"), DataType(DataType.Date)]
            public DateTime FechaDeNacimiento { get; set; }
        }

        private async Task LoadAsync(BookflixUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Apellido = user.Apellido,
                Nombre = user.Nombre,
                Dni = user.Dni,
                FechaDeNacimiento = user.FechaDeNacimiento
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar al usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar al usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            if (this.existeUsuario(Input.Dni, user.Email))
            { 
                ModelState.AddModelError("DNI", "Este DNI ya se encuentra en la base de datos");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            await this.actualizarUsuario(user);
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

        private async Task actualizarUsuario(BookflixUser user)
        {
            using (BookflixDbContext db = new BookflixDbContext())
            {
                if (user.Apellido != Input.Apellido || user.Nombre != Input.Nombre || user.Dni != Input.Dni || user.FechaDeNacimiento != Input.FechaDeNacimiento)
                {
                    user.Apellido = Input.Apellido;
                    user.Nombre = Input.Nombre;
                    user.Dni = Input.Dni;
                    user.FechaDeNacimiento = Input.FechaDeNacimiento;
                    db.Update(user);
                    await db.SaveChangesAsync();
                    StatusMessage = "Tu perfil ha sifo actualizado";
                }
            }
        }
        private bool existeUsuario(int dni, string email)
        {
            using (BookflixDbContext db = new BookflixDbContext())
            {
                return db.Users.Any(user => user.Email != email && user.Dni == dni);
            }
        }
    }
}
