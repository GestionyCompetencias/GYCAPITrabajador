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
    
    public partial class GRUPOFAMILIAR
    {
        public int ID { get; set; }
        public string TRABAJADOR { get; set; }
        public string NOMBRE { get; set; }
        public string APATERNO { get; set; }
        public string AMATERNO { get; set; }
        public Nullable<System.DateTime> FNACIMIENTO { get; set; }
        public string SEXO { get; set; }
        public string PARENTEZCO { get; set; }
        public Nullable<int> ECIVIL { get; set; }
        public Nullable<int> NACIONALIDAD { get; set; }
        public Nullable<bool> PUEBLOINDIGENA { get; set; }
        public string DESCPUEBLOINDIGENA { get; set; }
        public string NIVELESTUDIO { get; set; }
        public Nullable<bool> BENEFICIARIOSUBSIDIO { get; set; }
        public string DESCSUBSIDIO { get; set; }
        public Nullable<bool> TRABAJAREMUNERADO { get; set; }
        public Nullable<int> INGRESOPROMEDIO { get; set; }
    
        public virtual PERSONA PERSONA { get; set; }
    }
}
