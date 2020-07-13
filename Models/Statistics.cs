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
    [NotMapped]
    public class Statistics
    {
        [Key]
        public int Id { get; set; }


    }
}

