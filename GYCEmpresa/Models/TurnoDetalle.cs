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
    
    public partial class TurnoDetalle
    {
        public int IdTurnoDetalle { get; set; }
        public int IdTurno { get; set; }
        public Nullable<short> Dia { get; set; }
        public Nullable<System.TimeSpan> HoraInicio { get; set; }
        public Nullable<System.TimeSpan> HoraTermino { get; set; }
        public Nullable<System.TimeSpan> TotalHora { get; set; }
        public Nullable<System.TimeSpan> HorasDescanso { get; set; }
        public Nullable<System.TimeSpan> Computo { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<bool> EsVigente { get; set; }
    
        public virtual Turno Turno { get; set; }
    }
}
