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
    
    public partial class CARGO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CARGO()
        {
            this.MANOOBRADIRECTA = new HashSet<MANOOBRADIRECTA>();
            this.CONTRATADIRECTA = new HashSet<CONTRATADIRECTA>();
        }
    
        public int ID { get; set; }
        public string DESCRIPCION { get; set; }
        public Nullable<int> HOMOLOGADO { get; set; }
        public string EMPRESA { get; set; }
        public string TIPO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MANOOBRADIRECTA> MANOOBRADIRECTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRATADIRECTA> CONTRATADIRECTA { get; set; }
    }
}
