using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bookflix.Models;
using Bookflix.Data;

namespace Bookflix.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {

           /* this.CrearGenero("Terror");
            this.CrearGenero("Suspenso");
            this.CrearGenero("Comedia");
            this.CrearGenero("Fantasia");

            this.crearEditorial("Santilla", "Argentina");
            this.crearEditorial("Larousse", "Francia");
            this.crearEditorial("Onebox", "USA");
            this.crearEditorial("Minotauro", "España");

            this.crearAutor("Edgar Allan", "Poe");
            this.crearAutor("J. K", "Rowling");
            this.crearAutor("John", "Katzenbach");
            this.crearAutor("Alfred", "Hitchcock");*/

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void CrearGenero(string item)
        {
            using (var _context = new BookflixDbContext())
            {
                var genero = new Genero()
                {
                    Nombre = item
                };
                _context.Generos.Add(genero);
                _context.SaveChanges();
            }
        }

        public void crearEditorial(string item, string itemDos)
        {
            using (var _context = new BookflixDbContext())
            {
                var editorial = new Editorial()
                {
                    Nombre = item,
                    Pais = itemDos
                };
                _context.Editoriales.Add(editorial);
                _context.SaveChanges();
            }
        }

        public void crearAutor(string item, string itemDos)
        {
            using (var _context = new BookflixDbContext())
            {
                var autor = new Autor()
                {
                    Nombre = item,
                    Apellido = itemDos
                };
                _context.Autores.Add(autor);
                _context.SaveChanges();
            }
        }
    }
}
