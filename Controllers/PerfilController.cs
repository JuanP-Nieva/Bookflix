using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bookflix.Data;
using Bookflix.Models;
using Microsoft.AspNetCore.Authorization;

namespace Bookflix.Controllers
{
    public class PerfilController : Controller
    {
        private readonly BookflixDbContext _context;
        private readonly UserManager<BookflixUser> _userManager;

        public PerfilController(BookflixDbContext context, UserManager<BookflixUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Perfil
        public IActionResult Index()
        {
            if(User.IsInRole("Administrador"))
            {
                return RedirectToAction("Index","Libro");
            }
            var perfiles = _context.Perfiles.Where(p => p.Usuario.Id == _userManager.GetUserId(User));
            return View(perfiles);
        }

          public IActionResult AdministrarPerfil()
        {
            var perfiles = _context.Perfiles.Where(p => p.Usuario.Id == _userManager.GetUserId(User));
            
            if(User.IsInRole("Normal")){
                ViewBag.Valido = (perfiles.Count() < 2);
            }else{
                ViewBag.Valido = (perfiles.Count() < 4);                
            }

            return View(perfiles);
        }


        public async Task<IActionResult> IngresarPerfil(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var perfil = await _context.Perfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if(perfil == null)
            {
                return NotFound();
            }
            return View(perfil);
        }

        // GET: Perfil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (perfil == null)
            {
                return NotFound();
            }

            return View(perfil);
        }

        // GET: Perfil/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Perfil/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Perfil perfil)
        {
            if (ModelState.IsValid && !perfilUnico(perfil))
            {
                perfil.Usuario = await _userManager.GetUserAsync(HttpContext.User);
                _context.Add(perfil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(perfil);
        }

        public IActionResult EnConstruccion()
        {
            return View();
        }

        // GET: Perfil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfiles.FindAsync(id);
            if (perfil == null)
            {
                return NotFound();
            }
            return View(perfil);
        }

        // POST: Perfil/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Perfil perfil)
        {
            if (id != perfil.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && !perfilUnico(perfil))
            {
                try
                {
                    _context.Update(perfil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerfilExists(perfil.Id))
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
            return View(perfil);
        }

        // GET: Perfil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfil = await _context.Perfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (perfil == null)
            {
                return NotFound();
            }

            return View(perfil);
        }

        // POST: Perfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var perfil = await _context.Perfiles.FindAsync(id);
            _context.Perfiles.Remove(perfil);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PerfilExists(int id)
        {
            return _context.Perfiles.Any(e => e.Id == id);
        }

        //Verifica que el nombre de perfil sea unico
        private bool perfilUnico(Perfil perfil)
        {
            var perfilesUsuario = _context.Perfiles.Where(p => p.Usuario.Id == _userManager.GetUserId(User));   
            return perfilesUsuario.Any(pa => pa.Nombre.Equals(perfil.Nombre));
        }
    }
}
