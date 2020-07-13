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
    public class CapituloController : Controller
    {
        private readonly BookflixDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CapituloController(BookflixDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Capitulo
        public async Task<IActionResult> Index()
        {
            return View(await _context.Capitulos.ToListAsync());
        }

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

            var capitulo = await _context.Capitulos
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (capitulo == null)
            {
                return NotFound();
            }

            return View(capitulo);
        }


        public IActionResult cerrarCapitulo (int? id){
            if (id == null){
                return NotFound();
            }

            return RedirectToAction("Details", "Capitulo", new { Id = id});
        }

        // GET: Capitulo/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["LibroId"] = id;
            return View();
        }

        // POST: Capitulo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,LibroId,Titulo,NumeroCapitulo,FechaDeVencimiento,pdf")] Capitulo capitulo)
        {
            if (capituloUnico(capitulo.LibroId, capitulo.NumeroCapitulo) || capitulo.pdf == null)
            {
                ModelState.AddModelError("NumeroCapitulo", "Este número ya se encuentra en uso para otro capítulo");
            }

            if (ModelState.IsValid)
            {
                string stringFileNamePortada = this.UploadFile(capitulo.pdf, "Libros");
                var Chapter = new Capitulo
                {
                    Titulo = capitulo.Titulo,
                    LibroId = capitulo.LibroId,
                    NumeroCapitulo = capitulo.NumeroCapitulo,
                    FechaDeVencimiento = capitulo.FechaDeVencimiento,
                    Contenido = stringFileNamePortada
                };

                _context.Add(Chapter);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", new { LibroId = Chapter.LibroId });
                //return RedirectToAction(nameof(Index));
            }
            ViewData["LibroId"] = capitulo.LibroId;
            return View(capitulo);
            //return RedirectToAction("Create", new { LibroId = capitulo.LibroId });
        }


        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Administrador")]
        // public async Task<IActionResult> Finalizado([Bind("Id,LibroId,NumeroCapitulo,FechaDeVencimiento,pdf,Titulo")] Capitulo capitulo)
        // {
        //     if (capituloUnico(capitulo.LibroId, capitulo.NumeroCapitulo) || (capitulo.pdf == null))
        //     {
        //         ModelState.AddModelError("NumeroCapitulo", "Este número ya se encuentra en uso para otro capítulo");
        //     }

        //     if (ModelState.IsValid)
        //     {
        //         string stringFileNamePortada = this.UploadFile(capitulo.pdf, "Libros");
        //         var Chapter = new Capitulo
        //         {
        //             LibroId = capitulo.LibroId,
        //             Titulo = capitulo.Titulo,
        //             NumeroCapitulo = capitulo.NumeroCapitulo,
        //             FechaDeVencimiento = capitulo.FechaDeVencimiento,
        //             Contenido = stringFileNamePortada
        //         };

        //         var libro = await _context.Libros
        //          .FirstOrDefaultAsync(m => m.Id == Chapter.LibroId);

        //         libro.EstadoCompleto = true;

        //          _context.Add(Chapter);
        //          _context.Libros.Update(libro);
        //         await _context.SaveChangesAsync();

        //         return RedirectToAction("Index", "Libro");
        //         //return RedirectToAction(nameof(Index));
        //     }

        //     return RedirectToAction("Create", new { LibroId = capitulo.LibroId });
        // }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> LibroCompleto(int id)
        {
            var libro = await _context.Libros
                .FirstOrDefaultAsync(m => m.Id == id);
            libro.EstadoCompleto = true;
            _context.Libros.Update(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Libro");
        }

        private string UploadFile(IFormFile image, string path)
        {
            string fileName = null;
            if (image != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, path);
                fileName = image.FileName;
                string filePath = Path.Combine(uploadDir, image.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            return fileName;
        }


        [Authorize(Roles = "Administrador")]
        // GET: Capitulo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capitulo = await _context.Capitulos.FindAsync(id);
            if (capitulo == null)
            {
                return NotFound();
            }

            var capituloVM = new EdicionCapituloViewModel
            {
                Id = capitulo.Id,
                LibroId = capitulo.LibroId,
                NumeroCapitulo = capitulo.NumeroCapitulo,
                Contenido = null,
                Titulo = capitulo.Titulo,
                FechaDeVencimiento = capitulo.FechaDeVencimiento
            };
            
            return View(capituloVM);
        }


        // POST: Capitulo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LibroId,NumeroCapitulo,Contenido,Titulo,FechaDeVencimiento")] EdicionCapituloViewModel capitulo)
        {
            if (id != capitulo.Id)
            {
                return NotFound();
            }

            var capi = _context.Capitulos.FirstOrDefault(v => v.Id == id);
            
            if(this.existeCapitulo(capitulo.NumeroCapitulo, capitulo.Id, capi.LibroId))
            {
                ModelState.AddModelError("NumeroCapitulo", "Este número ya se encuentra en uso para otro capítulo");
            }


            if (ModelState.IsValid && ( !this.existeCapitulo(capitulo.NumeroCapitulo, capitulo.Id, capi.LibroId) || this.numeroCapituloEditable(capitulo.NumeroCapitulo,capitulo.Id)))
            {
                try
                {
                    capi.Id = capitulo.Id;
                    capi.NumeroCapitulo = capitulo.NumeroCapitulo;
                    capi.Contenido = this.checkearPorNull(capitulo.Contenido,capi.Contenido,"Libros");
                    capi.Titulo = capitulo.Titulo;
                    capi.FechaDeVencimiento = capitulo.FechaDeVencimiento;

                    _context.Update(capi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CapituloExists(capitulo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditConCapitulos","Libro", new { id = capi.LibroId });
            }
            return View(capitulo);
        }

        private bool numeroCapituloEditable(int numeroCapitulo,int id)
        {

            return _context.Capitulos.Any(capitulo => capitulo.Id == id && capitulo.NumeroCapitulo == numeroCapitulo);
        }

        private bool existeCapitulo(int numeroCapitulo, int id, int idLibro)
        {
            return _context.Capitulos.Any(capitulo => capitulo.Id != id && capitulo.NumeroCapitulo == numeroCapitulo && capitulo.LibroId == idLibro);
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

        [Authorize(Roles = "Administrador")]
        // GET: Capitulo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capitulo = await _context.Capitulos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (capitulo == null)
            {
                return NotFound();
            }

            return View(capitulo);
        }

        // POST: Capitulo/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var capitulo = await _context.Capitulos.FindAsync(id);
            _context.Capitulos.Remove(capitulo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CapituloExists(int id)
        {
            return _context.Capitulos.Any(e => e.Id == id);
        }

        private bool capituloUnico(int LibroId, int NumeroCapitulo)
        {
            var capitulosLibro = _context.Capitulos.Where(cap => cap.LibroId == LibroId);
            return capitulosLibro.Any(caps => caps.NumeroCapitulo == NumeroCapitulo);

        }
    }
}