using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookflix.Data;
using Bookflix.Models;
using Bookflix.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Bookflix.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly BookflixDbContext _context;
        private readonly UserManager<BookflixUser> _userManager;

        private readonly RoleManager<BookflixUser> _roleManager;

        public StatisticsController(BookflixDbContext context, UserManager<BookflixUser> userManager, RoleManager<BookflixUser> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string options)
        {
            
            Statistics stats = new Statistics();
            if (options == null)
            {
                options = "MostrarLibroMasLeido";
            }

            switch (options)
            {
                
                case "MostrarLibroMasLeido":
                    var pl = _context.Perfil_Lee_Libros
                                 .Include(c => c.LibroId)
                                 .GroupBy(c => c.LibroId);

                    pl.OrderByDescending(c => c.Count());

                    
                    ViewBag.Option = 1;
                    List<int> lista = new List<int>();

                    foreach (var item in pl)
                    {
                        lista.Add(item.Key);
                    }

                    int c;
                    if (lista.Count < 10)
                    {
                        c = lista.Count;
                    }
                    else
                    {
                        c = 10;
                    }

                    for (int i = 0; i < c; i++)
                    {
                        var libro = await _context.Libros
                                    .FirstOrDefaultAsync(l => l.Id == lista.ElementAt(i));
                        stats.Libros.Add(libro);
                    }
                    break;
                case "UsuariosNormales":
                   ViewBag.Option = 2;
                    break;
                case "UsuariosPremium":
                    ViewBag.Option = 3;
                    break;
            }



            var id = _context.Roles
                             .Where(n => n.Name == "Normal");

            stats.NormalUsers = _context.UserRoles
                                        .Where(d => d.RoleId.Equals(id))
                                        .Count();

            stats.PremiumUsers = _context.Users
                                         .Count() - stats.NormalUsers - 1;


            return View(stats);
        }

    }
}