using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Bookflix.Data;
using Bookflix.Models;

namespace Bookflix.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class BookflixUserController : Controller
    {
        private readonly BookflixDbContext _context;

        public BookflixUserController(BookflixDbContext context)
        {
            _context = context;
        }

        // GET: BookflixUser
        public async Task<IActionResult> Index(string options, string searchString)
        {
            IQueryable<BookflixUser> bookflixDbContext;
            
            if(String.IsNullOrEmpty(searchString))
            {
                bookflixDbContext = _context.Users.AsQueryable();
            }
            else
            {
                if(options == "BuscarEmail")
                {
                    bookflixDbContext = _context.Users.Where(user => user.Email.Contains(searchString));
                }
                else
                    if(options == "BuscarNombre")
                    {
                        bookflixDbContext = _context.Users.Where(user => user.Nombre.Contains(searchString));
                    }
                    else
                        if(options == "BuscarApellido")
                        {
                            bookflixDbContext = _context.Users.Where(user => user.Apellido.Contains(searchString));
                        }
                        else
                            bookflixDbContext = _context.Users.Where(user => user.Dni.ToString().Contains(searchString));
            }
            return View("Index", await bookflixDbContext.ToListAsync());
        }

        // GET: BookflixUser/Details/5
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

        // GET: BookflixUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookflixUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Dni,FechaDeNacimiento,Habilitado,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] BookflixUser bookflixUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookflixUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookflixUser);
        }

        // GET: BookflixUser/Edit/5
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

        // POST: BookflixUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nombre,Apellido,Dni,FechaDeNacimiento,Habilitado,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] BookflixUser bookflixUser)
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

        // GET: BookflixUser/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookflixUser = await _context.Users
                .Include(user => user.Perfiles)
                .FirstOrDefaultAsync(user => user.Id == id);
            if (bookflixUser == null)
            {
                return NotFound();
            }

            //this.borrarPerfiles(bookflixUser);
            _context.Users.Remove(bookflixUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleHabilitar(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookflixUser = await _context.Users
                .FirstOrDefaultAsync(user => user.Id == id);
            if (bookflixUser == null)
            {
                return NotFound();
            }

            bookflixUser.Habilitado = !bookflixUser.Habilitado;
            _context.Update(bookflixUser);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // private void borrarPerfiles(BookflixUser bookflixUser)
        // {
        //     foreach (Perfil perfil in bookflixUser.Perfiles)
        //     {
        //         //do something?
        //     }
        // }



        // POST: BookflixUser/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(string id)
        // {
        //     var bookflixUser = await _context.Users.FindAsync(id);
        //     _context.Users.Remove(bookflixUser);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        private bool BookflixUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
