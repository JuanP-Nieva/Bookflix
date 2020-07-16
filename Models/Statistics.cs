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
    }
}

