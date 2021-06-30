using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYCEmpresa.Models
{
    public class ListPapeleta
    {
        public String nombreEmpresa { get; set; }
        public String faena { get; set; }
        public string tipo { get; set; }
        public string textTipo { get; set; }
        public string valueTipo { get; set; }
        public String nombre { get; set; }
        public string rutTra { get; set; }
        public string dvrut { get; set; }
        public String cargoLaboral { get; set; }
        public DateTime fecIngreso { get; set; }
        public String catLaboral { get; set; }
        public List<remeresu> haberes { get; set; }
        public remeresu descLegal { get; set; }
        public remeresu descuentos { get; set; }      
        public int totalGanado { get; set; }
        public int totalDesc { get; set; }
        public int totalCobrar { get; set; }
        public DateTime fechaPago { get; set; }
    }
}