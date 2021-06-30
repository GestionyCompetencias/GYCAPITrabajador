using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYCEmpresa.Models
{
    public class ListComprobantes
    {
        public int nroSolici { get; set; }
        public DateTime fechInivac { get; set; }
        public DateTime fechTervac { get; set; }
        public DateTime fechTransa { get; set; }
        public string nrorut { get; set; }
        public int correl { get; set; }
    }
}