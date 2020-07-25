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
using Microsoft.AspNetCore.Identity;

namespace Bookflix.Controllers
{
    public class LibroController : Controller
    {
        private static int PerfilActual;
        private readonly BookflixDbContext _context;
        private readonly UserManager<BookflixUser> _userManager;
        private readonly IWebHostEnvironment WebHostEnvironment;

        private static List<Libro> librosActuales; //Esto se agrego 17/7 de prueba

        private static bool BienComentado;

        public LibroController(BookflixDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<BookflixUser> userManager)
        {
            _context = context;
            WebHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult IndexInicial(int id) //Guardo el ID del perfil para saber cual es el perfil actual
        {
            PerfilActual = id;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index(string options, string searchString)
        {
            IQueryable<Libro> bookflixDbContext;

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (options)
                {
                    case "BuscarEditorial":
                        bookflixDbContext = _context.Libros.Where(l => l.Editorial.Nombre.Contains(searchString)).Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
                        break;
                    case "BuscarAutor":
                        bookflixDbContext = _context.Libros.Where(l => l.Autor.Nombre.Contains(searchString) || l.Autor.Apellido.Contains(searchString)).Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
                        break;
                    case "BuscarGenero":
                        bookflixDbContext = _context.Libros.Where(l => l.Genero.Nombre.Contains(searchString)).Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
                        break;
                    default:
                        bookflixDbContext = _context.Libros.Where(l => l.Titulo.Contains(searchString)).Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
                        break;
                }
            }
            else
            {
                bookflixDbContext = _context.Libros.Include(l => l.Autor).Include(l => l.Editorial).Include(l => l.Genero);
            }

            librosActuales = bookflixDbContext.ToList(); 

            if (User.IsInRole("Administrador"))
            {
                return View("Index", librosActuales);
            }
            else
            {
                ViewBag.Perfil = _context
                                .Perfiles
                                .FirstOrDefault( p => p.Id == PerfilActual);
                return View("IndexSuscriptor", librosActuales);
            }
        }


        public IActionResult OrdenarLista(string options)
        {
            switch (options)
            {
                case "OrdenarEditorial":
                    librosActuales.Sort(delegate (Libro x, Libro y)
                    {
                        if (x.Editorial.Nombre == null && y.Editorial.Nombre == null) return 0;
                        else if (x.Editorial.Nombre == null) return -1;
                        else if (y.Editorial.Nombre == null) return 1;
                        else return x.Editorial.Nombre.CompareTo(y.Editorial.Nombre);
                    });
                    break;
                case "OrdenarAutor":
                    librosActuales.Sort(delegate (Libro x, Libro y)
                    {
                        if (x.Autor.Nombre == null && y.Autor.Nombre == null) return 0;
                        else if (x.Autor.Nombre == null) return -1;
                        else if (y.Autor.Nombre == null) return 1;
                        else return x.Autor.Nombre.CompareTo(y.Autor.Nombre);
                    });
                    break;
                case "OrdenarGenero":
                    librosActuales.Sort(delegate (Libro x, Libro y)
                    {
                        if (x.Genero.Nombre == null && y.Genero.Nombre == null) return 0;
                        else if (x.Genero.Nombre == null) return -1;
                        else if (y.Genero.Nombre == null) return 1;
                        else return x.Genero.Nombre.CompareTo(y.Genero.Nombre);
                    });
                    break;
                default:
                    librosActuales.Sort(delegate (Libro x, Libro y)
                    {
                        if (x.Titulo == null && y.Titulo == null) return 0;
                        else if (x.Titulo == null) return -1;
                        else if (y.Titulo == null) return 1;
                        else return x.Titulo.CompareTo(y.Titulo);
                    });
                    break;
            }

            if (User.IsInRole("Administrador"))
            {
                return View("Index", librosActuales);
            }
            else
            {
                ViewBag.Perfil = _context
                                .Perfiles
                                .FirstOrDefault( p => p.Id == PerfilActual);
                return View("IndexSuscriptor", librosActuales);
            }
        }

        //HASTA ACA

        [HttpPost]
        public async Task<IActionResult> Comentar(string comentario, int idLibro)
        {
            if (comentario == null)
            {
                BienComentado = true;
                return RedirectToAction("Details", new {id = idLibro});
            }
            BienComentado = false;

            var libro = await _context.Libros
                             .Where(l => l.Id == idLibro)
                             .FirstOrDefaultAsync();

            libro.CantidadComentarios++;
            Perfil_Comenta_Libro coment = new Perfil_Comenta_Libro
            {
                NumeroComentario = (int)libro.CantidadComentarios,
                LibroId = idLibro,
                PerfilId = PerfilActual,
                Comentario = comentario,
            };

            _context.Libros.Update(libro);
            _context.Perfil_Comenta_Libros.Add(coment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details",new { id = idLibro }); //Aca lo mandas a donde quieras
        }

        // GET: Libro/Details/5
        [HttpGet]
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

            libro.Perfil_Comenta_Libros = _context.Perfil_Comenta_Libros
                                        .Where(c => c.LibroId == libro.Id)
                                        .ToList();

            libro.Perfil_Lee_Libros = _context.Perfil_Lee_Libros
                                    .Where(l => l.LibroId == libro.Id && l.Finalizado)
                                    .ToList();


            ViewBag.PuedeVer = libro.Perfil_Lee_Libros.Exists(c =>
                        c.PerfilId == PerfilActual);

            ViewBag.PuedeComentar = false;

            if (ViewBag.PuedeVer)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                ViewBag.PuedeComentar = user.Habilitado;
            }

            var puntuacion = _context
                            .Perfil_Puntua_Libros
                            .FirstOrDefault(p => p.LibroId == id && p.PerfilId == PerfilActual);

            if (puntuacion == null)
            {
                ViewBag.Puntaje = 0;
            }
            else
            {
                ViewBag.Puntaje = puntuacion.Puntaje;
            }

            ViewBag.ComentarioVacio = BienComentado;

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

            LibroViewModel L = new LibroViewModel{
                Id = libro.Id,
                perfilID = PerfilActual
            };

            /*if (libro.Contenido == null)
            {
                libro.Capitulos = _context.Capitulos
                       .Where(c => c.LibroId == libro.Id)
                       .OrderBy(c => c.NumeroCapitulo)
                       .ToList();
            }*/

            if (libro.Contenido == null)
            {
                return RedirectToAction("Prueba", "Capitulo", L);
            }

            this.AgregarLecturaDePerfil((int)id);

            return View(libro);
        }

        //Agrego a la tabla Perfil-Lee-Libros el id del perfil y el id del libro        
        public void AgregarLecturaDePerfil(int id)
        {
            var perfilLeeLibro = new Perfil_Lee_Libro
            {
                PerfilId = PerfilActual,
                LibroId = id,
                Finalizado = true
            };

            using (var db = new BookflixDbContext())
            {
                if (!db.Perfil_Lee_Libros.Any(pll => pll.LibroId == id && pll.PerfilId == PerfilActual))
                {
                    db.Perfil_Lee_Libros.Add(perfilLeeLibro);
                    db.SaveChanges();
                }
            }
        }


        public IActionResult VerHistorial()
        {
            var perfil = _context.Perfiles
                .Include(x => x.Perfil_Lee_Libros)
                .ThenInclude(l => l.Libro)
                .Where(per => per.Id == PerfilActual)
                .Single();


            return View(perfil);
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
                    EstadoCompleto = false,
                    CantidadComentarios = 0
                };

                
                _context.Add(libro);
                await _context.SaveChangesAsync();

                return RedirectToAction("Create", "Capitulo", new { id = libro.Id});
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
            if (l.Portada == null)
            {
                ModelState.AddModelError("Contenido", "Debe seleccionar un libro para subir");
            }
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
                    EstadoCompleto = true,
                    CantidadComentarios = 0
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
                GeneroId = libro.GeneroId,
                Capitulos = _context.Capitulos
                                .Where(c => c.LibroId == libro.Id)
                                .OrderBy(c => c.NumeroCapitulo)
                                .ToList()

            };
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Apellido", libro.AutorId);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", libro.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", libro.GeneroId);
            return View(lvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditConCapitulos(int id, [Bind("Id,ISBN,Portada,Contenido,Titulo,Descripcion,AutorId,GeneroId,EditorialId")] EdicionLibroViewModel l)
        {
            if (id != l.Id)
            {
                return NotFound();
            }

            var libro = _context.Libros.FirstOrDefault(v => v.Id == id);

            if (this.existeLibro(l.ISBN, l.Id))
            {
                ModelState.AddModelError("ISBN", "Este ISBN ya se encuentra en uso para otro libro");
            }

            if (ModelState.IsValid && (!this.existeLibro(l.ISBN, l.Id) || isbnEditable(l.ISBN, l.Id)))
            {
                try
                {
                    libro.ISBN = l.ISBN;
                    libro.Id = l.Id;
                    libro.Portada = checkearPorNull(l.Portada, libro.Portada, "Images");
                    libro.Titulo = l.Titulo;
                    libro.Contenido = null;
                    libro.Descripcion = l.Descripcion;
                    libro.AutorId = l.AutorId;
                    libro.EditorialId = l.EditorialId;
                    libro.GeneroId = l.GeneroId;

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
            l.Capitulos = _context.Capitulos
                                .Where(c => c.LibroId == libro.Id)
                                .OrderBy(c => c.NumeroCapitulo)
                                .ToList();
            ViewData["AutorId"] = new SelectList(_context.Autores, "Id", "Apellido", l.AutorId);
            ViewData["EditorialId"] = new SelectList(_context.Editoriales, "Id", "Nombre", l.EditorialId);
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Nombre", l.GeneroId);
            return View(l);
        }

        private bool existeLibro(decimal isbn, int id)
        {
            return _context.Libros.Any(i => i.Id != id && i.ISBN == isbn);
        }


        private string checkearPorNull(IFormFile imagen, string imagen2, string path)
        {
            if (imagen == null)
            {
                return imagen2;
            }
            else
            {
                return this.UploadFile(imagen, path);
            }
        }

        private bool isbnEditable(decimal isbn, int id)
        {

            return _context.Libros.Any(libro => libro.ISBN == isbn && libro.Id == id);
        }

        public async Task<IActionResult> VerComentarios(int? id)
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

            libro.Perfil_Comenta_Libros = _context.Perfil_Comenta_Libros
                                        .Where(c => c.LibroId == libro.Id)
                                        .ToList();
            return View(libro);
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Libro");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComentario(int libroId, int nro)
        {
            Perfil_Comenta_Libro p = await _context.Perfil_Comenta_Libros
                                    .FirstOrDefaultAsync(c => c.LibroId == libroId && c.NumeroComentario == nro);

            //Borro tambien los reportes porque sino quedan y no puedo acceder mas
            List<Reportes> reportes = _context.Reportes
                                    .Where( r => r.LibroId == libroId && r.NumeroComentario == nro)
                                    .ToList();
            _context.Reportes.RemoveRange(reportes);
           
            _context.Perfil_Comenta_Libros.Remove(p);
            await _context.SaveChangesAsync();
            return RedirectToAction("VerComentarios", new { id = libroId});
        }

        public async Task<IActionResult> MarcarSpoiler(int nro, int libroId)
        {   
            Perfil_Comenta_Libro p = await _context.Perfil_Comenta_Libros
                                    .FirstOrDefaultAsync(c => c.LibroId == libroId && c.NumeroComentario == nro);

            p.MarcaSpoiler = "Spoiler";

            await _context.SaveChangesAsync();
            return RedirectToAction("VerComentarios", new { id = libroId});
        }

        public async Task<IActionResult> DesmarcarSpoiler(int nro, int libroId)
        {   
            Perfil_Comenta_Libro p = await _context.Perfil_Comenta_Libros
                                    .FirstOrDefaultAsync(c => c.LibroId == libroId && c.NumeroComentario == nro);

            p.MarcaSpoiler = "NoSpoiler";

            await _context.SaveChangesAsync();
            return RedirectToAction("VerComentarios", new { id = libroId});
        }


        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.ISBN == id);
        }

        public IActionResult AgregarCapitulos()
        {
            return View();
        }

        public async Task<IActionResult> AgregarFavorito(int id)
        {
            Perfil_Favea_Libro libroFavorito = new Perfil_Favea_Libro()
            {
                LibroId = id,
                PerfilId = PerfilActual
            };

            _context.Perfil_Favea_Libros.Add(libroFavorito);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> QuitarFavorito(int id)
        {
            Perfil_Favea_Libro elemento = _context.Perfil_Favea_Libros.FirstOrDefault(p => p.LibroId == id && p.PerfilId == PerfilActual);
            _context.Perfil_Favea_Libros.Remove(elemento);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Calificar(int libroId, int value)
        {
            if (this.yaEstaCalificado(libroId)) //Si el libro ya estaba calificado lo actualiza por la nueva puntuacion
            {
                var puntuacion = _context.Perfil_Puntua_Libros.FirstOrDefault(p => p.LibroId == libroId && p.PerfilId == PerfilActual);
                puntuacion.Puntaje = value;
                _context.Update(puntuacion);

            }
            else
            {
                Perfil_Puntua_Libro puntuacion = new Perfil_Puntua_Libro()
                {
                    PerfilId = PerfilActual,
                    LibroId = libroId,
                    Puntaje = value
                };
                _context.Perfil_Puntua_Libros.Add(puntuacion);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = libroId });
        }

        private bool yaEstaCalificado(int libroId)
        {
            return _context.Perfil_Puntua_Libros.Any(p => p.LibroId == libroId && p.PerfilId == PerfilActual);
        }
    }
}