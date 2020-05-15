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

namespace Bookflix.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<BookflixUser> _signInManager;
        private readonly UserManager<BookflixUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<BookflixUser> userManager,
            SignInManager<BookflixUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]                              //POELE LA REGEX CORRESPONDIENTE A LA PASSWD
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]                              //PONELE LA REGEX PARA QUE NO PUEDAN METER ESPACIOS AL PRINCIPIO
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Required]                      //IDEM NOMBRE
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            [Required]              //TIENE QUE SER UNA CANTIDAD X DE DIGITOS
            [Display(Name = "DNI")]
            public int Dni { get; set; }
            
            [Required]
            [Display(Name = "Fecha de nacimiento"), DataType(DataType.Date)]
            public DateTime FechaDeNacimiento { get; set; }
                    // ARREGLAR QUE NO PUEDA METER FECHAS FUTURAS

            //Categoria
      //      public string Categoria { get; set; }
       //     public string[] Categorias = new string[] {"Normal", "Premium"};

            //Datos de su tarjeta
       //     [Required]
       //     public int Numero { get; set; }
        //    [Required, Display(Name = "Código de seguridad"), Range(3,3)]
        //    public int Clave { get; set; }
         //   [Required]
       //     public string Titular { get; set; }
        //    [Required]
       //     public string Tipo { get; set;}

       //     [Required, DataType(DataType.Date), Display(Name = "Fecha de expiración")]
       //     public DateTime FechaDeVencimiento { get; set; }            
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid && Input.FechaDeNacimiento < DateTime.Today && Input.FechaDeNacimiento.Year > 1900)
            {
                var user = new BookflixUser { 
                    UserName = Input.Email,
                    Email = Input.Email,
                    Nombre = Input.Nombre,
                    Apellido = Input.Apellido,
                    Dni = Input.Dni,
                    FechaDeNacimiento = Input.FechaDeNacimiento
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
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
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
