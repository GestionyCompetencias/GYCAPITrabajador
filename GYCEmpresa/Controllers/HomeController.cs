using GYCEmpresa.App_Start;
using GYCEmpresa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace GYCEmpresa.Controllers
{
    public class HomeController : Controller
    {
        dbgycEntities3 db = new dbgycEntities3();
        dbgyc2DataContext db2 = new dbgyc2DataContext();
        Funciones f = new Funciones();
        private APITrabajadorController APITrabajador = new APITrabajadorController();

        public ActionResult Index()
        {
            JsonResult trabajador = APITrabajador.ExistePersonaDetalle("69393527");
            return Json(new
            {
                trabajador
            }, JsonRequestBehavior.AllowGet);


            //return View();

        }
    }


}