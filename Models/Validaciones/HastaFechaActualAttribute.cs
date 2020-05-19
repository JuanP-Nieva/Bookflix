using System.ComponentModel.DataAnnotations;
using System;

namespace Bookflix.Models.Validaciones
{
    public class HastaFechaActualAttribute : DateAttribute
    {
        public override string GetErrorMessage() =>
            $"Tenes que haber nacido antes del {DateTime.Today}.";
        protected override bool errorDeFecha(DateTime fecha) => (fecha > DateTime.Today);
    }
}