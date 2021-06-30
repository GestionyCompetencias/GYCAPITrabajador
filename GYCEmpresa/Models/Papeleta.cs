using GYCEmpresa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemuneracionWeb.Models
{
    public class Papeleta
    {
        private remepers trabajador;
        private List<remeresu> haberes;
        private List<remeresu> descuentos;
        private List<remeresu> descuentosLegales;
        private List<remeresu> resultados;

        public Papeleta()
        {
            this.haberes = new List<remeresu>();
            this.descuentos = new List<remeresu>();
            this.descuentosLegales = new List<remeresu>();
            this.resultados = new List<remeresu>();
        }

        public Papeleta(remepers trabajador, List<remeresu> haberes, List<remeresu> descuentos, List<remeresu> descuentosLegales, List<remeresu> resultados)
        {
            this.trabajador = trabajador;
            this.haberes = haberes;
            this.descuentos = descuentos;
            this.descuentosLegales = descuentosLegales;
            this.resultados = resultados;
        }

        public remepers Trabajador { get => trabajador; set => trabajador = value; }
        public List<remeresu> Haberes { get => haberes; set => haberes = value; }
        public List<remeresu> Descuentos { get => descuentos; set => descuentos = value; }
        public List<remeresu> DescuentosLegales { get => descuentosLegales; set => descuentosLegales = value; }
        public List<remeresu> Resultados { get => resultados; set => resultados = value; }
    }
}