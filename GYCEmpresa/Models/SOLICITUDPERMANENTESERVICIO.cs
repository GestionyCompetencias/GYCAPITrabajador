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
    
    public partial class SOLICITUDPERMANENTESERVICIO
    {
        public int ID { get; set; }
        public System.DateTime FECHA { get; set; }
        public string EMPRESA { get; set; }
        public int SERVICIO { get; set; }
        public Nullable<bool> PERMANENTE { get; set; }
        public Nullable<System.DateTime> FINICIO { get; set; }
        public Nullable<System.DateTime> FTERMINO { get; set; }
        public Nullable<int> DOTACION { get; set; }
        public int ESTADO { get; set; }
        public string OBSERVACION { get; set; }
        public string USUARIO { get; set; }
    
        public virtual EMPRESA EMPRESA1 { get; set; }
        public virtual SERVICIO SERVICIO1 { get; set; }
    }
}
