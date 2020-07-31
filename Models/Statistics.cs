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
            if ((this.NormalUsers + this.PremiumUsers) != 0)
            {
                return this.NormalUsers * 100 / (this.NormalUsers + this.PremiumUsers);
            }
            return 0;
        }

        public double calcularPorcentajePremium()
        {
            if ((this.NormalUsers + this.PremiumUsers) != 0)
            {
                return this.PremiumUsers * 100 / (this.NormalUsers + this.PremiumUsers);
            }
            return 0;
        }
    }
}

