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
    
    public partial class ITEMACREDITACION
    {
        public int ID { get; set; }
        public int IDACREDITACION { get; set; }
        public string ITEM { get; set; }
        public int ESTADO { get; set; }
        public Nullable<int> ESTADOSISCONTROL { get; set; }
        public Nullable<System.DateTime> FECVENCIMIENTOSISCONTROL { get; set; }
        public System.DateTime FINICIO { get; set; }
        public Nullable<System.DateTime> FGESTION { get; set; }
        public Nullable<System.DateTime> FCIERRE { get; set; }
        public string OBSERVACION { get; set; }
    
        public virtual ACREDITACION ACREDITACION { get; set; }
    }
}
