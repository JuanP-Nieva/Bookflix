using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Bookflix.Models;
using Bookflix.Models.Validaciones;
using Bookflix.Data;

namespace Bookflix.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<BookflixUser> _signInManager;
        private readonly UserManager<BookflixUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<BookflixUser> userManager,
            SignInManager<BookflixUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            //Datos de usuario

            [Required(ErrorMessage = "Este campo es obligatorio")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres y {1} como máximo.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no son iguales.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio"), RegularExpression(@"^[A-Za-z]*\s?()[A-Za-z]*$", ErrorMessage = "El {0} no puede empezar con espacios ni contener números")]
            [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} caracteres y {1} como máximo.", MinimumLength = 1)]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio"), RegularExpression(@"^[A-Za-z]*\s?()[A-Za-z]*$", ErrorMessage = "El {0} no puede empezar con espacios ni contener números")]
            [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} caracteres y {1} como máximo.", MinimumLength = 1)]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio"), RegularExpression(@"^[0-9]{7,8}$", ErrorMessage = "El {0} debe contener entre 7 y 8 dígitos")]

            [Display(Name = "DNI")]
            [DniUnico(ErrorMessage = "El DNI ya existe en la base de datos")]
            public int Dni { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio"), HastaFechaActual(ErrorMessage = "La fecha de nacimiento debe ser anterior al día de hoy.")]
            [Display(Name = "Fecha de nacimiento"), DataType(DataType.Date)]
            public DateTime FechaDeNacimiento { get; set; }

            //Datos de su tarjeta
            [Required(ErrorMessage = "Este campo es obligatorio"), RegularExpression(@"^[0-9]{16}$", ErrorMessage = "El {0} debe contener 16 dígitos")]
            public decimal Numero { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio"), Display(Name = "Código de seguridad"), RegularExpression(@"^[0-9]{3}$", ErrorMessage = "El {0} debe contener 3 dígitos")]
            public int Clave { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio")]
            [StringLength(100, ErrorMessage = "El nombre del {0} debe tener al menos {2} caracteres y {1} como máximo.", MinimumLength = 1)]
            public string Titular { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio")]
            public string Tipo { get; set; }

            [DataType(DataType.Date), Display(Name = "Fecha de expiración"), DesdeFechaActual(ErrorMessage = "La fecha de expiración debe ser posterior al día de hoy.")]
            [Required(ErrorMessage = "Este campo es obligatorio")]
            public DateTime FechaDeVencimiento { get; set; }

            [Required(ErrorMessage = "Este campo es obligatorio"), BindProperty]
            public string Categoria { get; set; }

            public Tarjeta crearTarjeta() => new Tarjeta
            {
                Numero = this.Numero,
                Clave = this.Clave,
                Titular = this.Titular,
                Tipo = this.Tipo,
                FechaDeVencimiento = this.FechaDeVencimiento
            };
            public BookflixUser CrearUsuario(UserManager<BookflixUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                BookflixUser user = new BookflixUser
                {
                    UserName = this.Email,
                    Email = this.Email,
                    Nombre = this.Nombre,
                    Apellido = this.Apellido,
                    Dni = this.Dni,
                    FechaDeNacimiento = this.FechaDeNacimiento
                };
                // await user.AgregarRol("Normal", userManager, roleManager);
                //user.CrearPago(categoroia, crearTarjeta()); 
                return user;
            }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                BookflixUser user = Input.CrearUsuario(_userManager, _roleManager);

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Input.Categoria);
                    _logger.LogInformation("User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                mostrarErrores(result);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private void mostrarErrores(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task crearLosRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole("Administrador"));
            await _roleManager.CreateAsync(new IdentityRole("Normal"));
            await _roleManager.CreateAsync(new IdentityRole("Premium"));
           // throw new Exception("todo piola");
        }
    }
}


/*
    if (result.Succeeded)
    {
        _logger.LogInformation("User created a new account with password.");

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = user.Id, code = code },
            protocol: Request.Scheme);                                          //Agregar aca la tarjeta y en el get de ConfirmEmail q la reciba como param

        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        if (_userManager.Options.SignIn.RequireConfirmedAccount)
        {
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
        }
        else
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }
    }
*/
