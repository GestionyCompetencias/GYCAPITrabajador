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
    
    public partial class SERVICIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SERVICIO()
        {
            this.SOLICITUDSERVICIO = new HashSet<SOLICITUDSERVICIO>();
            this.SOLICITUDPERMANENTESERVICIO = new HashSet<SOLICITUDPERMANENTESERVICIO>();
            this.SERVICIOCONTRATADO = new HashSet<SERVICIOCONTRATADO>();
            this.PERMISO_V2 = new HashSet<PERMISO_V2>();
        }
    
        public int ID { get; set; }
        public string NOMBRE { get; set; }
        public string COBRO { get; set; }
        public string CODIGO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITUDSERVICIO> SOLICITUDSERVICIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOLICITUDPERMANENTESERVICIO> SOLICITUDPERMANENTESERVICIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SERVICIOCONTRATADO> SERVICIOCONTRATADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERMISO_V2> PERMISO_V2 { get; set; }
    }
}