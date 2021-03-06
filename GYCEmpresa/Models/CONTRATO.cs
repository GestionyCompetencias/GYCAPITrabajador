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
    
    public partial class CONTRATO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONTRATO()
        {
            this.DESCUENTOHABERCONTRATO = new HashSet<DESCUENTOHABERCONTRATO>();
            this.FINIQUITO = new HashSet<FINIQUITO>();
            this.VALIDACIONES = new HashSet<VALIDACIONES>();
            this.ANEXOS = new HashSet<ANEXOS>();
        }
    
        public int ID { get; set; }
        public string NUMCONTRATOEMPRESA { get; set; }
        public string PERSONA { get; set; }
        public string EMPRESA { get; set; }
        public System.DateTime FINICIO { get; set; }
        public System.DateTime FTERMNO { get; set; }
        public int TCONTRATO { get; set; }
        public int FAENA { get; set; }
        public string CARGO { get; set; }
        public string AREA { get; set; }
        public int JORNADA { get; set; }
        public int HORDINARIAS { get; set; }
        public int MBRUTO { get; set; }
        public string OBSERVACIONES { get; set; }
        public byte[] PDF { get; set; }
        public Nullable<bool> FIRMATRABAJADOR { get; set; }
        public Nullable<bool> FIRMAEMPRESA { get; set; }
        public string VALIDOPOR { get; set; }
        public Nullable<System.DateTime> FECHAVALIDO { get; set; }
        public Nullable<bool> RECHAZADO { get; set; }
        public Nullable<System.DateTime> FECHA_CREACION { get; set; }
    
        public virtual EMPRESA EMPRESA1 { get; set; }
        public virtual FAENA FAENA1 { get; set; }
        public virtual PERSONA PERSONA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESCUENTOHABERCONTRATO> DESCUENTOHABERCONTRATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINIQUITO> FINIQUITO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VALIDACIONES> VALIDACIONES { get; set; }
        public virtual JORNADA JORNADA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANEXOS> ANEXOS { get; set; }
    }
}
