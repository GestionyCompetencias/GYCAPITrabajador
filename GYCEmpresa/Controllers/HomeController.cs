﻿using GYCEmpresa.App_Start;
using GYCEmpresa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Http;

namespace GYCEmpresa.Controllers
{
    public class HomeController : Controller
    {
        dbgycEntities3 db = new dbgycEntities3();
        dbgyc2DataContext db2 = new dbgyc2DataContext();
        Funciones f = new Funciones();
        private APITrabajadorController APITrabajador = new APITrabajadorController();

            public virtual JsonResult Index(solpersonal entrada)
        {
            string RUT = entrada.rut;

            if (RUT == null) return Json(new
            {
                mensaje="Falta parámetro"
            }, JsonRequestBehavior.AllowGet);


            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            JsonResult trabajador = APITrabajador.ExistePersonaDetalle(RUT);
            //var trabajador = APITrabajador.ExistePersonaDetalle(RUT);
            return Json(new
            {
                trabajador
            }, JsonRequestBehavior.AllowGet);


            //return View();

        }
    }
}
