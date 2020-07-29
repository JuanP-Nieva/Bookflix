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

        private static List<BookflixUser> usuariosActuales;

        public BookflixUserController(BookflixDbContext context)
        {
            _context = context;
        }

        // GET: BookflixUser
        public IActionResult Index(string options, string searchString)
        {
            IQueryable<BookflixUser> bookflixDbContext;
            
            if(!String.IsNullOrEmpty(searchString))
            {
                switch (options)
                {
                    case "BuscarDni":
                        bookflixDbContext = _context.Users.Where(user => user.Dni.ToString().Contains(searchString));
                        break;
                    case "BuscarApellido":
                        bookflixDbContext = _context.Users.Where(user => user.Apellido.Contains(searchString));
                        break;
                    case "BuscarNombre":
                        bookflixDbContext = _context.Users.Where(user => user.Nombre.Contains(searchString));
                        break;
                    default:
                        bookflixDbContext = _context.Users.Where(user => user.Email.Contains(searchString));
                        break;
                }
            }
            else
            {
                bookflixDbContext = _context.Users;
            }

            usuariosActuales = bookflixDbContext.ToList();

            return View("Index", usuariosActuales);
        }

        public IActionResult OrdenarLista(string options)
        {
            switch (options)
            {
                case "OrdenarDni":
                    usuariosActuales.Sort(delegate (BookflixUser x, BookflixUser y)
                    {
                        return x.Dni.CompareTo(y.Dni);
                    });
                    break;
                case "OrdenarApellido":
                    usuariosActuales.Sort(delegate (BookflixUser x, BookflixUser y)
                    {
                        if (x.Apellido == null && y.Apellido == null) return 0;
                        else if (x.Apellido == null) return -1;
                        else if (y.Apellido == null) return 1;
                        else return x.Apellido.CompareTo(y.Apellido);
                    });
                    break;
                case "OrdenarNombre":
                    usuariosActuales.Sort(delegate (BookflixUser x, BookflixUser y)
                    {
                        if (x.Nombre == null && y.Nombre == null) return 0;
                        else if (x.Nombre == null) return -1;
                        else if (y.Nombre == null) return 1;
                        else return x.Nombre.CompareTo(y.Nombre);
                    });
                    break;
                default:
                    usuariosActuales.Sort(delegate (BookflixUser x, BookflixUser y)
                    {
                        if (x.Email == null && y.Email == null) return 0;
                        else if (x.Email == null) return -1;
                        else if (y.Email == null) return 1;
                        else return x.Email.CompareTo(y.Email);
                    });
                    break;
            }
            return View("Index", usuariosActuales);
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
        //         _context.Perfiles.Remove(perfil);
        //     }
        //     await _context.SaveChangesAsync();
        // }

        private bool BookflixUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
