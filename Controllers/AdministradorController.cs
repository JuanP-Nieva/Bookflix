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

namespace Bookflix.Controllers
{
    [Authorize(Roles="Administrador")]
    public class AdministradorController : Controller
    {
        private readonly BookflixDbContext _context;

        
        public AdministradorController(BookflixDbContext context)
        {
            _context = context;
        }


        // GET: Administrador
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }


        // GET: Administrador/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookflixUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookflixUser == null)
            {
                return NotFound();
            }

            return View(bookflixUser);
        }

        // GET: Administrador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Dni,FechaDeNacimiento,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] BookflixUser bookflixUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookflixUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookflixUser);
        }

        // GET: Administrador/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookflixUser = await _context.Users.FindAsync(id);
            if (bookflixUser == null)
            {
                return NotFound();
            }
            return View(bookflixUser);
        }

        // POST: Administrador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nombre,Apellido,Dni,FechaDeNacimiento,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] BookflixUser bookflixUser)
        {
            if (id != bookflixUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookflixUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookflixUserExists(bookflixUser.Id))
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
            return View(bookflixUser);
        }

        // GET: Administrador/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookflixUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookflixUser == null)
            {
                return NotFound();
            }

            return View(bookflixUser);
        }

        // POST: Administrador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bookflixUser = await _context.Users.FindAsync(id);
            _context.Users.Remove(bookflixUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookflixUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
