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
    
    public partial class ABASTECIMIENTOAGUA
    {
        public int ID { get; set; }
        public System.DateTime FECHA { get; set; }
        public string USUARIO { get; set; }
        public int FAENA { get; set; }
        public System.DateTime FECHAABASTECIMIENTO { get; set; }
        public int UNIIDADES { get; set; }
        public string OBSERVACION { get; set; }
        public Nullable<int> ESTADO { get; set; }
    
        public virtual FAENA FAENA1 { get; set; }
    }
}
