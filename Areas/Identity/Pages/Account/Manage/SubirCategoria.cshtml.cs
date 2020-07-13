using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Bookflix.Models;

namespace Bookflix.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class SubirCategoriaModel : PageModel
    {
        private readonly SignInManager<BookflixUser> _signInManager;
        private readonly UserManager<BookflixUser> _userManager;
        private readonly ILogger<SubirCategoriaModel> _logger;

        public SubirCategoriaModel(SignInManager<BookflixUser> signInManager, UserManager<BookflixUser> userManager, ILogger<SubirCategoriaModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar al usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            await user.ChangeRole(_userManager, "Premium", "Normal");
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index", "Perfil");
        }
    }
}
