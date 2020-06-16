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

        // GET: Capitulo/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return RedirectToAction("Details", "Libro", new { Id = id});
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Finalizado([Bind("Id,LibroId,NumeroCapitulo,pdf,Titulo")] Capitulo capitulo)
        {
            if (capituloUnico(capitulo.LibroId, capitulo.NumeroCapitulo) || (capitulo.pdf == null))
            {
                ModelState.AddModelError("NumeroCapitulo", "Este número ya se encuentra en uso para otro capítulo");
            }

            if (ModelState.IsValid)
            {
                string stringFileNamePortada = this.UploadFile(capitulo.pdf, "Libros");
                var Chapter = new Capitulo
                {
                    LibroId = capitulo.LibroId,
                    Titulo = capitulo.Titulo,
                    NumeroCapitulo = capitulo.NumeroCapitulo,
                    Contenido = stringFileNamePortada
                };

                var libro = await _context.Libros
                 .FirstOrDefaultAsync(m => m.Id == Chapter.LibroId);

                libro.EstadoCompleto = true;

                 _context.Add(Chapter);
                 _context.Libros.Update(libro);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Libro");
                //return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Create", new { LibroId = capitulo.LibroId });
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
            return View(capitulo);
        }

        // POST: Capitulo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LibroId,NumeroCapitulo,Contenido")] Capitulo capitulo)
        {
            if (id != capitulo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(capitulo);
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
                return RedirectToAction(nameof(Index));
            }
            return View(capitulo);
        }

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