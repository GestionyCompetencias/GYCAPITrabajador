using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GYCEmpresa.Models;
using GYCEmpresa.App_Start;

namespace GYCEmpresa.Controllers
{
    public class NotificacionesController : Controller
    {
        dbgycEntities3 db = new dbgycEntities3();
        dbgyc2DataContext db2 = new dbgyc2DataContext();
        Funciones f = new Funciones();

        // GET: Notificaciones
        public ActionResult Notificaciones(string rut)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            var notificacion = new List<GYCEmpresa.Models.NOTIFICACION>();

            if (System.Web.HttpContext.Current.Session["sessionTrabajador"] == "N")
            {
                notificacion = db.NOTIFICACION.Where(item => item.EMPRESA == empresa && item.VISTO == false && item.NOTIFEMPRESA == true).ToList();
                return View(notificacion);
            }
            else
            {
                notificacion = db.NOTIFICACION.Where(item => item.EMPRESA == empresa && item.VISTO == false && item.NOTIFTRABAJADOR == true && item.TRABAJADOR == usuario).ToList();
                return View(notificacion);

            }
        }

        public JsonResult DetalleNotificacion(int ID)
        {
            var Detalle = db.NOTIFICACION.Where(x => x.ID == ID).Select(x => new { FECHA = x.FECHA.ToString(), TIPO = x.TIPO, OBSERVACION = x.OBSERVACION }).SingleOrDefault();
            return Json(Detalle, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult LeeNotificacion(int idnotificacion)
        {
            try
            {
                List<NOTIFICACION> Notificacion = db.NOTIFICACION.Where(v => v.ID == idnotificacion).ToList(); ;
                Notificacion.ForEach(item => item.VISTO = true);
                db.SaveChanges();

                return Json(new
                {
                    Mensaje = "ok"
                });
            }
            catch (Exception e2)
            {
                return Json(new
                {
                    Mensaje = e2.Message
                });
            }
        }
        public virtual JsonResult LeeTodoNotificacion()
        {
            try
            {
                string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
                var notificacion = new List<GYCEmpresa.Models.NOTIFICACION>();


                if (System.Web.HttpContext.Current.Session["sessionTrabajador"] == "N")
                {
                    db2.NOTIFICACIONES_TODOS_LEIDAS_EMPRESA(empresa);
                    db2.SubmitChanges();
                }
                else
                {
                    db.Database.ExecuteSqlCommand("UPDATE NOTIFICACION SET [VISTO] = 1 WHERE [NOTIFTRABAJADOR] = true AND [USUARIO] = " + usuario );
                }
                return Json(new
                {
                    Mensaje = "ok"
                });

            }
            catch (Exception e2)
            {
                return Json(new
                {
                    Mensaje = e2.Message
                });
            }
        }
    }
}
