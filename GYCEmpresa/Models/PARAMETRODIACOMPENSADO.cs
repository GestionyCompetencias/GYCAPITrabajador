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
    
    public partial class PARAMETRODIACOMPENSADO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PARAMETRODIACOMPENSADO()
        {
            this.EMPRESA = new HashSet<EMPRESA>();
        }
    
        public int ID { get; set; }
        public int MAX_DOMINGOS_TRAB_MES { get; set; }
        public double FACTOR_DIACOMPENSADO { get; set; }
        public double MONTO_DIACOMPENSADO { get; set; }
        public int DIAS_PLAZO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA> EMPRESA { get; set; }
    }
}