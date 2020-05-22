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
    [Authorize]
    public class NovedadController : Controller
    {
        private readonly BookflixDbContext _context;
        private readonly IWebHostEnvironment WebHostEnvironment;

        public NovedadController(BookflixDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            WebHostEnvironment = webHostEnvironment;

        }
        // GET: Novedad
        public async Task<IActionResult> Index()
        {
            return View(await _context.Novedades.ToListAsync());
        }

        // GET: Novedad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedad = await _context.Novedades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (novedad == null)
            {
                return NotFound();
            }

            return View(novedad);
        }

        // GET: Novedad/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Novedad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Imagen,Titulo")] NovedadViewModel vm)
        {
            
            if (ModelState.IsValid)
            {
                string stringFileName = this.UploadFile(vm.Imagen);
                var novedad = new Novedad

                {
                    Titulo = vm.Titulo,
                    Imagen = stringFileName,
                    Descripcion = vm.Descripcion
                };

                _context.Novedades.Add(novedad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        private string UploadFile(IFormFile image)
        {
            string fileName = null;
            if (image != null)
            {
                string uploadDir = Path.Combine(WebHostEnvironment.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        // GET: Novedad/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedad = await _context.Novedades.FindAsync(id);
            if (novedad == null)
            {
                return NotFound();
            }
            var vm = new EdicionNovedadViewModel
            {
                Id = novedad.Id,
                Titulo = novedad.Titulo,
                Imagen = null,
                Descripcion = novedad.Descripcion
            };

            return View(vm);
        }

        // POST: Novedad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Imagen,Titulo")] EdicionNovedadViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if(vm.Imagen == null)
            {
                var novedad = _context.Novedades.FirstOrDefault(v => v.Id == id );
                novedad.Titulo = vm.Titulo;
                novedad.Descripcion = vm.Descripcion;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string stringFileName = UploadFile(vm.Imagen);
                    var novedad = new Novedad
                    {
                        Id = vm.Id,
                        Descripcion = vm.Descripcion,
                        Imagen = stringFileName,
                        Titulo = vm.Titulo                        
                    };
                    _context.Update(novedad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NovedadExists(vm.Id))
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
            return View(vm);
        }


        // GET: Novedad/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedad = await _context.Novedades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (novedad == null)
            {
                return NotFound();
            }

            return View(novedad);
        }

        // POST: Novedad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var novedad = await _context.Novedades.FindAsync(id);
            _context.Novedades.Remove(novedad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NovedadExists(int id)
        {
            return _context.Novedades.Any(e => e.Id == id);
        }
    }
}


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using Bookflix.Data;
// using Bookflix.Models;

// namespace Bookflix.Controllers
// {
//     public class NovedadController : Controller
//     {
//         private readonly BookflixDbContext _context;

//         public NovedadController(BookflixDbContext context)
//         {
//             _context = context;
//         }

//         // GET: Novedad
//         public async Task<IActionResult> Index()
//         {
//             return View(await _context.Novedades.ToListAsync());
//         }

//         // GET: Novedad/Details/5
//         public async Task<IActionResult> Details(int? id)
//         {
//             if (id == null)
//             {
//                 return NotFound();
//             }

//             var novedad = await _context.Novedades
//                 .FirstOrDefaultAsync(m => m.Id == id);
//             if (novedad == null)
//             {
//                 return NotFound();
//             }

//             return View(novedad);
//         }

//         // GET: Novedad/Create
//         public IActionResult Create()
//         {
//             return View();
//         }

//         // POST: Novedad/Create
//         // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//         // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create([Bind("Id,Descripcion,Imagen,Titulo")] Novedad novedad)
//         {
//             if (ModelState.IsValid)
//             {
//                 _context.Add(novedad);
//                 await _context.SaveChangesAsync();
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(novedad);
//         }

//         // GET: Novedad/Edit/5
//         public async Task<IActionResult> Edit(int? id)
//         {
//             if (id == null)
//             {
//                 return NotFound();
//             }

//             var novedad = await _context.Novedades.FindAsync(id);
//             if (novedad == null)
//             {
//                 return NotFound();
//             }
//             return View(novedad);
//         }

//         // POST: Novedad/Edit/5
//         // To protect from overposting attacks, enable the specific properties you want to bind to, for 
//         // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Imagen,Titulo")] Novedad novedad)
//         {
//             if (id != novedad.Id)
//             {
//                 return NotFound();
//             }

//             if (ModelState.IsValid)
//             {
//                 try
//                 {
//                     _context.Update(novedad);
//                     await _context.SaveChangesAsync();
//                 }
//                 catch (DbUpdateConcurrencyException)
//                 {
//                     if (!NovedadExists(novedad.Id))
//                     {
//                         return NotFound();
//                     }
//                     else
//                     {
//                         throw;
//                     }
//                 }
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(novedad);
//         }

//         // GET: Novedad/Delete/5
//         public async Task<IActionResult> Delete(int? id)
//         {
//             if (id == null)
//             {
//                 return NotFound();
//             }

//             var novedad = await _context.Novedades
//                 .FirstOrDefaultAsync(m => m.Id == id);
//             if (novedad == null)
//             {
//                 return NotFound();
//             }

//             return View(novedad);
//         }

//         // POST: Novedad/Delete/5
//         [HttpPost, ActionName("Delete")]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> DeleteConfirmed(int id)
//         {
//             var novedad = await _context.Novedades.FindAsync(id);
//             _context.Novedades.Remove(novedad);
//             await _context.SaveChangesAsync();
//             return RedirectToAction(nameof(Index));
//         }

//         private bool NovedadExists(int id)
//         {
//             return _context.Novedades.Any(e => e.Id == id);
//         }
//     }
// }
