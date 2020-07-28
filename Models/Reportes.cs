using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Bookflix.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Bookflix.Models {
    public class Reportes {
        
        [Key]
        public int Id { get; set; }
        [Required]
        public int LibroId { get; set; }
        [Required]
        public int PerfilId { get; set; }
        [Required(ErrorMessage = "El Motivo obligatorio")]
        public string Motivo { get; set; }

        public Libro obtenerLibro()
        {
            using (var db = new BookflixDbContext())
            {
                return db.Libros.Include(l => l.Autor)
                                .Include(l => l.Genero)
                                .Include(l => l.Editorial)
                                .FirstOrDefault(c => c.Id == LibroId);                               
            }
        }
    }
}