using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookflix.Data;
using Bookflix.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Bookflix.Controllers
{
    public class TrailerController : Controller
    {
        private readonly BookflixDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TrailerController(BookflixDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Trailer
        public async Task<IActionResult> Index()
        {
            var bookflixDbContext = _context.Trailers.Include(t => t.Libro);
            return View(await bookflixDbContext.ToListAsync());
        }

        // GET: Trailer/Details/5
        public async Task<IActionResult> Details(int? id) // ojo que recibe el id del libro xq esta dificil pasarselo desde la vista sino
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = await _context.Trailers
                .Include(t => t.Libro)
                .FirstOrDefaultAsync(m => m.LibroId == id); //Lo busca por id de libro xq es un capo mal
            if (trailer == null)
            {
                return NotFound();
            }

            return View(trailer);
        }

        // GET: Trailer/Create
        public IActionResult Create(int? id)
        {
            //ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion");
            if (id == null)
            {
                return NotFound();
            }

            ViewData["LibroId"] = id;  // si tenes dudas de que hace esto preguntale a cisco
            return View();
        }

        // POST: Trailer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Img,Descripcion,LibroId")] Trailer trailer)
        {
            if (ModelState.IsValid)
            {
                //trailer.Imagen = this.UploadFile(trailer.Img, "Images");
                var _trailer = new Trailer
                {
                    Descripcion = trailer.Descripcion,
                    Imagen = this.UploadFile(trailer.Img, "Images"),
                    LibroId = trailer.LibroId
                };
                _context.Add(_trailer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Libro");
            }
            //ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion", trailer.LibroId);
            return View(trailer);
        }

        // GET: Trailer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = await _context.Trailers.FindAsync(id);
            if (trailer == null)
            {
                return NotFound();
            }
            ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion", trailer.LibroId);
            return View(trailer);
        }

        // POST: Trailer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Img,Descripcion,LibroId")] Trailer trailer)
        {
            if (id != trailer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trailer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrailerExists(trailer.Id))
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
            ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion", trailer.LibroId);
            return View(trailer);
        }

        // GET: Trailer/Delete/5
        public async Task<IActionResult> Delete(int? id) //recibe el id del librooooooo
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = await _context.Trailers
                .Include(t => t.Libro)
                .FirstOrDefaultAsync(m => m.LibroId == id);
            if (trailer == null)
            {
                return NotFound();
            }
            _context.Trailers.Remove(trailer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Libro");
        }

        // POST: Trailer/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var trailer = await _context.Trailers.FindAsync(id);
        //     _context.Trailers.Remove(trailer);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        private bool TrailerExists(int id)
        {
            return _context.Trailers.Any(e => e.Id == id);
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
    }
}
