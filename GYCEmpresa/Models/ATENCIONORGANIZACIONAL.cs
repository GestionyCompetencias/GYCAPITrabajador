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
    
    public partial class ATENCIONORGANIZACIONAL
    {
        public int ID { get; set; }
        public string TRABAJADOR { get; set; }
        public System.DateTime FECHAATENCION { get; set; }
        public int MATERIA { get; set; }
        public string CONSULTA { get; set; }
        public string RESOLUCION { get; set; }
        public Nullable<int> ESTADO { get; set; }
    
        public virtual DECMATERIA DECMATERIA { get; set; }
        public virtual PERSONA PERSONA { get; set; }
    }
}