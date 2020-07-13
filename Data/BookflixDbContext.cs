using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bookflix.Models;

namespace Bookflix.Data
{
    public class BookflixDbContext : IdentityDbContext<BookflixUser>
    {
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Editorial> Editoriales { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Novedad> Novedades { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Tarjeta> Tarjetas { get; set; }        
        public DbSet<Reportes> Reportes { get; set;}
        public DbSet<Perfil_Favea_Libro> Perfil_Favea_Libros { get; set; }   
        public DbSet<Perfil_Comenta_Libro> Perfil_Comenta_Libros { get; set; }
        public DbSet<Perfil_Lee_Libro> Perfil_Lee_Libros { get; set; }
        public DbSet<Perfil_Puntua_Libro> Perfil_Puntua_Libros { get; set; }

        public DbSet<Capitulo> Capitulos { get; set; }

        public DbSet<Trailer> Trailers { get; set; }

        public BookflixDbContext(DbContextOptions<BookflixDbContext> options)
            : base(options)
        {
        }

        public BookflixDbContext()
            : base()
        {
        } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-LV6QBJ1;Database=Bookflix;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Perfil_Comenta_Libro> ().HasKey (pl => new { pl.LibroId , pl.NumeroComentario});
            modelBuilder.Entity<Perfil_Favea_Libro> ().HasKey (pl => new { pl.PerfilId, pl.LibroId });
            modelBuilder.Entity<Perfil_Lee_Libro> ().HasKey (pl => new { pl.PerfilId, pl.LibroId });
            modelBuilder.Entity<Perfil_Puntua_Libro> ().HasKey (pl => new { pl.PerfilId, pl.LibroId });
        }
    }
}
