//using RemuneracionWeb.Controllers;
using GYCEmpresa.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYCEmpresa.Models
{
    public class SubirArchivoModel
    {
        public String confirmacion { get; set; }

        public String confirmacion2 { get; set; }

        public Exception error { get; set; }

        public Exception error2 { get; set; }


        public void guardar(HttpPostedFileBase file, string rutEmpr)
        {

            try
            {
                ComplementoRemuneracion programita = new ComplementoRemuneracion();
                programita.Cargar_Archivo(file, rutEmpr);
                this.confirmacion2 = "Ya está en BD";
            }
            catch (Exception e)
            {
                this.error2 = e;
            }
        }
    }
}