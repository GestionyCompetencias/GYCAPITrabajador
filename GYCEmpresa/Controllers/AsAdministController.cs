using GYCEmpresa.App_Start;
using GYCEmpresa.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GYCEmpresa.Controllers
{
    public class AsAdministController : Controller
    {
        dbgycEntities3 db = new dbgycEntities3();
        dbgyc2DataContext db2 = new dbgyc2DataContext();
        // GET: AsAdminist

        Funciones f = new Funciones();

        string paso = string.Empty;

        public ActionResult Index()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            return View();
        }
        public ActionResult IngresoTicket()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            return View();
        }
        //CONTRATACION SERVICIO POR TRABAJADOR 1 **********************************************
        //clase*********************************
        public class Autorizaciones
        {
            public string Rut { get; set; }
            public string Acreditacion { get; set; }
            public string AcreditacionAnexo { get; set; }
            public string ConfAnexo { get; set; }
            public string GestionODI { get; set; }
            public string EvaluacionODI { get; set; }
        }
        //registro en bd*********************************
        public void RegistraSolicitud(string faena, string Trabajador, int Servicio)
        {


            int intFaena = Convert.ToInt32(faena);
            var empresa = (from FAENA in db.FAENA
                           where
                             FAENA.ID == intFaena
                           select new
                           {
                               FAENA.EMPRESA
                           }).SingleOrDefault();

            SOLICITUDSERVICIO nuevaSolicitud = new SOLICITUDSERVICIO()
            {
                FECHA = DateTime.Now,
                EMPRESA = empresa.EMPRESA,
                SERVICIO = Servicio,
                TRABAJADOR = Trabajador,
                CERRADO = false,
                OBSERVACION = ""
            };
            db.SOLICITUDSERVICIO.Add(nuevaSolicitud);
            db.SaveChanges();

            var IdRegistro = (from SOLICITUDSERVICIO in
                                          (from SOLICITUDSERVICIO in db.SOLICITUDSERVICIO
                                           select new
                                           {
                                               SOLICITUDSERVICIO.ID,
                                               Dummy = "x"
                                           })
                              group SOLICITUDSERVICIO by new { SOLICITUDSERVICIO.Dummy } into g
                              select new
                              {
                                  Column1 = (int?)g.Max(p => p.ID)
                              }).SingleOrDefault();
            string NumServicio = "ServicioNª - " + Servicio;
            //Notifica(NumServicio, "", empresa.EMPRESA, "SOLICITUDSERVICIO", Convert.ToInt32(IdRegistro.Column1.ToString()));
        }

        public ActionResult AutorizacionesXTrabajador1(string empresa)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            return View();
        }
        [HttpPost]
        public ActionResult AutorizacionesXTrabajador1(string empresa, string faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            return RedirectToAction("AuthXTrab1Tabla", "AsAdminist", new { faena = faena });

        }

        public ActionResult AuthXTrab1Tabla(string faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            var Trab = (from CONTRATO in db.CONTRATO
                        where
                          CONTRATO.FAENA == 1
                        select new
                        {
                            CONTRATO.PERSONA1.RUT,
                            CONTRATO.PERSONA1.APATERNO,
                            CONTRATO.PERSONA1.AMATERNO,
                            CONTRATO.PERSONA1.NOMBRE
                        }).ToList();

            var TrabajadoresARRAY = Trab.ToArray();
            ViewBag.Trabajadores = TrabajadoresARRAY;
            int faenaEntero = Convert.ToInt32(faena);
            List<TRABAJADORCONTRATO> Trabajadores = db.TRABAJADORCONTRATO.Where(tc => tc.FAENA == faenaEntero).ToList();
            return View(Trabajadores);

        }
        [HttpPost]
        public ActionResult AuthXTrab1Tabla(IList<Autorizaciones> Autorizaciones, string faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            try
            {
                foreach (var aut in Autorizaciones)
                {
                    string[] valor1 = aut.Acreditacion.Split('-');
                    if (valor1[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor1[0]));
                    }
                    string[] valor2 = aut.AcreditacionAnexo.Split('-');
                    if (valor2[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor2[0]));
                    }
                    string[] valor3 = aut.ConfAnexo.Split('-');
                    if (valor3[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor3[0]));
                    }
                    string[] valor4 = aut.GestionODI.Split('-');
                    if (valor4[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor4[0]));
                    }

                }
            }
            catch (Exception)
            {

            }


            return RedirectToAction("AuthXTrab1Tabla", "AsAdminist", new { faena });
        }


        // CONTRATACION SERVICIO POR TRABAJADOR 2 ********************************************
        public ActionResult AutorizacionesXTrabajador2(string empresa)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            return View();
        }
        [HttpPost]
        public ActionResult AutorizacionesXTrabajador2(string empresa, string faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            return RedirectToAction("AuthXTrab2Tabla", "AsAdminist", new { faena = faena });

        }

        public ActionResult AuthXTrab2Tabla(string faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            int intFaena = Convert.ToInt32(faena);
            var Trab = (from CONTRATO in db.CONTRATO
                        where
                          CONTRATO.FAENA == intFaena
                        select new
                        {
                            CONTRATO.PERSONA1.RUT,
                            CONTRATO.PERSONA1.APATERNO,
                            CONTRATO.PERSONA1.AMATERNO,
                            CONTRATO.PERSONA1.NOMBRE
                        }).ToList();

            var TrabajadoresARRAY = Trab.ToArray();
            ViewBag.Trabajadores = TrabajadoresARRAY;
            int faenaEntero = Convert.ToInt32(faena);

            List<TRABAJADORCONTRATO> Trabajadores = db.TRABAJADORCONTRATO.Where(tc => tc.FAENA == faenaEntero).ToList();
            return View(Trabajadores);

        }
        [HttpPost]
        public ActionResult AuthXTrab2Tabla(IList<Autorizaciones> Autorizaciones, string faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            try
            {
                foreach (var aut in Autorizaciones)
                {
                    string[] valor1 = aut.Acreditacion.Split('-');
                    if (valor1[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor1[0]));
                    }
                    string[] valor2 = aut.AcreditacionAnexo.Split('-');
                    if (valor2[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor2[0]));
                    }
                    string[] valor3 = aut.ConfAnexo.Split('-');
                    if (valor3[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor3[0]));
                    }
                    string[] valor4 = aut.GestionODI.Split('-');
                    if (valor4[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor4[0]));
                    }
                    string[] valor5 = aut.EvaluacionODI.Split('-');
                    if (valor5[1] == "1")
                    {
                        RegistraSolicitud(faena, aut.Rut, Convert.ToInt32(valor5[0]));
                    }

                }
            }
            catch (Exception)
            {

            }

            return RedirectToAction("AuthXTrab2Tabla", "AsAdminist", new { faena });
        }

        //CONTRATACION DE SERVICIOS PERMANENTES ****************************************
        //clase********************************
        public class Servicios
        {
            public int Contratar { get; set; }
            public string Codigo { get; set; }
            public string Servicio { get; set; }
            public int Permanente { get; set; }
            public string Desde { get; set; }
            public string Hasta { get; set; }
            public int Dotacion { get; set; }
            public string Empresa { get; set; }
        }
        //registro en bd**********************
        public void RegistroSolicitudPermanente(Servicios servicios)
        {

            bool permanete = false;
            DateTime fechaInicio = Convert.ToDateTime(servicios.Desde);
            DateTime fechaFin = Convert.ToDateTime(servicios.Hasta);
            int intDotacion = servicios.Dotacion;

            var NombreEmpresa = (db.EMPRESA.Where(x => x.RUT == servicios.Empresa)).SingleOrDefault();
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

            if (servicios.Permanente == 1)
            {
                permanete = true;
            }

            var idServicio = db.SERVICIO.Where(y => y.CODIGO == servicios.Codigo).SingleOrDefault();
            //var idServicio = (from SERVICIO in db.SERVICIO
            //                  where
            //                    SERVICIO.CODIGO == servicios.Codigo
            //                  select new
            //                  {
            //                      SERVICIO.ID, SERVICIO.NOMBRE
            //                  }).SingleOrDefault();

            SOLICITUDPERMANENTESERVICIO nuevaSolicitud = new SOLICITUDPERMANENTESERVICIO()
            {
                FECHA = DateTime.Now,
                EMPRESA = servicios.Empresa,
                SERVICIO = Convert.ToInt32(idServicio.ID.ToString()),
                PERMANENTE = permanete,
                DOTACION = intDotacion,
                USUARIO = usuario
            };
            if (DateTime.Parse(servicios.Desde).Year == 1990)
            {
                nuevaSolicitud.FINICIO = DateTime.Now;
            }
            else
            {
                nuevaSolicitud.FINICIO = DateTime.Parse(servicios.Desde);
            }

            if (servicios.Permanente == 1)
            {
                nuevaSolicitud.FTERMINO = DateTime.Now.AddYears(-(DateTime.Now.Year) + 2099);
            }
            else
            {
                nuevaSolicitud.FTERMINO = DateTime.Parse(servicios.Hasta);
            }

            db.SOLICITUDPERMANENTESERVICIO.Add(nuevaSolicitud);
            db.SaveChanges();

            var IdRegistro = (from SOLICITUDPERMANENTESERVICIO in
                                     (from SOLICITUDPERMANENTESERVICIO in db.SOLICITUDPERMANENTESERVICIO
                                      select new
                                      {
                                          SOLICITUDPERMANENTESERVICIO.ID,
                                          Dummy = "x"
                                      })
                              group SOLICITUDPERMANENTESERVICIO by new { SOLICITUDPERMANENTESERVICIO.Dummy } into g
                              select new
                              {
                                  Column1 = (int?)g.Max(p => p.ID)
                              }).SingleOrDefault();

            //Notifica(servicios.Servicio, "", servicios.Empresa, "SOLICITUDPERMANENTESERVICIO", Convert.ToInt32(IdRegistro.Column1.ToString()));
            f.GenerarNotificacion("CONTRATACION DE SERVICIO", "Empresa " + NombreEmpresa.RSOCIAL + "contrata servicio " + idServicio.NOMBRE, true, "SOLICITUDPERMANENTESERVICIO", Convert.ToInt32(IdRegistro.Column1), false, false, null);

        }

        public ActionResult AutorizacionXServicio()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var serv = (db2.SERVICIOS_CONTRATADOS(empresa, DateTime.Now)).ToList();

            var ServiciosARRAY = serv.ToArray();
            ViewBag.Servicios = ServiciosARRAY;
            ViewBag.Empresa = empresa;
            return View();
        }

        [HttpPost]
        public ActionResult AutorizacionXServicio(IList<Servicios> Servicios)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            try
            {
                foreach (var serv in Servicios)
                {
                    if (serv.Contratar == 1)
                    {
                        RegistroSolicitudPermanente(serv);
                    }
                }
            }
            catch (Exception e3)
            {
                string Mensaje = e3.Message;
            }
            return RedirectToAction("AutorizacionXServicio", "AsAdminist");
        }

        public ActionResult ServiciosContratados()
        {

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var Servicios = (db.SERVICIOCONTRATADO.Where(x => x.EMPRESA == empresa).Select(x => new { SERVICIO = x.SERVICIO1.NOMBRE, FINICIO = x.FINICIO.ToString(), FTERMINO = x.FTERMINO.ToString(), x.OBSERVACION })).ToList().ToArray();
            ViewBag.Servicios = Servicios;
            return View(db.SERVICIOCONTRATADO.Where(x => x.EMPRESA == empresa));
        }


        //*******************************************************************************************************************************************************************
        //TICKETS*******************************************************************************************************************************************************************
        //*******************************************************************************************************************************************************************

        //ACREDITACION DE VEHICULO **********************************************************************************************************************************
        public ActionResult AcreditaVehiculo()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(93) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;



            return View(db.ACREDITAVEHICULO);
        }

        [HttpPost]
        public ActionResult AcreditaVehiculo(ACREDITAVEHICULO Vehiculo, HttpPostedFileBase PCirculacion, HttpPostedFileBase RTecnica, HttpPostedFileBase Seguro, string Patente, string Observacion)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }


            if (f.PoseePermiso(93) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            Vehiculo.USUARIO = usuario;
            Vehiculo.ESTADO = 0;
            Vehiculo.FECHA = DateTime.Now;
            //IMAGENES*****************************

            if (PCirculacion != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PCirculacion.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    Vehiculo.PDFCIRCULACION = array;
                }
            }
            if (RTecnica != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    RTecnica.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    Vehiculo.PDFRTECNICA = array;
                }
            }
            if (Seguro != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Seguro.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    Vehiculo.PDFSEGURO = array;
                }
            }
            Vehiculo.PATENTE = Patente;
            Vehiculo.OBSERVACIONES = Observacion;
            Vehiculo.FECHA = DateTime.Now;

            db.ACREDITAVEHICULO.Add(Vehiculo);
            db.SaveChanges();

            var IDInsert = (from ACREDITAVEHICULO in
                                             (from ACREDITAVEHICULO in db.ACREDITAVEHICULO
                                              select new
                                              {
                                                  ACREDITAVEHICULO.ID,
                                                  Dummy = "x"
                                              })
                            group ACREDITAVEHICULO by new { ACREDITAVEHICULO.Dummy } into g
                            select new
                            {
                                ID = (int?)g.Max(p => p.ID)
                            }).SingleOrDefault();

            if (Vehiculo.OBSERVACIONES != null)
            {
                Observacion = Vehiculo.OBSERVACIONES.ToString();
            }
            int IdSolicitud = Convert.ToInt32(IDInsert.ID.ToString());
            f.GenerarNotificacion("ACRED. VEHICULO", "Solicitud de Acred. de vehiculo fecha:" + DateTime.Now.ToShortDateString() + "Observacion: " + Observacion, true, "ACREDITAVEHICULO", IdSolicitud, false, false, usuario);

            ModelState.Clear();

            return View(db.ACREDITAVEHICULO);
        }

        //registra nuevo vehiculo en la bd**********************************************************************************************************************************
        public ActionResult RegistraVehiculo(int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(82) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var Marca = (from MARCA in db.MARCA
                         select new
                         {
                             id = MARCA.ID,
                             descripcion = MARCA.MARCA1
                         }).ToList();
            var MarcaSL = new SelectList(Marca, "id", "descripcion");
            ViewBag.Marca = MarcaSL;

            var Modelo = (from MODELO in db.MODELO
                          select new
                          {
                              id = MODELO.ID,
                              MODELO = MODELO.MODELO1,
                          }).ToList();
            var ModeloSL = new SelectList(Modelo, "id", "MODELO");
            ViewBag.Modelo = ModeloSL;

            var Carroceria = (from CARROCERIA in db.CARROCERIA
                              select new
                              {
                                  id = CARROCERIA.ID,
                                  CARROCERIA = CARROCERIA.CARROCERIA1
                              }).ToList();
            var CarroceriaSL = new SelectList(Carroceria, "id", "CARROCERIA");
            ViewBag.Carroceria = CarroceriaSL;


            return View(db.VEHICULO);

        }
        [HttpPost]
        public ActionResult RegistraVehiculo(VEHICULO Vehiculo)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(116) < 82) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var Marca = (from MARCA in db.MARCA
                         select new
                         {
                             id = MARCA.ID,
                             descripcion = MARCA.MARCA1
                         }).ToList();
            var MarcaSL = new SelectList(Marca, "id", "descripcion");
            ViewBag.Marca = MarcaSL;

            var Modelo = (from MODELO in db.MODELO
                          select new
                          {
                              id = MODELO.ID,
                              MODELO = MODELO.MODELO1,
                          }).ToList();
            var ModeloSL = new SelectList(Modelo, "id", "MODELO");
            ViewBag.Modelo = ModeloSL;

            var Carroceria = (from CARROCERIA in db.CARROCERIA
                              select new
                              {
                                  id = CARROCERIA.ID,
                                  CARROCERIA = CARROCERIA.CARROCERIA1
                              }).ToList();
            var CarroceriaSL = new SelectList(Carroceria, "id", "CARROCERIA");
            ViewBag.Carroceria = CarroceriaSL;

            Vehiculo.EMPRESA = empresa;

            db.VEHICULO.Add(Vehiculo);
            db.SaveChanges();


            return RedirectToAction("AcreditaVehiculo", "AsAdminist");
        }

        //CARTA AMONESTACION **********************************************************************************************************************************
        public ActionResult Amonestacion()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(94) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faena = (from FAENA in db.FAENA
            //             where
            //               FAENA.EMPRESA == empresa
            //             select new
            //             {
            //                 Id = FAENA.ID,
            //                 Descripcion = FAENA.DESCRIPCION
            //             }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> TrabajadorList = new List<SelectListItem>();

            TrabajadorList.Add(new SelectListItem() { Text = "Seleccione trabajador", Value = "1" });

            var TrabajadoresSL = new SelectList(TrabajadorList, "Value", "Text");
            ViewBag.Trabajadores = TrabajadoresSL;

            ViewBag.Fecha = DateTime.Now.ToString("yyyy'-'MM'-'dd");

            ViewBag.Visible = "display:none";

            return View();
        }

        [HttpPost]
        public ActionResult Amonestacion(int faena, string trabajador, DateTime Fecha, string Amonestacion)
        {
            if (f.PoseePermiso(94) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            var Trabajadores = (from trabajadores in db.CONTRATO
                                where trabajadores.FAENA == faena
                                select new { RUT = trabajadores.PERSONA, NOMBRE = trabajadores.PERSONA1.APATERNO + " " + trabajadores.PERSONA1.AMATERNO + ", " + trabajadores.PERSONA1.NOMBRE }).Distinct().OrderBy(x => x.NOMBRE);

            var TrabajadoresSL = new SelectList(Trabajadores.OrderBy(x => x.NOMBRE), "RUT", "NOMBRE");
            ViewBag.Trabajadores = TrabajadoresSL;

            ViewBag.Fecha = Fecha.ToString("yyyy'-'MM'-'dd");


            ViewBag.Visible = "";

            if (trabajador != "1")
            {
                CARTAAMONESTACION Nueva = new CARTAAMONESTACION();
                Nueva.FECHA = Fecha;
                Nueva.EMPRESA = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
                Nueva.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
                Nueva.RUT = trabajador;
                Nueva.AMONESTACION = Amonestacion;
                Nueva.ESTADO = 0;

                db.CARTAAMONESTACION.Add(Nueva);
                db.SaveChanges();

                var IdAmonestacion = (from amon in db2.MAX_ID_CARTAAMONESTACION
                                      select amon).SingleOrDefault();

                f.GenerarNotificacion("C. AMONESTACION", "Solicitud de C. de Amonestación fecha:" + DateTime.Now.ToShortDateString() + " Observacion: " + Amonestacion, true, "CARTAAMONESTACION", Convert.ToInt32(IdAmonestacion.ID), false, false, trabajador);

                ModelState.Clear();

                return View();
            }
            //Notifica("AMONESTACION", Datos.AMONESTACION.ToString(), empresa, "CARTAAMONESTACION", Convert.ToInt32(IdRegistro.Column1.ToString()));

            return View();
        }

        // MOVILIZACION HHEE **********************************************************************************************************************************
        public ActionResult MovilizacionHHEE()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(95) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;
            return View(db.MOVILIZACIONHHEE);
        }

        [HttpPost]
        public ActionResult MovilizacionHHEE(MOVILIZACIONHHEE Movilizacion, string Fecha, string Hora, string Unidades)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(95) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            Movilizacion.FECHAMOVILIZACION = Convert.ToDateTime(Fecha + " " + Hora + ":00");
            Movilizacion.FECHA = DateTime.Now;
            Movilizacion.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            Movilizacion.CANTTRABAJADORES = Convert.ToInt32(Unidades);
            Movilizacion.ESTADO = 0;

            db.MOVILIZACIONHHEE.Add(Movilizacion);
            db.SaveChanges();

            ModelState.Clear();

            var IdRegistro = (from MOVILIZACIONHHEE in
                                          (from MOVILIZACIONHHEE in db.MOVILIZACIONHHEE
                                           select new
                                           {
                                               MOVILIZACIONHHEE.ID,
                                               Dummy = "x"
                                           })
                              group MOVILIZACIONHHEE by new { MOVILIZACIONHHEE.Dummy } into g
                              select new
                              {
                                  Column1 = (int?)g.Max(p => p.ID)
                              }).SingleOrDefault();

            String Observacion = string.Empty;
            if (Movilizacion.OBSERVACION != null)
            {
                Observacion = Movilizacion.OBSERVACION.ToString();
            }
            f.GenerarNotificacion("MOVILIZACION HHEE", "Solicitud de Movilización fecha:" + DateTime.Now.ToShortDateString() + " Observacion: " + Observacion, true, "MOVILIZACIONHHEE", Convert.ToInt32(IdRegistro.Column1.ToString()), false, false, null); ;



            return View(db.MOVILIZACIONHHEE);
        }

        //ANEXO **********************************************************************************************************************************
        public ActionResult SolicitudAnexo()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(81) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faena = (from FAENA in db.FAENA
            //             where
            //               FAENA.EMPRESA == empresa
            //             select new
            //             {
            //                 Id = FAENA.ID,
            //                 Descripcion = FAENA.DESCRIPCION
            //             }).ToList();

            var FaenaSL = f.FaenasEmpresa();

            ViewBag.Faenas = FaenaSL;

            ViewBag.FechaInicio = DateTime.Now.ToString("yyyy'-'MM'-'dd");

            return View(db.ANEXOS);
        }

        [HttpPost]
        public ActionResult SolicitudAnexo(SOLICITUDANEXO Anexo, DateTime Fecha, string RutTrabajadores, string todos)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(81) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            ViewBag.FechaInicio = Fecha.ToString("yyyy'-'MM'-'dd");

            Anexo.EMPRESA = empresa;
            Anexo.ESTADO = 0;
            Anexo.FECHA = DateTime.Now;
            Anexo.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            Anexo.FECHAINICIOANEXO = Fecha;

            string Observacion = string.Empty;
            if (Anexo.OBSERVACION != null)
            {
                Observacion = Anexo.OBSERVACION.ToString();
            }
            Observacion = Observacion + " ----- Rut Trabajajadores: #" + RutTrabajadores;


            Anexo.OBSERVACION = Observacion;

            db.SOLICITUDANEXO.Add(Anexo);
            db.SaveChanges();

            var IdRegistro = (from ANEXOS in
                                              (from ANEXOS in db.SOLICITUDANEXO
                                               select new
                                               {
                                                   ANEXOS.ID,
                                                   Dummy = "x"
                                               })
                              group ANEXOS by new { ANEXOS.Dummy } into g
                              select new
                              {
                                  Column1 = (int?)g.Max(p => p.ID)
                              }).SingleOrDefault();

            int Registro = Convert.ToInt32(IdRegistro.Column1.ToString());


            f.GenerarNotificacion("SOLICITUD ANEXO", "Solicitud de Anexo fecha:" + DateTime.Now.ToShortDateString(), true, "SOLICITUDANEXO", Registro, false, false, null);


            ModelState.Clear();

            return View(db.SOLICITUDANEXO);
        }

        public JsonResult GetTrabajadoresList(string FaenaId, string searchTerm)
        {
            int IdFaena = Convert.ToInt32(FaenaId);

            var TrabajadoresList = db.CONTRATO.Where(x => x.FTERMNO >= DateTime.Now && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.FAENA == IdFaena).Select(x => new { RUT = x.PERSONA, NOMBRE = x.PERSONA1.APATERNO + " " + x.PERSONA1.AMATERNO + ", " + x.PERSONA1.NOMBRE }).ToList();

            if (searchTerm != null)
            {
                TrabajadoresList = TrabajadoresList.Where(x => x.NOMBRE.Contains(searchTerm.ToUpper())).ToList();
            }

            var modifiedData = TrabajadoresList.Select(x => new
            {
                id = x.RUT,
                text = x.NOMBRE
            });
            return Json(modifiedData, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetTrabajadoresSeleccionados(string trabajadores)
        {
            var test = trabajadores;
            return Json(trabajadores, JsonRequestBehavior.AllowGet);
        }

        //COLACION HHEE **********************************************************************************************************************************
        public ActionResult ColacionHHEE()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(96) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            return View(db.COLACIONESHHEE);
        }

        [HttpPost]
        public ActionResult ColacionHHEE(COLACIONESHHEE Colacion, string Fecha, string Hora, string Unidades)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(96) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            Colacion.FECHACOLACION = Convert.ToDateTime(Fecha + " " + Hora + ":00");
            Colacion.FECHA = DateTime.Now;
            Colacion.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            Colacion.CANTTRABAJADORES = Convert.ToInt32(Unidades);

            db.COLACIONESHHEE.Add(Colacion);
            db.SaveChanges();

            var IdRegistro = (from COLACIONESHHEE in
                                          (from COLACIONESHHEE in db.COLACIONESHHEE
                                           select new
                                           {
                                               COLACIONESHHEE.ID,
                                               Dummy = "x"
                                           })
                              group COLACIONESHHEE by new { COLACIONESHHEE.Dummy } into g
                              select new
                              {
                                  Column1 = (int?)g.Max(p => p.ID)
                              }).SingleOrDefault();

            int Registro = Convert.ToInt32(IdRegistro.Column1.ToString());


            string Observacion = string.Empty;
            if (Colacion.OBSERVACION != null)
            {
                Observacion = Colacion.OBSERVACION.ToString();
            }

            f.GenerarNotificacion("COLACION HHEE", "Solicitud de Colación fecha:" + DateTime.Now.ToShortDateString() + " Observacion: " + Observacion, true, "COLACIONESHHEE", Registro, false, false, null);


            ModelState.Clear();

            return View(db.COLACIONESHHEE);
        }

        //ACT. EXTRAPROGRAMATICA **********************************************************************************************************************************
        public ActionResult Extraprogramatica()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(100) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            return View(db.ACTEXTRAPROGRAMATICA);
        }
        [HttpPost]
        public ActionResult Extraprogramatica(ACTEXTRAPROGRAMATICA Actividad, string Fecha, string Hora, string Observacion)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(100) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            Actividad.FECHAACTIVIDAD = Convert.ToDateTime(Fecha + " " + Hora + ":00");
            Actividad.FECHA = DateTime.Now;
            Actividad.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            Actividad.ESTADO = 0;

            db.ACTEXTRAPROGRAMATICA.Add(Actividad);
            db.SaveChanges();


            var IdRegistro = (from ACTEXTRAPROGRAMATICA in
                                          (from ACTEXTRAPROGRAMATICA in db.ACTEXTRAPROGRAMATICA
                                           select new
                                           {
                                               ACTEXTRAPROGRAMATICA.ID,
                                               Dummy = "x"
                                           })
                              group ACTEXTRAPROGRAMATICA by new { ACTEXTRAPROGRAMATICA.Dummy } into g
                              select new
                              {
                                  Column1 = (int?)g.Max(p => p.ID)
                              }).SingleOrDefault();

            f.GenerarNotificacion("ACT. EXTRAPROG.", "Solicitud de Act. Extraprogramatica fecha:" + DateTime.Now.ToShortDateString() + " Observacion: " + Observacion, true, "ACTEXTRAPROGRAMATICA", Convert.ToInt32(IdRegistro.Column1.ToString()), false, false, null);


            ModelState.Clear();


            return View(db.ACTEXTRAPROGRAMATICA);
        }

        //CALIBRACION DE EQUIPOS **********************************************************************************************************************************
        public ActionResult CalibracionEquipos()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(99) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            //string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            return View(db.CALIBRACIONEQUIPOS);
        }
        [HttpPost]
        public ActionResult CalibracionEquipos(CALIBRACIONEQUIPOS Modelo, string Fecha, string Hora, string Unidades)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(99) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            Modelo.FECHACALIBRACION = Convert.ToDateTime(Fecha + " " + Hora + ":00");
            Modelo.FECHA = DateTime.Now;
            Modelo.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            Modelo.UNIIDADES = Convert.ToInt32(Unidades);
            Modelo.ESTADO = 0;

            db.CALIBRACIONEQUIPOS.Add(Modelo);
            db.SaveChanges();

            var IdRegistro = (from CALIBRACIONEQUIPOS in
                                          (from CALIBRACIONEQUIPOS in db.CALIBRACIONEQUIPOS
                                           select new
                                           {
                                               CALIBRACIONEQUIPOS.ID,
                                               Dummy = "x"
                                           })
                              group CALIBRACIONEQUIPOS by new { CALIBRACIONEQUIPOS.Dummy } into g
                              select new
                              {
                                  Column1 = (int?)g.Max(p => p.ID)
                              }).SingleOrDefault();

            string Observacion = string.Empty;
            if (Modelo.OBSERVACION != null)
            {
                Observacion = Modelo.OBSERVACION.ToString();
            }
            f.GenerarNotificacion("CALIB. EQUIPOS", "Solicitud de Calibración de Equipo fecha:" + DateTime.Now.ToShortDateString() + " Observacion: " + Observacion, true, "CALIBRACIONEQUIPOS", Convert.ToInt32(IdRegistro.Column1.ToString()), false, false, null);


            ModelState.Clear();


            return View(db.CALIBRACIONEQUIPOS);
        }

        //LIMPIEZA DE BAÑOS **********************************************************************************************************************************
        public ActionResult LimpiezaBanos()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(98) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            return View(db.LIMPIEZABANHOS);
        }

        [HttpPost]
        public ActionResult LimpiezaBanos(LIMPIEZABANHOS Modelo, string Fecha, string Hora, string Unidades)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(98) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            Modelo.FECHALIMPIEZA = Convert.ToDateTime(Fecha + " " + Hora + ":00");
            Modelo.FECHA = DateTime.Now;
            Modelo.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            Modelo.UNIIDADES = Convert.ToInt32(Unidades);
            Modelo.ESTADO = 0;

            db.LIMPIEZABANHOS.Add(Modelo);
            db.SaveChanges();

            var IdRegistro = (from LIMPIEZABANHOS in
                                          (from LIMPIEZABANHOS in db.LIMPIEZABANHOS
                                           select new
                                           {
                                               LIMPIEZABANHOS.ID,
                                               Dummy = "x"
                                           })
                              group LIMPIEZABANHOS by new { LIMPIEZABANHOS.Dummy } into g
                              select new
                              {
                                  Column1 = (int?)g.Max(p => p.ID)
                              }).SingleOrDefault();



            string Observacion = string.Empty;
            if (Modelo.OBSERVACION != null)
            {
                Observacion = Modelo.OBSERVACION.ToString();
            }
            f.GenerarNotificacion("LIMPIEZA BAÑOS", "Solicitud de Limpieza de Baños fecha:" + DateTime.Now.ToShortDateString() + " Observacion: " + Observacion, true, "LIMPIEZABANHOS", Convert.ToInt32(IdRegistro.Column1.ToString()), false, false, null);



            ModelState.Clear();


            return View(db.LIMPIEZABANHOS);
        }

        public ActionResult SolicitudVacaciones()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(108) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faena = (from FAENA in db.FAENA
            //             where
            //               FAENA.EMPRESA == empresa
            //             select new
            //             {
            //                 Id = FAENA.ID,
            //                 Descripcion = FAENA.DESCRIPCION
            //             }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> TrabajadorList = new List<SelectListItem>();

            TrabajadorList.Add(new SelectListItem() { Text = "Seleccione trabajador", Value = "1" });

            var TrabajadoresSL = new SelectList(TrabajadorList, "Value", "Text");
            ViewBag.Trabajadores = TrabajadoresSL;
            ViewBag.vigenteSelected = 1;

            ViewBag.FechaInicio = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = DateTime.Now.ToString("yyyy'-'MM'-'dd");

            ViewBag.Visible = "display:none";
            ViewBag.OK = "display:none";
            ViewBag.ERROR = "display:none";
            ViewBag.Alert = string.Empty;

            return View();
        }

        [HttpPost]
        public ActionResult SolicitudVacaciones(int Faena, DateTime FechaInicio, DateTime FechaTermino, string Trabajador, float dias_legales, float dias_progresivos, int vigentes, string submitbutton)
        {

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            if (f.PoseePermiso(108) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;
            /*
            var Trabajadores = (from trabajadores in db.CONTRATO
                                where trabajadores.FAENA == Faena
                                select new { RUT = trabajadores.PERSONA, NOMBRE = trabajadores.PERSONA1.APATERNO + " " + trabajadores.PERSONA1.AMATERNO + ", " + trabajadores.PERSONA1.NOMBRE }).Distinct().OrderBy(x => x.NOMBRE);
                                */

            var Trabajadores = f.filtroTrabajadoresVigencia(Faena, Convert.ToBoolean(vigentes));

            var TrabajadoresSL = new SelectList(Trabajadores, "RUT", "NOMBRE");
            ViewBag.Trabajadores = TrabajadoresSL;
            ViewBag.vigenteSelected = Convert.ToBoolean(vigentes) ? 1 : 0;

            ViewBag.FechaInicio = FechaInicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = FechaTermino.ToString("yyyy'-'MM'-'dd");

            ViewBag.Visible = "";
            ViewBag.OK = "display:none";
            ViewBag.ERROR = "display:none";
            ViewBag.Alert = string.Empty;

            if (Trabajador != "1" && submitbutton != null)
            {
                try
                {
                    ViewBag.Visible = "display:none";
                    ViewBag.OK = "";
                    ViewBag.ERROR = "display:none";
                    


                    string PuedeRegistrar = db2.FILTRO_FECHAS_SOLICITUDES(Trabajador, FechaInicio, FechaTermino, "V").Select(x => x.Column1).SingleOrDefault();

                    if (PuedeRegistrar != "OK")
                    {
                        ViewBag.OK = "display:none";

                        if (PuedeRegistrar == "VACACION")
                        {
                            ViewBag.Alert = "Ya existe un periodo de Vacaciones registrado en el rango de fechas seleccionado para este trabajador";
                            return View();
                        }
                        if (PuedeRegistrar == "LICENCIA")
                        {
                            ViewBag.Alert = "Ya existe una Licencia Médica registrado en el rango de fechas seleccionado para este trabajador";
                            return View();
                        }

                    }

                    SOLICITUDVACACIONES Nueva = new SOLICITUDVACACIONES();
                    Nueva.AUTORIZAEMPRESA = false;
                    Nueva.AUTORIZATRABAJADOR = false;
                    Nueva.EMPRESA = empresa;
                    Nueva.FECHA = DateTime.Now;
                    Nueva.FINICIO = FechaInicio;
                    Nueva.FTERMINO = FechaTermino;
                    Nueva.USUARIO = usuario;
                    Nueva.TRABAJADOR = Trabajador;
                    Nueva.DESGLOSE = "LEGALES:" + dias_legales + "#PROGRESIVOS:" + dias_progresivos;
                    Nueva.ESTADO = 0;
                    Nueva.RECHAZADA = false;

                    db.SOLICITUDVACACIONES.Add(Nueva);
                    db.SaveChanges();

                    var SolicitudID = (db2.MAX_ID_SOLICITUD_VACACIONES.SingleOrDefault());
                    f.GenerarNotificacion("VACACIONES", "Solicitud de Vacaciones", true, "SOLICITUDVACACIONES", Convert.ToInt32(SolicitudID.ID), false, true, Trabajador);

                    ViewBag.Alert = "Solicitud registrada existosamente";
                    return RedirectToAction("SolicitudVacaciones", "AsAdminist");


                }
                catch (Exception)
                {
                    ViewBag.ERROR = "";
                    ViewBag.OK = "display:none";
                    return View();
                }

            }
            return View();
        }

        public ActionResult AutorizacionVacaciones()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(108) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = "display:none";

            //DateTime finicio = DateTime.Now.AddDays(-1).AddHours(-DateTime.Now.Hour);
            //DateTime ftermino = DateTime.Now;

            //ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            //ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            return View(db2.OFERTA_VACACIONES(int.Parse(FaenaSL.First().Value)));
        }

        [HttpPost]
        public ActionResult AutorizacionVacaciones(int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            if (f.PoseePermiso(108) < 14) { return Redirect("~/Home/ErrorPorPermiso"); }


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = " ";

            //ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            //ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            return View(db2.OFERTA_VACACIONES(faena));
        }

        public bool Permiso(int permiso)
        {
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var Autoriza = (from autoriza in db.PERMISO
                            where autoriza.PERMISO1 == permiso && autoriza.EMPRESA == empresa && autoriza.RUT == usuario
                            select autoriza).ToList();
            if (Autoriza.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual JsonResult AutorizaVacaciones(string RUT, string FINICIO, string FTERMINO, int IDSOLICITUD)
        {
            try
            {
                if (f.PoseePermiso(108) == 14 && f.isLogged())
                {
                    return Json(new
                    {
                        Mensaje = "No posee permisos para autorizar vacaciones",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {
                    DateTime Finicio = Convert.ToDateTime(FINICIO);
                    DateTime Ftermino = Convert.ToDateTime(FTERMINO);

                    string PuedeRegistrar = db2.FILTRO_FECHAS_SOLICITUDES(RUT, Finicio, Ftermino, "V").Select(x => x.Column1).SingleOrDefault();

                    if (PuedeRegistrar != "OK")
                    {
                        string Mensaje = string.Empty;

                        if (PuedeRegistrar == "VACACION")
                        {
                            Mensaje = "Ya existe un periodo de Vacaciones registrado en el rango de fechas seleccionado para este trabajador";

                        }
                        if (PuedeRegistrar == "LICENCIA")
                        {
                            Mensaje = "Ya existe una Licencia Médica registrado en el rango de fechas seleccionado para este trabajador";
                        }

                        return Json(new
                        {
                            Mensaje = Mensaje,
                            redirectURL = Url.Action("AutorizacionVacaciones", "AsAdminist"),
                            isRedirect = true
                        });

                    }

                    var SolicitudID = (db2.MAX_ID_SOLICITUD_VACACIONES.SingleOrDefault());

                    var Trabajador = (from trab in db.PERSONA
                                      where trab.RUT == RUT
                                      select new
                                      {
                                          TRABAJADOR = trab.RUT + " - " + trab.APATERNO + ", " + trab.NOMBRE
                                      }).SingleOrDefault();

                    f.EliminaNotificacion("SOLICITUDVACACIONES", Convert.ToInt32(SolicitudID.ID));
                    f.GenerarNotificacion("VACACIONES", "Solicitud autorizada", false, "SOLICITUDVACACIONES", Convert.ToInt32(SolicitudID.ID), false, true, RUT);
                    f.GenerarNotificacion("VACACIONES", "Solicitud autorizada Trabajador: " + Trabajador.TRABAJADOR, true, "SOLICITUDVACACIONES", Convert.ToInt32(SolicitudID.ID), false, false, RUT);


                    db2.AUTRIZA_VACACIONES(RUT, Convert.ToDateTime(FINICIO), Convert.ToDateTime(FTERMINO));

                    string DesgloseVacacion = (db.SOLICITUDVACACIONES.Where(x => x.ID == IDSOLICITUD).Select(x => x.DESGLOSE)).SingleOrDefault();
                    string[] DetalleDesglose = DesgloseVacacion.Split('#');
                    foreach (var TipoDesglose in DetalleDesglose)
                    {
                        string[] DiasDesglose = TipoDesglose.Split(':');
                        int idTipoVacacion = 0;
                        switch (DiasDesglose[0])
                        {
                            case "LEGALES":
                                idTipoVacacion = 1;
                                break;
                            case "PROGRESIVOS":
                                idTipoVacacion = 2;
                                break;

                        }
                        int IdVacacion = int.Parse(db2.MAX_ID_VACACIONES.Select(x => x.Expr1).SingleOrDefault().ToString());
                        DESGLOSEVACACION Nueva = new DESGLOSEVACACION();
                        Nueva.ID_PERIODO = IdVacacion;
                        Nueva.ID_TIPO = idTipoVacacion;
                        Nueva.DIAS = int.Parse(DiasDesglose[1].ToString());
                        db.DESGLOSEVACACION.Add(Nueva);

                        db.SaveChanges();

                    }
                        return Json(new
                        {
                            Mensaje = "Solicitud autorizada exitosamente",
                            redirectURL = Url.Action("AutorizacionVacaciones", "AsAdminist"),
                            isRedirect = true
                        });

                }
            }
            catch (Exception e2)
            {
                return Json(new
                {
                    Mensaje = e2.Message
                });
            }
        }

        public virtual JsonResult RechazaVacaciones(string RUT, string FINICIO, string FTERMINO)
        {
            try
            {

                if (f.PoseePermiso(108) == 14 && f.isLogged())
                {
                    return Json(new
                    {
                        Mensaje = "No posee permisos para rechazar vacaciones",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {

                    var SolicitudID = (db2.MAX_ID_SOLICITUD_VACACIONES.SingleOrDefault());

                    var Trabajador = (from trab in db.PERSONA
                                      where trab.RUT == RUT
                                      select new
                                      {
                                          TRABAJADOR = trab.RUT + " - " + trab.APATERNO + ", " + trab.NOMBRE
                                      }).SingleOrDefault();

                    f.EliminaNotificacion("SOLICITUDVACACIONES", Convert.ToInt32(SolicitudID.ID));
                    f.GenerarNotificacion("VACACIONES", "Solicitud rechazada", false, "SOLICITUDVACACIONES", Convert.ToInt32(SolicitudID.ID), false, true, RUT);
                    f.GenerarNotificacion("VACACIONES", "Solicitud rechazada Trabajador: " + Trabajador.TRABAJADOR, true, "SOLICITUDVACACIONES", Convert.ToInt32(SolicitudID.ID), false, false, RUT);



                    db2.RECHAZA_VACACIONES(RUT, Convert.ToDateTime(FINICIO), Convert.ToDateTime(FTERMINO));
                    return Json(new
                    {
                        Mensaje = "Solicitud rechazada exitosamente",
                        redirectURL = Url.Action("AutorizacionVacaciones", "AsAdminist"),
                        isRedirect = true
                    });
                }
            }
            catch (Exception e2)
            {
                return Json(new
                {
                    Mensaje = e2.Message
                });
            }
        }

        public virtual JsonResult AutorizaTodoVacaciones(DateTime FINICIO, DateTime FTERMINO, int FAENA)
        {
            try
            {
                if (f.PoseePermiso(108) == 14 && f.isLogged())
                {
                    return Json(new
                    {
                        Mensaje = "No posee permisos para autorizar vacaciones",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {
                    var Autorizaciones = (from aut in db2.OFERTA_VACACIONES(FAENA)
                                          select aut).ToList();

                    foreach (var item in Autorizaciones.ToList())
                    {


                        DateTime Finicio = Convert.ToDateTime(FINICIO);
                        DateTime Ftermino = Convert.ToDateTime(FTERMINO);
                        string PuedeRegistrar = db2.FILTRO_FECHAS_SOLICITUDES(item.RUT, Finicio, Ftermino, "V").Select(x => x.Column1).SingleOrDefault();

                        if (PuedeRegistrar != "OK")
                        {
                            string Mensaje = string.Empty;

                            if (PuedeRegistrar == "VACACION")
                            {
                                Mensaje = "Ya existe un periodo de Vacaciones registrado en el rango de fechas seleccionado para el trabajador " + item.NOMBRE + "\n Anule esta solicitud y posteriormente prosiga con la autorización.\nEs posible que deba actualizar esta página";

                            }
                            if (PuedeRegistrar == "LICENCIA")
                            {
                                Mensaje = "Ya existe una Licencia Médica registrado en el rango de fechas seleccionado para el trabajador " + item.NOMBRE + "\n Anule esta solicitud y posteriormente prosiga con la autorización.\nEs posible que deba actualizar esta página";
                            }

                            return Json(new
                            {
                                Mensaje = Mensaje,
                                redirectURL = Url.Action("AutorizacionVacaciones", "AsAdminist"),
                                isRedirect = true
                            });

                        }

                        db2.AUTRIZA_VACACIONES(item.RUT, Convert.ToDateTime(item.FINICIO), Convert.ToDateTime(item.FTERMINO));
                    }


                    return Json(new
                    {
                        Mensaje = "Solicitudes registradas exitosamente",
                        redirectURL = Url.Action("AutorizacionVacaciones", "AsAdminist"),
                        isRedirect = true
                    });
                }
            }
            catch (Exception e2)
            {
                return Json(new
                {
                    Mensaje = e2.Message
                });
            }

        }



        public ActionResult SolicitudPermisoInasistencia()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(109) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string trabaj = System.Web.HttpContext.Current.Session["sessionTrabajador"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            List<SelectListItem> TrabajadorList = new List<SelectListItem>();

            if (trabaj == "N")
            {

                TrabajadorList.Add(new SelectListItem() { Text = "Seleccione trabajador", Value = "1" });
                var TrabajadoresSL = new SelectList(TrabajadorList, "Value", "Text");
                ViewBag.Trabajadores = TrabajadoresSL;

            }
            else
            {
                var persona = (db.PERSONA.Where(x => x.RUT == usuario)).SingleOrDefault();
                string nom = persona.APATERNO + " " + persona.AMATERNO + " " + persona.NOMBRE;
                List<SelectListItem> UsuarioList = new List<SelectListItem> { new SelectListItem() { Text = nom, Value = "U" }, };
                ViewBag.Usuarios = new SelectList(UsuarioList, "Value", "Text");


            }

            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;
            ViewBag.Fecha = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.Alert = string.Empty;
            ViewBag.FechaInicio = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.HoraInicio = DateTime.Now.Hour;
            ViewBag.HoraTermino = DateTime.Now.Hour;

            ViewBag.Visible = "display:none";
            ViewBag.OK = "display:none";
            ViewBag.ERROR = "display:none";
            ViewBag.Alert2 = string.Empty;
            ViewBag.Compensados = false;

            if (f.PermisoPorServicio(112) == false)
            {
                ViewBag.Compensados = true;
            }

            var tipper = db.remepage.Where(x => x.nom_tabla == "PERMISO" && x.rut_empr == empresa).Select(x => new { Id = x.gls_param, Descripcion = x.gls_param }).ToList().OrderBy(x => x.Descripcion);
            var TipperSL = new SelectList(tipper, "Id", "Descripcion");
            ViewBag.Tipper = TipperSL;



            return View();
        }


        [HttpPost]
        public int SolicitudPermisoInasistencia(string Trabajador, string FechaInicio, string FechaTermino, string HoraInicio, string HoraTermino, string empresa)
        {


          
            DateTime FechaIni = Convert.ToDateTime(FechaInicio);
            DateTime FechaTer = Convert.ToDateTime(FechaTermino);
                    string PuedeRegistrar = db2.FILTRO_FECHAS_SOLICITUDES(Trabajador, FechaIni, FechaTer, "P").Select(x => x.Column1).SingleOrDefault();


                  

                    var Persona = (from persona in db.PERSONA
                                   where persona.RUT == Trabajador
                                   select new
                                   {
                                       NOMBRE = persona.APATERNO + " " + persona.AMATERNO + ", " + persona.NOMBRE
                                   }).SingleOrDefault();
                    //db2.INSERT_SOLICITUD_PERMISOINASISTENCIA(usuario, Trabajador, FechaInicio, FechaTermino, empresa, 0);
                    SOLICITUDPERMISOINASISTENCIA sol = new SOLICITUDPERMISOINASISTENCIA();
                    sol.USUARIO = Trabajador;
                    sol.TRABAJADOR = Trabajador;
                    sol.FINICIO = FechaIni;
                    sol.FTERMINO = FechaTer;
                    sol.FECHA = DateTime.Now.Date;
                    sol.AUTORIZAEMPRESA = null;
                    sol.AUTORIZATRABAJADOR = true;
                    sol.PDF = null;
                    sol.EMPRESA = empresa;
                    sol.ESTADO = 0;
                    sol.RECHAZADA = false;
                    sol.COMPENSADO = 0;
                    sol.OBSERVACION = "0";
                    db.SOLICITUDPERMISOINASISTENCIA.Add(sol);
                    db.SaveChanges();

                    var SolicitudID = (db2.MAX_ID_SOL_PERMISO.SingleOrDefault());
                    return 1;
         }

        public ActionResult SolicitudPermisoInasistenciaDiaCompensado()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(112) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            // BPD - 20201206 - Cambios para aplicar filtros de vigente y no vigente para los trabajadores de la empresa

            //List<SelectListItem> Faenas = new List<SelectListItem>();
            //Faenas = (from FAENA in db.FAENA
            //          where FAENA.EMPRESA == empresa
            //          select new SelectListItem { Value = FAENA.ID.ToString(), Text = FAENA.DESCRIPCION }).ToList();

            var Faenas = f.FaenasEmpresa();
            ViewBag.Faenas = Faenas;

            ViewBag.Fecha = DateTime.Now.ToString("yyyy'-'MM'-'dd");


            ViewBag.Alert = false;

            ViewBag.FechaInicio = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.HoraInicio = DateTime.Now.Hour;
            ViewBag.HoraTermino = DateTime.Now.Hour;

            ViewBag.Visible = "display:none";
            ViewBag.OK = "display:none";
            ViewBag.ERROR = "display:none";
            ViewBag.Alert2 = string.Empty;

            List<SelectListItem> TrabajadorList = new List<SelectListItem>();

            TrabajadorList.Add(new SelectListItem() { Text = "Seleccione Trabajador", Value = "1" });

            var TrabajadoresSL = new SelectList(TrabajadorList, "Value", "Text");
            ViewBag.Trabajadores = TrabajadoresSL;
            ViewBag.vigenteSelected = 1;

            ViewBag.Compensados = false;

            if (f.PermisoPorServicio(112) == false)
            {
                ViewBag.Compensados = true;
            }

            
            //return View(Faenas);
            return View();
        }

        [HttpPost]
        public ActionResult SolicitudPermisoInasistenciaDiaCompensado(int Faena, DateTime FechaInicio, DateTime FechaTermino, int HoraInicio, int HoraTermino, string Trabajador, int Compensado2, int vigentes, string submitbutton)
        {


            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            if (f.PoseePermiso(112) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

            // BPD - 20201206 - Cambios para aplicar filtros de vigente y no vigente para los trabajadores de la empresa

            //List<SelectListItem> Faenas = new List<SelectListItem>();
            //Faenas = (from FAENA in db.FAENA
            //          where FAENA.EMPRESA == empresa
            //          select new SelectListItem { Value = FAENA.ID.ToString(), Text = FAENA.DESCRIPCION }).ToList();

            var Faenas = f.FaenasEmpresa();
            ViewBag.Faenas = Faenas;

            var Trabajadores = f.filtroTrabajadoresVigencia(Faena, Convert.ToBoolean(vigentes));

            var TrabajadoresSL = new SelectList(Trabajadores, "RUT", "NOMBRE");
            ViewBag.Trabajadores = TrabajadoresSL;
            ViewBag.vigenteSelected = Convert.ToBoolean(vigentes) ? 1 : 0;

            ViewBag.FechaInicio = FechaInicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = FechaTermino.ToString("yyyy'-'MM'-'dd");

            var Fechasfaena = f.FechasLimiteFaena(FechaInicio, FechaTermino, Faena);
            FechaInicio = Fechasfaena[0];
            FechaTermino = Fechasfaena[1];

            FechaInicio = FechaInicio.AddHours(HoraInicio);
            FechaTermino = FechaTermino.AddHours(HoraTermino);

            ViewBag.Visible = "";

            ViewBag.HoraInicio = DateTime.Now.Hour;
            ViewBag.HoraTermino = DateTime.Now.Hour;

            ViewBag.Compensados = "display:none";
            ViewBag.Alert2 = string.Empty;

            if (f.PermisoPorServicio(112) == true)
            {
                ViewBag.Compensados = "";
            }

            if (submitbutton != null)
            {
                ViewBag.Alert = false;
                return View();
            }

            try
            {

                string PuedeRegistrar = db2.FILTRO_FECHAS_SOLICITUDES(Trabajador, FechaInicio, FechaTermino, "P").Select(x => x.Column1).SingleOrDefault();

                if (PuedeRegistrar != "OK")
                {
                    if (PuedeRegistrar == "VACACION")
                    {
                        ViewBag.Alert2 = "Ya existe un periodo de Vacaciones registrado en el rango de fechas seleccionado para este trabajador";
                        return View();
                    }
                    if (PuedeRegistrar == "LICENCIA")
                    {
                        ViewBag.Alert2 = "Ya existe una Licencia Médica registrado en el rango de fechas seleccionado para este trabajador";
                        return View();
                    }
                    if (PuedeRegistrar == "PERMISO")
                    {
                        ViewBag.Alert2 = "Ya existe un Permiso de Inasistencia registrado en el rango de fechas seleccionado para este trabajador";
                        return View();
                    }

                }

                var Persona = (from persona in db.PERSONA
                               where persona.RUT == Trabajador
                               select new
                               {
                                   NOMBRE = persona.APATERNO + " " + persona.AMATERNO + ", " + persona.NOMBRE
                               }).SingleOrDefault();
                db2.INSERT_SOLICITUD_PERMISOINASISTENCIA(usuario, Trabajador, FechaInicio, FechaTermino, empresa, Compensado2);
                var SolicitudID = (db2.MAX_ID_SOL_PERMISO.SingleOrDefault());
                f.GenerarNotificacion("PermisoInasistencia", "Solicitud de Permiso " + Persona.NOMBRE, true, "PERMISOINASISTENCIA", Convert.ToInt32(SolicitudID.ID), false, true, Trabajador);
                f.GenerarNotificacion("PermisoInasistencia", "Solicitud de Permiso " + Persona.NOMBRE, false, "PERMISOINASISTENCIA", Convert.ToInt32(SolicitudID.ID), true, false, Trabajador);
                ViewBag.Alert = true;
                //return View(Faenas);
            }
            catch (Exception)
            {
                ViewBag.Alert = false;
            }

            return View();
        }


        [HttpGet]
        public JsonResult ListTrabajadores(int faena)
        {
            List<ElementJsonIntKey> lst = new List<ElementJsonIntKey>();
            lst = (from t in db.CONTRATO
                   where t.FAENA == faena && t.FTERMNO >= DateTime.Now
                   select new ElementJsonIntKey
                   {
                       Value = t.PERSONA,
                       Text = t.PERSONA1.APATERNO + " " + t.PERSONA1.AMATERNO + ", " + t.PERSONA1.NOMBRE + " (" + t.PERSONA1.RUT + ") "
                   }).Distinct().OrderBy(x => x.Text).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CompensadosTrabajador(string Trabajador, int Faena)
        {
            try
            {

                List<ElementJsonIntKey> lst = new List<ElementJsonIntKey>();
                DateTime FechaInicio = Convert.ToDateTime("1900-1-1");
                lst = db2.COMPENSADOS_DISPONIBLES(FechaInicio, Trabajador).Select(x => new ElementJsonIntKey { Value = x.SALDO_COMPENSADOS.ToString(), Text = x.TRABAJADOR }).ToList();
                if (lst.Count == 0)
                {
                    lst.Add(new ElementJsonIntKey { Value = "0", Text = Trabajador });
                }
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e2)
            {
                var Mensaje = e2.Message;
                throw;
            }
        }

        public JsonResult ObtenerDiasPorCompensar(DateTime FechaInicio, DateTime FechaTermino)
        {
            try
            {
                List<ElementJsonIntKey> lst = new List<ElementJsonIntKey>();
                //FechaInicio = FechaInicio.AddDays(-1);
                //FechaTermino = FechaTermino.AddDays(1);

                lst = db2.CALCULA_DIAS_EFECTIVOS_VACACION(FechaInicio, FechaTermino).Select(x => new ElementJsonIntKey { Value = "0", Text = x.Column1.ToString() }).ToList();

                if (lst.Count == 0)
                {
                    lst.Add(new ElementJsonIntKey { Value = "0", Text = "0" });
                }

                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var Mensaje = ex.Message;
                throw;
            }

        }
        public class ElementJsonIntKey
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public ActionResult AutorizacionPermisoInasistencia()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false || f.PermisoPorServicio(112) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            var infopermiso = new List<GYCEmpresa.Models.OFERTA_PERMISOINASISTENCIA>();


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = "display:none";

            DateTime finicio = DateTime.Now.Date;
            DateTime ftermino = DateTime.Now.Date;

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            return View(infopermiso);
        }

        [HttpPost]
        public ActionResult AutorizacionPermisoInasistencia(int faena, DateTime finicio, DateTime ftermino)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            if (f.PoseePermiso(102) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }
            if (f.PoseePermiso(112) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }
            var permi = new List<GYCEmpresa.Models.OFERTA_PERMISOINASISTENCIA>();

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;
            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");
            DateTime diasig = ftermino.Date.AddDays(1);
            ViewBag.Visible = " ";
            var Trabajadores = f.filtroTrabajadoresVigencia(faena, Convert.ToBoolean(1));
            PERSONA Pers = new PERSONA();
            var Ofer = new List<GYCEmpresa.Models.SOLICITUDPERMISOINASISTENCIA>();
            foreach (var t in Trabajadores)
            {
                Pers = db.PERSONA.Where(x => x.RUT == t.RUT).SingleOrDefault();
                Ofer = db.SOLICITUDPERMISOINASISTENCIA.Where(x => x.TRABAJADOR == t.RUT && x.ESTADO==0 && x.RECHAZADA==false && x.FECHA >= finicio && x.FECHA < diasig).ToList();
                foreach(var p in Ofer)
                {
                    permi.Add(new OFERTA_PERMISOINASISTENCIA()
                    { RUT = p.TRABAJADOR,
                        NOMBRE = Pers.APATERNO + " " + Pers.AMATERNO + " " + Pers.NOMBRE,
                        CARGO = "",
                        COMPENSADO = (int)p.COMPENSADO,
                        FECHA_SOLICITUD = Convert.ToString(p.FECHA.Date),
                        FINICIO = p.FINICIO.ToString("dd'-'MM'-'yyyy' 'HH':'mm"),
                         FTERMINO = p.FTERMINO.ToString("dd'-'MM'-'yyyy' 'HH':'mm"),
                        ID = p.ID,
                        COMPENSADO1 = 0
                    })  ;

                }
            }
            return View(permi);
        }

        public virtual JsonResult AutorizaPermisoInasistencia(int ID, int GOCE, int COMPENSADOS)
        {
            try
            {
                string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

                if (f.PoseePermiso(102) != 14 && f.PoseePermiso(112) != 14 && f.isLogged())
                {
                    return Json(new
                    {
                        Mensaje = "No posee permisos para autorizar",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {

                    var Solicitud = (from sol in db.SOLICITUDPERMISOINASISTENCIA
                                     where sol.ID == ID
                                     select sol).SingleOrDefault();

                    var SolicitudValida = db2.VALIDA_DIAS_PERMISO_INASISTENCIA(Solicitud.FINICIO, Solicitud.FTERMINO, Solicitud.TRABAJADOR).Select(x => x.Column1).SingleOrDefault();
                    if (SolicitudValida.Value == 0)
                    {
                        return Json(new
                        {
                            Mensaje = "El rango de fechas de la solicitud incluye dias que el trabajador no registra turno. No es posible autorizar esta solicitud.",
                            redirectURL = Url.Action("Index", "Home"),
                            isRedirect = true
                        });
                    }

                    string PuedeRegistrar = db2.FILTRO_FECHAS_SOLICITUDES(Solicitud.TRABAJADOR, Solicitud.FINICIO, Solicitud.FTERMINO, "P").Select(x => x.Column1).SingleOrDefault();

                    if (PuedeRegistrar != "OK")
                    {
                        string Mensaje = string.Empty;

                        if (PuedeRegistrar == "VACACION")
                        {
                            Mensaje = "Ya existe un periodo de Vacaciones registrado en el rango de fechas seleccionado para este trabajador";
                        }
                        if (PuedeRegistrar == "LICENCIA")
                        {
                            Mensaje = "Ya existe una Licencia Médica registrado en el rango de fechas seleccionado para este trabajador";
                        }
                        if (PuedeRegistrar == "PERMISO")
                        {
                            Mensaje = "Ya existe un Permiso de Inasistencia registrado en el rango de fechas seleccionado para este trabajador";
                        }

                        return Json(new
                        {
                            Mensaje = Mensaje,
                            redirectURL = Url.Action("AutorizacionPermiso", "AsAdminist"),
                            isRedirect = true
                        });

                    }


                    f.GenerarNotificacion("PERMISO INASISTENCIA", "Solicitud autorizada", false, "PERMISOINASISTENCIA", Convert.ToInt32(Solicitud.ID), false, true, Solicitud.TRABAJADOR);
                        f.GenerarNotificacion("PERMISO INASISTENCIA", "Solicitud autorizada Trabajador: " + Solicitud.TRABAJADOR, true, "PERMISOINASISTENCIA", Convert.ToInt32(Solicitud.ID), false, false, Solicitud.TRABAJADOR);
                        bool Goce_Sueldo = false;
                        if (GOCE == 1)
                        {
                            Goce_Sueldo = true;
                        }

                        //ANALISIS DE SOLICITUD DE PERMISO ***************************************
                        int DiffDias = Solicitud.FTERMINO.DayOfYear - Solicitud.FINICIO.DayOfYear + 1;
                        bool PermisoTodoDia = false;

                        if (DiffDias != 0)
                        {
                            int DiaSemanaFin = Convert.ToInt32(Solicitud.FTERMINO.DayOfWeek);
                            var HoraFinTurno = (from hini in db2.TURNO_TRABAJADOR
                                                where hini.RUT == Solicitud.TRABAJADOR && hini.DIA == DiaSemanaFin
                                                select new
                                                {
                                                    HORASALIDA = hini.HORA_FIN
                                                }).SingleOrDefault();

                            PermisoTodoDia = true;
                            DiffDias = DiffDias - 1;

                            DateTime FechaPermiso = DateTime.Parse(Solicitud.FINICIO.ToShortDateString());
                            DateTime FechaHoraPermiso = FechaPermiso.AddHours(Convert.ToInt32(Solicitud.FINICIO.Hour.ToString()));

                            for (int i = 0; i <= DiffDias; i++)
                            {
                            int HoraInicio = 8;
                            int HoraFin = 17;

                            var tipotur = (from tt in db.remepage where tt.nom_tabla == "TURNO" && tt.val_param == empresa select tt).SingleOrDefault();
                                if(tipotur == null)
                                {
                                    var HorasTurno = (from ht in db2.TURNO_TRABAJADOR
                                    where ht.RUT == Solicitud.TRABAJADOR && ht.DIA == Convert.ToInt32(FechaPermiso.DayOfWeek)
                                          select ht).SingleOrDefault();
                                      HoraInicio = TimeSpan.Parse(HorasTurno.HORA_INICIO.ToString()).Hours;
                                      HoraFin = TimeSpan.Parse(HorasTurno.HORA_FIN.ToString()).Hours;
                                }
                           if (i == 0)
                                {
                                    HoraInicio = Solicitud.FINICIO.Hour;
                                }
                                if (i > 0)
                                    FechaPermiso = FechaPermiso.AddDays(1);

                                if (COMPENSADOS > 0)
                                {
                                    db2.AUTORIZA_PERMISOINASISTENCIA(Solicitud.TRABAJADOR, FechaPermiso.AddHours(HoraInicio), FechaPermiso.AddHours(HoraFin), Goce_Sueldo, 1);
                                    COMPENSADOS--;
                                }
                                else
                                {
                                    db2.AUTORIZA_PERMISOINASISTENCIA(Solicitud.TRABAJADOR, FechaPermiso.AddHours(HoraInicio), FechaPermiso.AddHours(HoraFin), Goce_Sueldo, 0);
                                }

                            }
                            //PERMISO POR DIA PARCIAL
                            if (DiffDias != 0)
                            {
                                var HorasTurno = (from ht in db2.TURNO_TRABAJADOR
                                                  where ht.RUT == Solicitud.TRABAJADOR && ht.DIA == Convert.ToInt32(Solicitud.FTERMINO.DayOfWeek)
                                                  select ht).SingleOrDefault();

                                int HoraInicio = TimeSpan.Parse(HorasTurno.HORA_INICIO.ToString()).Hours;
                                int HoraFin = TimeSpan.Parse(HorasTurno.HORA_FIN.ToString()).Hours;

                                if (HoraFin > Solicitud.FTERMINO.Hour)
                                {
                                    HoraFin = Solicitud.FTERMINO.Hour;
                                }

                                if (COMPENSADOS > 0)
                                {
                                    db2.AUTORIZA_PERMISOINASISTENCIA(Solicitud.TRABAJADOR, DateTime.Parse(Solicitud.FTERMINO.ToShortDateString()).AddHours(HoraInicio), DateTime.Parse(Solicitud.FTERMINO.ToShortDateString()).AddHours(HoraFin), Goce_Sueldo, 1);
                                    COMPENSADOS--;
                                }
                                else
                                {
                                    db2.AUTORIZA_PERMISOINASISTENCIA(Solicitud.TRABAJADOR, DateTime.Parse(Solicitud.FTERMINO.ToShortDateString()).AddHours(HoraInicio), DateTime.Parse(Solicitud.FTERMINO.ToShortDateString()).AddHours(HoraFin), Goce_Sueldo, 0);
                                }

                            }
                        }
                        else
                        {
                            var HorasTurno = (from ht in db2.TURNO_TRABAJADOR
                                              where ht.RUT == Solicitud.TRABAJADOR && ht.DIA == Convert.ToInt32(Solicitud.FTERMINO.DayOfWeek)
                                              select ht).SingleOrDefault();

                            int HoraInicio = TimeSpan.Parse(HorasTurno.HORA_INICIO.ToString()).Hours;
                            int HoraFin = TimeSpan.Parse(HorasTurno.HORA_FIN.ToString()).Hours;

                            if (Solicitud.FINICIO.Hour > HoraInicio)
                            {
                                HoraInicio = Solicitud.FINICIO.Hour;
                            }
                            if (HoraFin > Solicitud.FTERMINO.Hour)
                            {
                                HoraFin = Solicitud.FTERMINO.Hour;
                            }
                            if (COMPENSADOS > 0)
                            {
                                db2.AUTORIZA_PERMISOINASISTENCIA(Solicitud.TRABAJADOR, DateTime.Parse(Solicitud.FINICIO.ToShortDateString()).AddHours(HoraInicio), DateTime.Parse(Solicitud.FTERMINO.ToShortDateString()).AddHours(HoraFin), Goce_Sueldo, 1);
                            }
                            else
                            {
                                db2.AUTORIZA_PERMISOINASISTENCIA(Solicitud.TRABAJADOR, DateTime.Parse(Solicitud.FINICIO.ToShortDateString()).AddHours(HoraInicio), DateTime.Parse(Solicitud.FTERMINO.ToShortDateString()).AddHours(HoraFin), Goce_Sueldo, 0);
                            }

                        }


                        Solicitud.ESTADO = 1;
                        db.SaveChanges();


                        return Json(new
                        {
                            Mensaje = "Solicitud autorizada exitosamente",
                            redirectURL = Url.Action("AutorizacionPermisoInasistencia", "AsAdminist"),
                            isRedirect = true
                        });
                    
                }
            }
            catch (Exception e2)
            {
                return Json(new
                {
                    Mensaje = e2.Message
                });
            }
        }

        public virtual JsonResult RechazaPermisoInasistencia(string RUT, string FINICIO, string FTERMINO)
        {
            try
            {
                int ind1 = f.PoseePermiso(102);
                int ind2 = f.PoseePermiso(112);
               

                //if (!(f.PoseePermiso(102) == 14 && f.PoseePermiso(112) == 14 && f.isLogged()))
                if (!(f.PoseePermiso(102) == 14 && f.PoseePermiso(112) == 14 ))
                    {
                        return Json(new
                    {
                        Mensaje = "No posee permisos para rechazar solicitudes",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {

                    var SolicitudID = (db2.MAX_ID_SOL_PERMISO.SingleOrDefault());

                    var Trabajador = (from trab in db.PERSONA
                                      where trab.RUT == RUT
                                      select new
                                      {
                                          TRABAJADOR = trab.RUT + " - " + trab.APATERNO + ", " + trab.NOMBRE
                                      }).SingleOrDefault();

                    f.GenerarNotificacion("PERMISO INASISTENCIA", "Solicitud rechazada", false, "PERMISOINASISTENCIA", Convert.ToInt32(SolicitudID.ID), false, true, RUT);
                    f.GenerarNotificacion("PERMISO INASISTENCIA", "Solicitud rechazada Trabajador: " + Trabajador.TRABAJADOR, true, "PERMISOINASISTENCIA", Convert.ToInt32(SolicitudID.ID), false, false, RUT);
                    db2.RECHAZA_PERMISOINASISTENCIA(RUT, Convert.ToDateTime(FINICIO), Convert.ToDateTime(FTERMINO));
                    return Json(new
                    {
                        Mensaje = "Solicitud rechazada exitosamente",
                        redirectURL = Url.Action("AutorizacionPermisoInasistencia", "AsAdminist"),
                        isRedirect = true
                    });
                }
            }
            catch (Exception e2)
            {
                return Json(new
                {
                    Mensaje = e2.Message
                });
            }
        }

        public ActionResult HorasExtraPendientes()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(105) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            List<SOLICITUDHHEE> hhee = db.SOLICITUDHHEE.Where(item => item.EMPRESA == empresa && item.AUTORIZAEMPRESA == null).ToList();

            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();

            ViewBag.Faenas = FaenaSL;
            return View(hhee);
        }

        [HttpPost]
        public ActionResult HorasExtraPendientes(string firmaText, int idsolicitud, string submit)
        {

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            if (f.PoseePermiso(105) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }


            SOLICITUDHHEE shhee = db.SOLICITUDHHEE.Where(v => v.ID == idsolicitud).FirstOrDefault();

            string mensajeNotificacion = "Empresa : " + System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();


            if (submit == "Firmar")
            {
                shhee.AUTORIZAEMPRESA = true;
                if (shhee.AUTORIZATRABAJADOR == true)
                {
                    f.GenerarNotificacion("Empresa SOLICITUD HHEE AUTORIZADO", mensajeNotificacion, true, "HHEE", shhee.ID, true, false, null);
                }
                else
                {
                    f.GenerarNotificacion("Empresa SOLICITUD HHEE firmado", mensajeNotificacion, true, "HHEE", shhee.ID, true, false, null);
                }

                ViewBag.Resultado = "SOLICITUD HHEE firmado";
            }
            else
            {
                shhee.AUTORIZAEMPRESA = false;
                f.GenerarNotificacion("Empresa SOLICITUD HHEE rechazado", mensajeNotificacion, true, "HHEE", shhee.ID, true, false, null);

                ViewBag.Resultado = "Solicitud HHEE rechazado";
            }

            db.SaveChanges();


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            List<SOLICITUDHHEE> hhee = db.SOLICITUDHHEE.Where(item => item.EMPRESA == empresa && item.AUTORIZAEMPRESA == null).ToList();

            return View(hhee);
        }

        public ActionResult SolicitudHHEE()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(105) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            return View();
        }

        [HttpPost]
        public ActionResult SolicitudHHEE(SOLICITUDHHEE solicitud)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            if (f.PoseePermiso(105) < 13) { return Redirect("~/Home/ErrorPorPermiso"); }


            solicitud.FECHA = DateTime.Now;
            solicitud.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"].ToString();
            solicitud.AUTORIZAEMPRESA = false;
            solicitud.AUTORIZATRABAJADOR = false;
            solicitud.EMPRESA = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            solicitud.ESTADO = 0;

            db.SOLICITUDHHEE.Add(solicitud);
            db.SaveChanges();


            f.GenerarNotificacion("Solicitud HHEE", "Solicitud HHEE Trabajador: " + solicitud.TRABAJADOR, true, "HHEE", solicitud.ID, false, true, solicitud.TRABAJADOR);

            return View();
        }

        public ActionResult GetPDFv(int IDsolicitud)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            var PDF = db.SOLICITUDVACACIONES.Where(v => v.ID == IDsolicitud).First();

            MemoryStream ms = new MemoryStream();
            ms.Write(PDF.PDF.ToArray(), 0, PDF.PDF.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "Application/pdf");

        }

        public ActionResult GetPDFhe(int IDsolicitud)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            var PDF = db.SOLICITUDHHEE.Where(v => v.ID == IDsolicitud).First();

            MemoryStream ms = new MemoryStream();
            ms.Write(PDF.PDF.ToArray(), 0, PDF.PDF.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "Application/pdf");

        }


        public ActionResult EstadoTicket()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> EstadoSolicitud = new List<SelectListItem>
                {
                new SelectListItem() { Text = "Abierto" , Value = "0" } ,
                new SelectListItem() { Text = "Aprobado" , Value = "1" } ,
                new SelectListItem() { Text = "Anulado" , Value = "2" }
                };

            ViewBag.Estado = new SelectList(EstadoSolicitud, "Value", "Text");

            ViewBag.Visible = "display:none";

            DateTime finicio = DateTime.Now.AddDays(-1).AddHours(-DateTime.Now.Hour);
            DateTime ftermino = DateTime.Now;

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            string Jornada = string.Empty;

            return View(db2.REPORTE_SOLICITUDES("1", 0, 0, finicio, ftermino).ToList());
        }

        [HttpPost]
        public ActionResult EstadoTicket(DateTime finicio, DateTime ftermino, int estado, int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            //var Faenas = (from FAENA in db.FAENA
            //              where FAENA.EMPRESA == empresa
            //              select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> EstadoSolicitud = new List<SelectListItem>
                {
                new SelectListItem() { Text = "Abierto" , Value = "0" } ,
                new SelectListItem() { Text = "Aprobado" , Value = "1" } ,
                new SelectListItem() { Text = "Anulado" , Value = "2" }
                };

            ViewBag.Estado = new SelectList(EstadoSolicitud, "Value", "Text");

            ViewBag.Visible = " ";


            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");


            return View(db2.REPORTE_SOLICITUDES(empresa, estado, faena, finicio, ftermino).ToList());
        }

        public virtual JsonResult VisualizaTicket(string Tipo, int id)
        {
            try
            {
                //VISUALIZAR TICKET SELECCIONADO
                return Json(new
                {
                    Mensaje = ""
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

        public ActionResult ReporteColacionesDiarias()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            ViewBag.Visible = "display:none";

            DateTime finicio = DateTime.Now.AddDays(-1).AddHours(-DateTime.Now.Hour);
            DateTime ftermino = DateTime.Now;


            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            return View();
        }

        [HttpPost]
        public ActionResult ReporteColacionesDiarias(DateTime finicio, DateTime ftermino)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            ViewBag.Visible = "";


            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            ftermino = ftermino.AddDays(1);

            var Totales = (from totales in db.COLACIONESDIARIAS
                           where totales.EMPRESA == empresa && totales.FECHA <= ftermino && totales.FECHA >= finicio
                           group totales by totales.FAENA1.DESCRIPCION into GrupoPorFaena
                           select new
                           {
                               FAENA = GrupoPorFaena.Key,
                               CANTIDAD = GrupoPorFaena.Sum(x => x.CANTIDAD),
                               DOTACION = GrupoPorFaena.Sum(x => x.DOTACION_DIARIA)
                           }).ToList().ToArray();

            ViewBag.Totales = Totales;

            var Detalle = (from col in db.COLACIONESDIARIAS
                           from ea in db.EMPRESA
                           where col.EMPRESA_ALIMENTACION == ea.RUT
                           && col.EMPRESA == empresa && col.FECHA >= finicio && col.FECHA <= ftermino
                           select new
                           {
                               col.FECHA,
                               FAENA = col.FAENA1.DESCRIPCION,
                               col.CANTIDAD,
                               EMP_ALIMENTACION = ea.RSOCIAL,
                               DOTACION = col.DOTACION_DIARIA
                           }).ToList().ToArray().OrderBy(x => x.FECHA);

            ViewBag.Detalle = Detalle;

            return View();
        }


        public ActionResult ConsultaPermisoInasistencia(string RUT, DateTime FINICIO, DateTime FTERMINO, string empresa)
        {
                var persona = (db.PERSONA.Where(x => x.RUT == RUT)).SingleOrDefault();
                string nom = persona.APATERNO + " " + persona.AMATERNO + " " + persona.NOMBRE;

                var querySolicitudes = (from spi in db.SOLICITUDPERMISOINASISTENCIA
                                        where spi.EMPRESA == empresa && ((spi.FINICIO >= FINICIO && spi.FINICIO < FTERMINO) || (spi.FTERMINO >= FINICIO && spi.FTERMINO < FTERMINO))
                                        join per in db.PERSONA on spi.TRABAJADOR equals per.RUT
                                        select new
                                        {
                                            ID = spi.ID,
                                            FECHA = spi.FECHA,
                                            FINICIO = spi.FINICIO,
                                            FTERMINO = spi.FTERMINO,
                                            AUTORIZAEMPRESA = spi.AUTORIZAEMPRESA,
                                            AUTORIZATRABAJADOR = spi.AUTORIZATRABAJADOR,
                                            COMPENSADO = spi.COMPENSADO,
                                            TRABAJADOR = per.NOMBRE + " " + per.APATERNO + " " + per.AMATERNO,
                                            MOTIVO = spi.OBSERVACION

                                        }).ToList();


                List<DisplayInasistencias> di = new List<DisplayInasistencias>();
                foreach (var v in querySolicitudes)
                {
                    di.Add(new DisplayInasistencias()
                    {
                        ID = v.ID,
                        FECHA = v.FECHA,
                        FINICIO = v.FINICIO,
                        FTERMINO = v.FTERMINO,
                        AUTORIZAEMPRESA = v.AUTORIZAEMPRESA,
                        AUTORIZATRABAJADOR = v.AUTORIZATRABAJADOR,
                        COMPENSADO = v.COMPENSADO,
                        TRABAJADOR = v.TRABAJADOR,
                        MOTIVO = v.MOTIVO

                    });
                }

                return View(di);
        }



        public ActionResult decisionInasistencia(bool autoriza, int idSolicitud)
        {
            SOLICITUDPERMISOINASISTENCIA solicitud = db.SOLICITUDPERMISOINASISTENCIA.Where(x => x.ID == idSolicitud).FirstOrDefault();
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            if (solicitud.EMPRESA == empresa)
            {
                if (autoriza == true)
                {
                    solicitud.AUTORIZAEMPRESA = true;
                }
                else
                {
                    solicitud.AUTORIZAEMPRESA = false;
                }

                db.SaveChanges();
            }

            return Redirect("~/AsAdminist/ConsultaPermisoInasistencias");
        }

    }


}


public class DisplayInasistencias
{
    public int ID { get; set; }
    public DateTime FECHA { get; set; }
    public DateTime FINICIO { get; set; }
    public DateTime FTERMINO { get; set; }
    public bool? AUTORIZAEMPRESA { get; set; }
    public bool? AUTORIZATRABAJADOR { get; set; }
    public int? COMPENSADO { get; set; }
    public string TRABAJADOR { get; set; }
    public string MOTIVO { get; set; }

}
namespace GYCEmpresa.Models
{
    public  class OFERTA_PERMISOINASISTENCIA
  {

    public string RUT { get; set; }

    public string NOMBRE { get; set; }

    public string CARGO { get; set; }

    public int COMPENSADO { get; set; }

    public string FECHA_SOLICITUD { get; set; }

    public string FINICIO { get; set; }

    public string FTERMINO { get; set; }

    public int ID { get; set; }

    public int COMPENSADO1 { get; set; }

  }
}

