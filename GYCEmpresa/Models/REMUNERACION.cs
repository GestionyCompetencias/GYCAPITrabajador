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
    
    public partial class REMUNERACION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REMUNERACION()
        {
            this.DESCUENTOSHABERES = new HashSet<DESCUENTOSHABERES>();
        }
    
        public int ID { get; set; }
        public int CONTRATO { get; set; }
        public string PERSONA { get; set; }
        public System.DateTime FECHA { get; set; }
        public double MBRUTO { get; set; }
        public double DTRABAJADOS { get; set; }
        public Nullable<double> HEXTRAS { get; set; }
        public Nullable<double> IMPUNICO { get; set; }
        public int PREVISION { get; set; }
        public double MPREVISION { get; set; }
        public int SALUD { get; set; }
        public double MSALUD { get; set; }
        public Nullable<double> MSALUDADICIONAL { get; set; }
        public double MAFC { get; set; }
        public Nullable<int> PREVISION_APV { get; set; }
        public Nullable<double> MPREVISION_APV { get; set; }
        public Nullable<int> PREVISION_AHORRO { get; set; }
        public Nullable<double> MPREVISION_AHORRO { get; set; }
        public Nullable<int> PREVISION_OTRO { get; set; }
        public Nullable<double> MPREVISION_OTRO { get; set; }
        public string PDF { get; set; }
        public string VALIDOPOR { get; set; }
        public Nullable<System.DateTime> FECHAVALIDO { get; set; }
    
        public virtual PREVISION PREVISION1 { get; set; }
        public virtual PREVISION PREVISION2 { get; set; }
        public virtual PREVISION PREVISION3 { get; set; }
        public virtual PREVISION PREVISION4 { get; set; }
        public virtual SALUD SALUD1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESCUENTOSHABERES> DESCUENTOSHABERES { get; set; }
        public virtual PERSONA PERSONA1 { get; set; }
    }
}
