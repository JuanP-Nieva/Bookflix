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
    public class Perfil_Valora_LibroController : Controller
    {
        private readonly BookflixDbContext _context;

        public Perfil_Valora_LibroController(BookflixDbContext context)
        {
            _context = context;
        }

        // GET: Perfil_Valora_Libro
        public async Task<IActionResult> Index()
        {
            var bookflixDbContext = _context.Perfil_Valora_Libros.Include(p => p.Libro).Include(p => p.Perfil);
            return View(await bookflixDbContext.ToListAsync());
        }

        // GET: Perfil_Valora_Libro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil_Valora_Libro = await _context.Perfil_Valora_Libros
                .Include(p => p.Libro)
                .Include(p => p.Perfil)
                .FirstOrDefaultAsync(m => m.PerfilId == id);

            if (perfil_Valora_Libro == null)
            {
                return NotFound();
            }

            return View(perfil_Valora_Libro);
        }

        // GET: Perfil_Valora_Libro/Create
        public IActionResult Create( Perfil_Puntua_Libro ppl )
        {
            ViewBag.LibroId = ppl.LibroId;
            ViewBag.PerfilId = ppl.PerfilId;
            return View();
        }

        // POST: Perfil_Valora_Libro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibroId,PerfilId,Puntaje,Comentario,Spoiler,Visible")] Perfil_Valora_Libro perfil_Valora_Libro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(perfil_Valora_Libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LibroId"] = new SelectList(_context.Libros, "Id", "Descripcion", perfil_Valora_Libro.LibroId);
            ViewData["PerfilId"] = new SelectList(_context.Perfiles, "Id", "Nombre", perfil_Valora_Libro.PerfilId);
            return View(perfil_Valora_Libro);
        }

        // GET: Perfil_Valora_Libro/Edit/5
        public async Task<IActionResult> Edit(Perfil_Puntua_Libro ppl)
        {
            if (ppl == null)
            {
                return NotFound();
            }

            Perfil_Valora_Libro pvl = await _context.Perfil_Valora_Libros
                                        .FirstOrDefaultAsync(c => c.LibroId == ppl.LibroId && c.PerfilId == ppl.PerfilId);
            //pvl.Puntaje = ppl.Puntaje;

            return View(pvl);
        }

        // POST: Perfil_Valora_Libro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("LibroId,PerfilId,Puntaje,Comentario,Spoiler,Visible")] Perfil_Valora_Libro perfil_Valora_Libro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    perfil_Valora_Libro.Visible = !perfil_Valora_Libro.Spoiler;
                    _context.Update(perfil_Valora_Libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Perfil_Valora_LibroExists(perfil_Valora_Libro.PerfilId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details","Libro", new {id = perfil_Valora_Libro.LibroId});
            }

            return View(perfil_Valora_Libro);
        }

        // GET: Perfil_Valora_Libro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil_Valora_Libro = await _context.Perfil_Valora_Libros
                .Include(p => p.Libro)
                .Include(p => p.Perfil)
                .FirstOrDefaultAsync(m => m.PerfilId == id);
            if (perfil_Valora_Libro == null)
            {
                return NotFound();
            }

            return View(perfil_Valora_Libro);
        }

        // POST: Perfil_Valora_Libro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var perfil_Valora_Libro = await _context.Perfil_Valora_Libros.FindAsync(id);
            _context.Perfil_Valora_Libros.Remove(perfil_Valora_Libro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Perfil_Valora_LibroExists(int id)
        {
            return _context.Perfil_Valora_Libros.Any(e => e.PerfilId == id);
        }
    }
}
