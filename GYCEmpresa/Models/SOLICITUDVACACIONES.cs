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
    
    public partial class SOLICITUDVACACIONES
    {
        public int ID { get; set; }
        public System.DateTime FECHA { get; set; }
        public string USUARIO { get; set; }
        public string TRABAJADOR { get; set; }
        public System.DateTime FINICIO { get; set; }
        public System.DateTime FTERMINO { get; set; }
        public Nullable<bool> AUTORIZAEMPRESA { get; set; }
        public Nullable<bool> AUTORIZATRABAJADOR { get; set; }
        public byte[] PDF { get; set; }
        public string EMPRESA { get; set; }
        public string DESGLOSE { get; set; }
        public int ESTADO { get; set; }
        public Nullable<bool> RECHAZADA { get; set; }
    
        public virtual EMPRESA EMPRESA1 { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}
