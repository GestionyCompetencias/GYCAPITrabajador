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
    
    public partial class LICENCIAMEDICA
    {
        public int ID { get; set; }
        public string CODIGO_LICENCIA { get; set; }
        public string TRABAJADOR { get; set; }
        public System.DateTime FINICIO { get; set; }
        public System.DateTime FTERMINO { get; set; }
        public Nullable<int> DIAS { get; set; }
        public int TIPO_LICENCIA { get; set; }
        public string COMENTARIO { get; set; }
        public Nullable<int> TIPO_MEDICO { get; set; }
        public byte[] PDF { get; set; }
    
        public virtual TIPOLICENCIASMEDICAS TIPOLICENCIASMEDICAS { get; set; }
        public virtual TIPOMEDICO TIPOMEDICO { get; set; }
        public virtual PERSONA PERSONA { get; set; }
    }
}