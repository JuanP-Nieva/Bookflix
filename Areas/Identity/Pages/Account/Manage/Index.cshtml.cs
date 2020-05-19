using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bookflix.Models;

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
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            [Required]
            [Display(Name = "DNI")]
            public int Dni { get; set; }

            [Required]
            [Display(Name = "Fecha de nacimiento")]
            public DateTime FechaDeNacimiento { get; set; }
        }

        private async Task LoadAsync(BookflixUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Dni = user.Dni,
                FechaDeNacimiento = user.FechaDeNacimiento
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se ha podido cargar al usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se ha podido encontrar al usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            user.Nombre = Input.Nombre;
            user.Apellido = Input.Apellido;
            user.Dni = Input.Dni;
            user.FechaDeNacimiento = Input.FechaDeNacimiento;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Tu perfil ha sido actualizado";
            return RedirectToPage();
        }
    }
}
