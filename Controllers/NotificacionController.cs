using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookflix.Data;
using Bookflix.Models;


namespace Bookflix.Controllers
{
    public class NotificacionController : Controller
    {
        private readonly BookflixDbContext _context;
        private readonly UserManager<BookflixUser> _userManager;
        private static List<BookflixUser> _users;

        public NotificacionController(BookflixDbContext context, UserManager<BookflixUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Notificacion
        public async Task<IActionResult> Index(string email)
        {
            BookflixUser user = await _userManager.FindByEmailAsync(email);
            List<Usuario_Recibe_Notificacion> urn = await _context.Usuario_Recibe_Notificaciones.Where(urn => urn.BookflixUserId == user.Id).ToListAsync();
            List<Notificacion> l = new List<Notificacion>();
            foreach (var item in urn)
            {
                l.Add(_context.Notificaciones.First(n => n.Id == item.NotificacionId));
            }
            return View(l);
        }

        // GET: Notificacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            return View(notificacion);
        }

        public IActionResult CrearUnico(string id)
        {
            BookflixUser user = _context.Users.First(user => user.Id == id);
            _users = new List<BookflixUser>();
            _users.Add(user);
            return RedirectToAction("Create");
        }

        // GET: Notificacion/Create
        public IActionResult Create(List<BookflixUser> users = null)
        {
            return View();
        }

        public IActionResult Notiglobal (){

            return View();
        }

        // POST: Notificacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Contenido")] Notificacion notificacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction("crear", new { id = notificacion.Id });

            }
            return View(notificacion);
        }

        public async Task<IActionResult> Creaglobal([Bind("Id, Contenido")] Notificacion notificacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction("SendAll", new { id = notificacion.Id });

            }
            return View(notificacion);
        }

        public async Task<IActionResult> SendAll(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }
            var users = _context.Users.ToList();

            foreach (BookflixUser user in users)
            {
                Usuario_Recibe_Notificacion urn = new Usuario_Recibe_Notificacion
                {
                    BookflixUserId = user.Id,
                    NotificacionId = notificacion.Id
                };
                _context.Usuario_Recibe_Notificaciones.Add(urn);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "BookflixUser");
        }

        public async Task<IActionResult> crear(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }

            foreach (BookflixUser user in _users.Distinct())
            {
                Usuario_Recibe_Notificacion urn = new Usuario_Recibe_Notificacion
                {
                    BookflixUserId = user.Id,
                    NotificacionId = notificacion.Id
                };
                _context.Usuario_Recibe_Notificaciones.Add(urn);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "BookflixUser");
        }

        // GET: Notificacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }
            return View(notificacion);
        }

        // POST: Notificacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Contenido")] Notificacion notificacion)
        {
            if (id != notificacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificacionExists(notificacion.Id))
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
            return View(notificacion);
        }

        // GET: Notificacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            return View(notificacion);
        }

        // POST: Notificacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificacionExists(int id)
        {
            return _context.Notificaciones.Any(e => e.Id == id);
        }
    }
}
