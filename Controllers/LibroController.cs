using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookflix.Data;
using Bookflix.Models;
using Microsoft.AspNetCore.Authorization;
using Bookflix.Models.Validaciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Bookflix.ViewModel;
using iTextSharp.text.pdf;

namespace Bookflix.Controllers
{

    public class LibroController : Controller
    {
        private readonly BookflixDbContext _context;

        private readonly IWebHostEnvironment WebHostEnvironment;
        public LibroController(BookflixDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            WebHostEnvironment = webHostEnvironment;
        }

        // GET: Libro
        public async Task<IActionResult> Index(string options, string searchString)
        {
            IQueryable<Libro> bookflixDbContext;
            
            if(!String.IsNullOrEmpty(searchString))
            {
                if(options == "BuscarTitulo"){
                    bookflixDbContext = _context.Libros.Where(l => l.Titulo.Contains(searchString)).Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
                }else
                    if(options == "BuscarAutor"){
                        bookflixDbContext = _context.Libros.Where(l => l.Autor.Nombre.Contains(searchString)).Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
                    }else
                        if(options == "BuscarGenero"){
                            bookflixDbContext = _context.Libros.Where(l => l.Genero.Nombre.Contains(searchString)).Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
                        }else
                            bookflixDbContext = _context.Libros.Where(l => l.Editorial.Nombre.Contains(searchString)).Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
                
            }
            else{
                bookflixDbContext = _context.Libros.Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
            }
            if(User.IsInRole("Administrador"))
            {
                return View("Index",await bookflixDbContext.ToListAsync());
            }else{
                return View("IndexSuscriptor",await bookflixDbContext.ToListAsync());
            }
            
        }


        // GET: Libro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Editorial)
                .Include(l => l.Genero)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            if (libro.Contenido == null)
            {
                libro.Capitulos = _context.Capitulos
                        .Where(c => c.LibroId == libro.Id)
                        .OrderBy(c => c.NumeroCapitulo)
                        .ToList();
            }

            return View(libro);
        }

        public async Task<IActionResult> Leer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Editorial)
                .Include(l => l.Genero)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        



        [Authorize(Roles = "Administrador")]
        // GET: Libro/Create
        public IActionResult Create()
        {
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre");
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre");
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre");
            return View();
        }

         [Authorize(Roles = "Administrador")]
        // GET: Libro/CreateConCapitulos
        public IActionResult CreateConCapitulos()
        {
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre");
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre");
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateConCapitulos([Bind("ISBN,Portada,Titulo,Descripcion,AutorId,GeneroId,EditorialId")] LibroViewModel l)
        {
            if (ModelState.IsValid && !isbnUnico(l.ISBN))
            {
                string stringFileNamePortada = this.UploadFile(l.Portada, "Images");
                var libro = new Libro

                {
                    ISBN = l.ISBN,
                    Portada = stringFileNamePortada,
                    Titulo = l.Titulo,
                    Contenido = null,
                    Descripcion = l.Descripcion,
                    AutorId = l.AutorId,
                    EditorialId = l.EditorialId,
                    GeneroId = l.GeneroId,
                    EstadoCompleto = false
                };
                _context.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Capitulo", new { id =libro.Id });
                // return RedirectToAction(nameof(Index));
            }
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre", l.AutorId);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", l.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", l.GeneroId);
            return View(l);
        }

        // POST: Libro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("ISBN,Portada,Titulo,Contenido,Descripcion,AutorId,GeneroId,EditorialId")] LibroViewModel l)
        {
            if (ModelState.IsValid && !isbnUnico(l.ISBN))
            {
                string stringFileNamePortada = this.UploadFile(l.Portada, "Images");
                string stringFileNameContenido = this.UploadFile(l.Contenido, "Libros");
                var libro = new Libro

                {
                    ISBN = l.ISBN,
                    Portada = stringFileNamePortada,
                    Titulo = l.Titulo,
                    Contenido = stringFileNameContenido,
                    Descripcion = l.Descripcion,
                    AutorId = l.AutorId,
                    EditorialId = l.EditorialId,
                    GeneroId = l.GeneroId,
                    EstadoCompleto = true
                };
                _context.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Nombre", l.AutorId);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", l.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", l.GeneroId);
            return View(l);
        }


        private string UploadFile(IFormFile image, string path)
        {
            string fileName = null;
            if (image != null)
            {
                string uploadDir = Path.Combine(WebHostEnvironment.WebRootPath, path);
                fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        private bool isbnUnico(decimal isbn)
        {
            return _context.Libros.Any(libro => libro.ISBN == isbn);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            var lvm = new EdicionLibroViewModel
            {
                ISBN = libro.ISBN,
                Id = libro.Id,
                Portada = null,
                Titulo = libro.Titulo,
                Contenido = null,
                Descripcion = libro.Descripcion,
                AutorId = libro.AutorId,
                EditorialId = libro.EditorialId,
                GeneroId = libro.GeneroId
            };
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Apellido", libro.AutorId);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", libro.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(lvm);
        }

        // POST: Libro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ISBN,Portada,Titulo,Contenido,Descripcion,AutorId,GeneroId,EditorialId")] EdicionLibroViewModel l)
        {
            if (id != l.Id)
            {
                return NotFound();
            }

            if (isbnEditable(l.ISBN, l.Id) && (l.Portada == null || l.Contenido == null))
            {
                var libro = _context.Libros.FirstOrDefault(v => v.Id == id);
                
                libro.ISBN = l.ISBN;
                libro.Id = l.Id;
                libro.Portada = checkearPorNull(l.Portada, libro.Portada, "Images");
                libro.Titulo = l.Titulo;
                libro.Contenido = checkearPorNull(l.Contenido, libro.Contenido, "Libros");
                libro.Descripcion = l.Descripcion;
                libro.AutorId = l.AutorId;
                libro.EditorialId = l.EditorialId;
                libro.GeneroId = l.GeneroId;


                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            


            if (ModelState.IsValid && isbnEditable(l.ISBN, l.Id))
            {
                try
                {
                    string stringFileNamePortada = this.UploadFile(l.Portada, "Images");
                    string stringFileNameContenido = this.UploadFile(l.Contenido, "Libros");
                    var libro = new Libro
                    {
                        Id = l.Id,
                        ISBN = l.ISBN,
                        Portada = stringFileNamePortada,
                        Titulo = l.Titulo,
                        Contenido = stringFileNameContenido,
                        Descripcion = l.Descripcion
                    };
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(l.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Apellido", l.AutorId);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", l.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", l.GeneroId);
            return View(l);
        }

        
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditConCapitulos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);
            
            if (libro == null)
            {
                return NotFound();
            }

            var lvm = new EdicionLibroViewModel
            {
                ISBN = libro.ISBN,
                Id = libro.Id,
                Portada = null,
                Titulo = libro.Titulo,
                Contenido = null,
                Descripcion = libro.Descripcion,
                AutorId = libro.AutorId,
                EditorialId = libro.EditorialId,
                GeneroId = libro.GeneroId
            };
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Apellido", libro.AutorId);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", libro.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(lvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditConCapitulos(int id, [Bind("Id,ISBN,Portada,Titulo,Descripcion,AutorId,GeneroId,EditorialId")] EdicionLibroViewModel l)
        {
            if (id != l.Id)
            {
                return NotFound();
            }

            if (isbnEditable(l.ISBN, l.Id) && (l.Portada == null))
            {
                var libro = _context.Libros.FirstOrDefault(v => v.Id == id);
                
                libro.ISBN = l.ISBN;
                libro.Id = l.Id;
                libro.Portada = checkearPorNull(l.Portada, libro.Portada, "Images");
                libro.Titulo = l.Titulo;
                libro.Contenido = null;
                libro.Descripcion = l.Descripcion;
                libro.AutorId = l.AutorId;
                libro.EditorialId = l.EditorialId;
                libro.GeneroId = l.GeneroId;
                

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            


            if (ModelState.IsValid && isbnEditable(l.ISBN, l.Id))
            {
                try
                {
                    string stringFileNamePortada = this.UploadFile(l.Portada, "Images");
                    var libro = new Libro
                    {
                        Id = l.Id,
                        ISBN = l.ISBN,
                        Portada = stringFileNamePortada,
                        Titulo = l.Titulo,
                        Contenido = null,
                        Descripcion = l.Descripcion
                    };
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(l.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Apellido", l.AutorId);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", l.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", l.GeneroId);
            return View(l);
        }

        private string checkearPorNull(IFormFile imagen, string imagen2, string path)
        {
            if(imagen == null)
            {
                return imagen2;
            }else{
                return this.UploadFile(imagen, path);
            }
        }

        private bool isbnEditable(decimal isbn, int id)
        {

            return _context.Libros.Any(libro => libro.ISBN == isbn && libro.Id == id);
        }

        // GET: Libro/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.Editorial)
                .Include(l => l.Genero)
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.ISBN == id);
        }

        public IActionResult AgregarCapitulos()
        {
            return View();
        }

    }
}
