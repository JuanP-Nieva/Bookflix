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
    public class ReportesController : Controller
    {
        private readonly BookflixDbContext _context;

        public ReportesController(BookflixDbContext context)
        {
            _context = context;
        }

        // GET: Reportes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reportes.ToListAsync());
        }

        // GET: Reportes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportes = await _context.Reportes
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (reportes == null)
            {
                return NotFound();
            }

            Perfil_Valora_Libro p = _context.Perfil_Valora_Libros
                                    .FirstOrDefault(c => c.LibroId == reportes.LibroId && c.PerfilId == reportes.PerfilId);

            if (p == null)
            {
                return NotFound();
            }
            ViewBag.Libro = _context.Libros
                                .Include(l => l.Autor)
                                .Include(l => l.Editorial)
                                .Include(l => l.Genero)
                                .FirstOrDefault(c => c.Id == reportes.LibroId); //Agrego el libro para mostrar info del mismo en el details de reporte
            ViewBag.IdReporte = id;
            ViewBag.Motivo = reportes.Motivo;
            return View(p);
        }

        // GET: Reportes/Create
        public IActionResult Create(int idLibro, int perfilId)
        {
            if (idLibro == 0 || perfilId == 0)
            {
                return NotFound();
            }
            ViewBag.IdLibro = idLibro;
            ViewBag.PerfilId = perfilId;
            return View();
        }

        // POST: Reportes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LibroId,PerfilId,Motivo")] Reportes reportes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reportes);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details","Libro", new {id = reportes.LibroId});
            }

            ViewBag.IdLibro = reportes.LibroId;
            ViewBag.PerfilId = reportes.PerfilId;
            return View(reportes);
        }

        // GET: Reportes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportes = await _context.Reportes.FindAsync(id);
            if (reportes == null)
            {
                return NotFound();
            }
            return View(reportes);
        }

        // POST: Reportes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LibroId,NumeroComentario,Motivo")] Reportes reportes)
        {
            if (id != reportes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reportes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportesExists(reportes.Id))
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
            return View(reportes);
        }

        // GET: Reportes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportes = await _context.Reportes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportes == null)
            {
                return NotFound();
            }

            _context.Reportes.Remove(reportes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Reportes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int perfilId)
        {
            Perfil_Valora_Libro p = await _context.Perfil_Valora_Libros
                                    .FirstOrDefaultAsync(c => c.LibroId == id && c.PerfilId == perfilId);
            List<Reportes> reportes = _context.Reportes.Where( r => r.LibroId == id && r.PerfilId == perfilId).ToList();
            _context.Reportes.RemoveRange(reportes);
            p.Comentario = null;
            _context.Perfil_Valora_Libros.Update(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> RechazarReporte(int id)
        {
            Reportes reportes = await _context.Reportes
                                .FirstOrDefaultAsync(c => c.Id == id);
            _context.Reportes.Remove(reportes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportesExists(int id)
        {
            return _context.Reportes.Any(e => e.Id == id);
        }

    }
}
