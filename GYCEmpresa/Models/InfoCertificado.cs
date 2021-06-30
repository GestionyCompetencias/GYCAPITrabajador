using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYCEmpresa.Models
{
    public class InfoCertificado
    {
        public string tipoDocumento { get; set; }
        public string fechaDocumento { get; set; }
        public string nombreTrabajador { get; set; }
        public string rutTrabajador { get; set; }
        public string nombreEmpresa { get; set; }
        public string respresentanteEmpresa { get; set; }
        public string hashTrabajador { get; set; }
        public string hashEmpresa { get; set; }
        public string hashGYCSol { get; set; }
        public string hashDocumento { get; set; }
        public string hashBloque { get; set; }
    }
}