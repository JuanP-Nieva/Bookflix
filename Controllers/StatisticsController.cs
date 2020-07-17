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


        public StatisticsController(BookflixDbContext context, UserManager<BookflixUser> userManager)
        {
            _context = context;
            _userManager = userManager;

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
                    var pl = _context.Perfil_Lee_Libros.AsEnumerable().GroupBy(pl => pl.LibroId).OrderByDescending(l => l.Count());

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

                    stats.Libros = new List<Libro>();

                    for (int i = 0; i < c; i++)
                    {
                        var libro = await _context.Libros
                                    .Include(c => c.Autor)
                                    .Include(c => c.Genero)
                                    .Include(c => c.Editorial)
                                    .FirstOrDefaultAsync(l => l.Id == lista.ElementAt(i));
                        stats.Libros.Add(libro);
                    }
                    break;
                case "UsuariosNormales":
                    ViewBag.Option = 2;
                    break;
            }



            var role = _context.Roles
                             .FirstOrDefault(n => n.Name == "Normal");

            stats.NormalUsers = _context.UserRoles
                                        .Where(d => d.RoleId.Equals(role.Id))
                                        .Count();

            stats.PremiumUsers = _context.Users
                                         .Count() - stats.NormalUsers - 1;

            ViewBag.PorcentajeNormal = stats.calcularPorcentajeNormal();
            ViewBag.PorcentajePremium = stats.calcularPorcentajePremium();

            ViewBag.TotalUsuarios = stats.NormalUsers + stats.PremiumUsers;

            return View(stats);
        }

    }
}