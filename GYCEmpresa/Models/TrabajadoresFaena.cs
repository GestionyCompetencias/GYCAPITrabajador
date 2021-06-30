using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYCEmpresa.Models
{
    public class TrabajadoresFaena
    {
        public string Rut { get; set; }
        public string Nombre { get; set; }
        public string TipoContrato { get; set; }
        public string NumContratoEmpresa { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public int IdFaena { get; set; }
        public string Faena { get; set; }
        public string Cargo { get; set; }

    }
}