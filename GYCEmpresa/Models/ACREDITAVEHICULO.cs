//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GYCEmpresa.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ACREDITAVEHICULO
    {
        public int ID { get; set; }
        public System.DateTime FECHA { get; set; }
        public string USUARIO { get; set; }
        public string PATENTE { get; set; }
        public string OBSERVACIONES { get; set; }
        public byte[] PDFCIRCULACION { get; set; }
        public byte[] PDFRTECNICA { get; set; }
        public byte[] PDFSEGURO { get; set; }
        public int FAENA { get; set; }
        public Nullable<int> ESTADO { get; set; }
    }
}
