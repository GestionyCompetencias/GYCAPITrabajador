using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYCEmpresa.Models
{
    public class InfoLicenciaMedica : LICENCIAMEDICA
    {
        public string DESC_TIPO_LICENCIA { get; set; }
        public string NOMBRE { get; set; }
        public string DESC_TIPO_MEDICO { get; set; }
        public DateTime FECINICIO { get; set; }
        public DateTime FECTERMINO { get; set; }
    }
}