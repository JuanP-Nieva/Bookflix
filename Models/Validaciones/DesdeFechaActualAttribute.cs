using System.ComponentModel.DataAnnotations;
using System;

namespace Bookflix.Models.Validaciones
{
    public class DesdeFechaActualAttribute : DateAttribute
    {
        public override string GetErrorMessage() =>
            $"La tarjeta tiene que expirar luego del {DateTime.Today}.";
        protected override bool errorDeFecha(DateTime fecha) => (fecha < DateTime.Today);
    }
}