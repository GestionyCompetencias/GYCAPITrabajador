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
    
    public partial class DIASCOMPENSADOS
    {
        public int ID { get; set; }
        public string TRABAJADOR { get; set; }
        public System.DateTime FECHAORIGENCOMPENSADO { get; set; }
        public double DIASCOMPENSADOS1 { get; set; }
        public System.DateTime FECHAVENCIMIENTO { get; set; }
        public string AUTORIZADOPOR { get; set; }
        public System.DateTime FECHAAUTORIZACION { get; set; }
    
        public virtual PERSONA PERSONA { get; set; }
    }
}
