using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookflix.Data;
using Bookflix.Models;

namespace Bookflix.Controllers
{
    public class Perfil_Lee_LibroController : Controller
    {
        private readonly BookflixDbContext _context;

        public Perfil_Lee_LibroController(BookflixDbContext context)
        {
            _context = context;
        }

        // GET: Perfil_Lee_Libro
        public async Task<IActionResult> Index()
        {
            var bookflixDbContext = _context.Perfil_Lee_Libros.Include(p => p.Libro).Include(p => p.Perfil);
            return View(await bookflixDbContext.ToListAsync());
        }

        // GET: Perfil_Lee_Libro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil_Lee_Libro = await _context.Perfil_Lee_Libros
                .Include(p => p.Libro)
                .Include(p => p.Perfil)
                .FirstOrDefaultAsync(m => m.PerfilId == id);
                
            if (perfil_Lee_Libro == null)
            {
                return NotFound();
            }

            return View(perfil_Lee_Libro);
        }

        // GET: Perfil_Lee_Libro/Create
        public IActionResult Create()
        {
            ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion");
            ViewData["PerfilId"] = new SelectList(_context.Perfiles, "Id", "Nombre");
            return View();
        }

        // POST: Perfil_Lee_Libro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibroId,PerfilId,Finalizado")] Perfil_Lee_Libro perfil_Lee_Libro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(perfil_Lee_Libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion", perfil_Lee_Libro.LibroId);
            ViewData["PerfilId"] = new SelectList(_context.Perfiles, "Id", "Nombre", perfil_Lee_Libro.PerfilId);
            return View(perfil_Lee_Libro);
        }

        // GET: Perfil_Lee_Libro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil_Lee_Libro = await _context.Perfil_Lee_Libros.FindAsync(id);
            if (perfil_Lee_Libro == null)
            {
                return NotFound();
            }
            ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion", perfil_Lee_Libro.LibroId);
            ViewData["PerfilId"] = new SelectList(_context.Perfiles, "Id", "Nombre", perfil_Lee_Libro.PerfilId);
            return View(perfil_Lee_Libro);
        }

        // POST: Perfil_Lee_Libro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LibroId,PerfilId,Finalizado")] Perfil_Lee_Libro perfil_Lee_Libro)
        {
            if (id != perfil_Lee_Libro.PerfilId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(perfil_Lee_Libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Perfil_Lee_LibroExists(perfil_Lee_Libro.PerfilId))
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
            ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion", perfil_Lee_Libro.LibroId);
            ViewData["PerfilId"] = new SelectList(_context.Perfiles, "Id", "Nombre", perfil_Lee_Libro.PerfilId);
            return View(perfil_Lee_Libro);
        }

        // GET: Perfil_Lee_Libro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil_Lee_Libro = await _context.Perfil_Lee_Libros
                .Include(p => p.Libro)
                .Include(p => p.Perfil)
                .FirstOrDefaultAsync(m => m.PerfilId == id);
                
            if (perfil_Lee_Libro == null)
            {
                return NotFound();
            }

            return View(perfil_Lee_Libro);
        }

        // POST: Perfil_Lee_Libro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var perfil_Lee_Libro = await _context.Perfil_Lee_Libros.FindAsync(id);
            _context.Perfil_Lee_Libros.Remove(perfil_Lee_Libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Perfil_Lee_LibroExists(int id)
        {
            return _context.Perfil_Lee_Libros.Any(e => e.PerfilId == id);
        }
    }
}
