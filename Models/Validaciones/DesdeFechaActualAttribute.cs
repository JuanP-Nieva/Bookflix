using System.ComponentModel.DataAnnotations;
using System;

namespace Bookflix.Models.Validaciones
{
    public class DesdeFechaActualAttribute : DateAttribute
    {
        public override string GetErrorMessage() =>
            $"La fecha de expiracion debe ser luego del {DateTime.Today.Date}.";
        protected override bool errorDeFecha(DateTime fecha) => (fecha < DateTime.Today);
    }
}