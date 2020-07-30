using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Bookflix.Models.Validaciones;

namespace Bookflix.Models
{
    public class Statistics
    {
        public int NormalUsers { get; set; }
        public int PremiumUsers { get; set; }
        public List<Libro> Libros { get; set; }
        public Queue<int> CantidadDeLecturas {get; set;}


    
        //Metodos
        public double calcularPorcentajeNormal()
        {
            return this.NormalUsers * 100 / (this.NormalUsers + this.PremiumUsers);
        }

        public double calcularPorcentajePremium()
        {
            return this.PremiumUsers * 100 / (this.NormalUsers + this.PremiumUsers);
        }
    }
}

