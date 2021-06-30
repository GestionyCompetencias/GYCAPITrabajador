using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemuneracionWeb.Models
{
    public class LibroRemuneracion
    {
        private string rutTrab;
        private string nombreTrab;
        private int diasTrabajados;
        private string sueldoBase;
        private string hExtras;
        private string gratLegas;
        private string otroImpo;
        private string imponible;
        private string asigFami;
        private string otroNoImpo;
        private string noImpo;
        private string haberes;
        private string prevision;
        private string salud;
        private string impUnico;
        private string seguroCesa;
        private string otroDescLegal;
        private string descLegales;
        private string descVarios;
        private string descuentos;
        private string liquido;

        //CONSTRUCTORES
        public LibroRemuneracion()
        {
        }

        public LibroRemuneracion(string rutTrab, string nombreTrab, int diasTrabajados, string sueldoBase,
            string hExtras, string gratLegas, string otroImpo, string imponible, string asigFami, string otroNoImpo, string noImpo,
            string haberes, string prevision, string salud, string impUnico, string seguroCesa, string otroDescLegal, string descLegales,
            string descVarios, string descuentos, string liquido)
        {
            this.rutTrab = rutTrab;
            this.nombreTrab = nombreTrab;
            this.diasTrabajados = diasTrabajados;
            this.sueldoBase = sueldoBase;
            this.hExtras = hExtras;
            this.gratLegas = gratLegas;
            this.otroImpo = otroImpo;
            this.imponible = imponible;
            this.asigFami = asigFami;
            this.otroNoImpo = otroNoImpo;
            this.noImpo = noImpo;
            this.haberes = haberes;
            this.prevision = prevision;
            this.salud = salud;
            this.impUnico = impUnico;
            this.seguroCesa = seguroCesa;
            this.otroDescLegal = otroDescLegal;
            this.descLegales = descLegales;
            this.descVarios = descVarios;
            this.descuentos = descuentos;
            this.liquido = liquido;
        }

        //GET Y SET
        public string RutTrab { get => rutTrab; set => rutTrab = value; }
        public string NombreTrab { get => nombreTrab; set => nombreTrab = value; }
        public int DiasTrabajados { get => diasTrabajados; set => diasTrabajados = value; }
        public string SueldoBase { get => sueldoBase; set => sueldoBase = value; }
        public string HExtras { get => hExtras; set => hExtras = value; }
        public string GratLegas { get => gratLegas; set => gratLegas = value; }
        public string OtroImpo { get => otroImpo; set => otroImpo = value; }
        public string Imponible { get => imponible; set => imponible = value; }
        public string AsigFami { get => asigFami; set => asigFami = value; }
        public string OtroNoImpo { get => otroNoImpo; set => otroNoImpo = value; }
        public string NoImpo { get => noImpo; set => noImpo = value; }
        public string Haberes { get => haberes; set => haberes = value; }
        public string Prevision { get => prevision; set => prevision = value; }
        public string Salud { get => salud; set => salud = value; }
        public string ImpUnico { get => impUnico; set => impUnico = value; }
        public string SeguroCesa { get => seguroCesa; set => seguroCesa = value; }
        public string OtroDescLegal { get => otroDescLegal; set => otroDescLegal = value; }
        public string DescLegales { get => descLegales; set => descLegales = value; }
        public string DescVarios { get => descVarios; set => descVarios = value; }
        public string Descuentos { get => descuentos; set => descuentos = value; }
        public string Liquido { get => liquido; set => liquido = value; }
    }
 
}