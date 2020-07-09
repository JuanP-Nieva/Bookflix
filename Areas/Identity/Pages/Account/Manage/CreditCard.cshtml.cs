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
    public partial class CreditCardModel : PageModel
    {
        private readonly UserManager<BookflixUser> _userManager;
        private readonly SignInManager<BookflixUser> _signInManager;

        public CreditCardModel(
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


            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

       
    }
}