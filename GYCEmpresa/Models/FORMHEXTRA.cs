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
    
    public partial class FORMHEXTRA
    {
        public int ID { get; set; }
        public string EMPRESA { get; set; }
        public string DESCRIPCION { get; set; }
        public string FORMULA { get; set; }
    
        public virtual EMPRESA EMPRESA1 { get; set; }
    }
}
