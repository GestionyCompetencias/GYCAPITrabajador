using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYCEmpresa.Models
{
    public class ListCuentaCorriente
    {
        public string nrt_ruttr;
        public String periodo { get; set; }
        public string diastot { get; set; }
        public string finivac{ get; set; }
        public string ftervac { get; set; }
        public string diasusa { get; set; }
        public string diasacu { get; set; }
        public string fcompe { get; set; }
        public string diascom { get; set; }
        public string diasocu { get; set; }
        public string diaspen { get; set; }
        public string tipouso { get; set; }
        public string tiporden { get; set; }
    }
    public class periodoscalc
    {
        public string nrt_ruttr { get; set; }
        public string nro_periodo { get; set; }
        public DateTime fec_inivac { get; set; }
        public DateTime fec_finvac { get; set; }
        public int dias_habil { get; set; }
        public int dias_corri { get; set; }
        public int año_inicio { get; set; }
        public int año_termino { get; set; }
        public string tip_uso { get; set; }
        public int nro_solici { get; set; }
        public int dias_legal { get; set; }
        public int dias_progr { get; set; }
        public int dias_contr { get; set; }
        public int dias_admin { get; set; }
        public int dias_faena { get; set; }
        public int dias_especi { get; set; }
        public int dias_otros { get; set; }
    }
}