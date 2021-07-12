using Amazon.Runtime.Internal.Transform;
using GYCEmpresa.App_Start;
using GYCEmpresa.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using Microsoft.Ajax.Utilities;
using System.Reflection.Emit;
using SpreadsheetLight;

namespace GYCEmpresa.Controllers
{
    public class ControlAsistenciaController : Controller
    {

        dbgycEntities3 db = new dbgycEntities3();
        dbgyc2DataContext db2 = new dbgyc2DataContext();

        Funciones f = new Funciones();

        // GET: ControlAsistencia
        public ActionResult Index()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            return View();
        }

        public ActionResult SeleccionarFaena()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            List<FAENA> faenas = db.FAENA.Where(item => item.EMPRESA == empresa).ToList();
            return View(faenas);
        }

        public ActionResult Marcaciones(int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            ViewBag.Empresa = empresa;

            List<MarcacionModel> listaMarcaciones = new List<MarcacionModel>();

            var Faena = (from FAENA in db.FAENA
                         where FAENA.EMPRESA == empresa
                         orderby FAENA.DESCRIPCION
                         select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();


            var Trabajadores = (from CONTRATO in db.CONTRATO
                                where CONTRATO.FAENA == faena
                                select new
                                {
                                    Rut = CONTRATO.PERSONA
                                }).ToList();



            var FaenaSL = new SelectList(Faena, "Id", "Descripcion");

            ViewBag.Faenas = FaenaSL;
            ViewBag.faenaSelected = faena;
            ViewBag.nombreFaena = db.FAENA.Where(item => item.ID == faena).First().DESCRIPCION;

            return View(listaMarcaciones);
        }

        [HttpPost]
        public ActionResult Marcaciones(DateTime? finicio, DateTime? ftermino, int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            ViewBag.Empresa = empresa;
            ViewBag.nombreFaena = db.FAENA.Where(item => item.ID == faena).First().DESCRIPCION;

            List<MarcacionModel> listaMarcaciones = new List<MarcacionModel>();

            var Trabajadores = (from CONTRATO in db.CONTRATO
                                where CONTRATO.FAENA == faena
                                select new
                                {
                                    Rut = CONTRATO.PERSONA
                                }).ToList();


            ftermino = ftermino.Value.AddDays(1);

            foreach (var trabajador in Trabajadores)
            {
                MarcacionModel trabajadorActual = new MarcacionModel();
                trabajadorActual.RUT = trabajador.Rut;
                var nombres = db.PERSONA.Where(item => item.RUT == trabajador.Rut).Select(e => new
                {
                    e.NOMBRE,
                    e.APATERNO,
                    e.AMATERNO
                }).First();
                trabajadorActual.NOMBRES = nombres.NOMBRE + " " + nombres.APATERNO + " " + nombres.AMATERNO;

                trabajadorActual.marcs = db.MARCACIONyRUT.Where(item => item.RUT == trabajador.Rut && item.CHECKTIME > finicio && item.CHECKTIME <= ftermino).ToList();
                trabajadorActual.horasExtra = db.HHEE.Where(item => item.TRABAJADOR == trabajador.Rut).ToList();
                listaMarcaciones.Add(trabajadorActual);
            }

            return View(listaMarcaciones);


        }

        public ActionResult HorasExtraAutorizadas()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(105) == false && f.PermisoPorServicio(113) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var Faena = (from FAENA in db.FAENA
                         where FAENA.EMPRESA == empresa
                         orderby FAENA.DESCRIPCION
                         select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();



            var FaenaSL = new SelectList(Faena, "Id", "Descripcion");
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> TrabajadorList = new List<SelectListItem>();

            TrabajadorList.Add(new SelectListItem() { Text = "Seleccione Trabajador", Value = "1" });

            var TrabajadoresSL = new SelectList(TrabajadorList, "Value", "Text");
            ViewBag.Trabajadores = TrabajadoresSL;
            ViewBag.vigenteSelected = 1;

            ViewBag.FechaInicio = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaFin = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.Fecha1 = DateTime.Now.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fecha2= DateTime.Now.ToString("dd'-'MM'-'yyyy");

            ViewBag.Visible = "display:none";

            return View(db2.HHEE_AUTORIZADAS("X", 0, DateTime.Now, DateTime.Now));
        }

        [HttpPost]
        public ActionResult HorasExtraAutorizadas(int faena, string trabajador, DateTime fechaInicio, DateTime fechaFin, string todos, int vigentes, string buttonsubmit)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            //if (f.PermisoPorServicio(105) == false)
            //{
            //    return Redirect("~/Home/ErrorPorPermiso");
            //}
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            ViewBag.vigenteSelected = Convert.ToBoolean(vigentes) ? 1 : 0;

            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(fechaInicio, fechaFin, faena);
                if (fechaInicio < Fechasfaena[0])
                    fechaInicio = Fechasfaena[0];
                if (fechaFin > Fechasfaena[1])
                    fechaFin = Fechasfaena[1];
            }


            ViewBag.FechaInicio = fechaInicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaFin = fechaFin.ToString("yyyy'-'MM'-'dd");
            ViewBag.Fecha1 = fechaInicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fecha2 =fechaFin.ToString("dd'-'MM'-'yyyy");


            ViewBag.Visible = "";

            if (trabajador != "1" /*&& buttonsubmit != null*/)
            {
                if (todos == "true")
                {
                    trabajador = "";
                }
                return View(db2.HHEE_AUTORIZADAS(trabajador, faena, fechaInicio, fechaFin));
            }

            return View(db2.HHEE_AUTORIZADAS("X", 0, DateTime.Now, DateTime.Now));

        }

        public ActionResult HorasDescuento()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            List<HorasDescuento> Resultado = new List<HorasDescuento>();

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var Faena = (from FAENA in db.FAENA
                         where FAENA.EMPRESA == empresa
                         orderby FAENA.DESCRIPCION
                         select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            //var Faena = f.FaenasEmpresa();
            var FaenaSL = new SelectList(Faena, "Id", "Descripcion");
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> TrabajadorList = new List<SelectListItem>();
            TrabajadorList.Add(new SelectListItem() { Text = "Seleccione Trabajador", Value = "1" });
            var TrabajadoresSL = new SelectList(TrabajadorList, "Value", "Text");
            ViewBag.Trabajadores = TrabajadoresSL;
 
            ViewBag.vigenteSelected = 1;
            ViewBag.FechaInicio = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaFin = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaMostrar1 = DateTime.Now.Date.ToString("dd'-'MM'-'yyyy");
            ViewBag.FechaMostrar2 = DateTime.Now.Date.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fechamala = "N";

            ViewBag.Visible = "display:none";

            return View(Resultado);
        }

        [HttpPost]
        public ActionResult HorasDescuento(int faena, string trabajador, DateTime fechaInicio, DateTime fechaFin, string todos, int vigentes, string submitbutton)
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;


            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            ViewBag.vigenteSelected = Convert.ToBoolean(vigentes) ? 1 : 0;

            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(fechaInicio, fechaFin, faena);
                if (fechaInicio < Fechasfaena[0])
                    fechaInicio = Fechasfaena[0];
                if (fechaFin > Fechasfaena[1])
                    fechaFin = Fechasfaena[1];
            }
            remepage dettiemp = new remepage();
            int holguraent;
            var infoferia = new List<GYCEmpresa.Models.remepage>();
            infoferia = db.remepage.Where(x => x.nom_tabla == "FERIADOS" && x.fec_param >= fechaInicio && x.fec_param <= fechaFin).ToList();
            var infoTiemp = new List<GYCEmpresa.Models.remepage>();
            infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
            dettiemp = infoTiemp.Where(x => "1         " == x.cod_param).SingleOrDefault();
            holguraent = 0;
            if (dettiemp != null)
                holguraent = Convert.ToInt32(dettiemp.val_param);


            ViewBag.FechaInicio = fechaInicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaFin = fechaFin.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaMostrar1 = fechaInicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.FechaMostrar2 = fechaFin.ToString("dd'-'MM'-'yyyy");

            if (fechaInicio > fechaFin)
            {
                ViewBag.Fechamala = "S";
                return View(db2.DETALLE_HHDESCUENTO(DateTime.Now, DateTime.Now, 0, "X"));
            }
            else
            {
                ViewBag.Fechamala = "N";
            }

            List<HorasDescuento> Resultado = new List<HorasDescuento>();
            List<DetalleDias> Diastrabajador = new List<DetalleDias>();
            PERSONA Pers = new PERSONA();
            if(todos == "true")
            {
                var Trabajadores = f.filtroTrabajadoresVigencia(faena, Convert.ToBoolean(vigentes));
                ViewBag.Visible = "";
                foreach (var item in Trabajadores)
                {
                    Diastrabajador = AsistenciaDiaria(empresa,item.RUT, fechaInicio, fechaFin, faena, infoferia, holguraent);
                    Pers = db.PERSONA.Where(x => x.RUT == item.RUT).SingleOrDefault();
                    foreach (var d in Diastrabajador)
                    {
                      if(d.M_TARDA > 0)
                      {
                        int horas = d.M_TARDA / 60;
                        int minut = d.M_TARDA - horas * 60;
                        string hhdd = horas.ToString("0#") + ':' + minut.ToString("0#");
                        string marca = d.H_ENTRADA.ToString("HH':'mm");
                        Resultado.Add(new HorasDescuento()
                        {
                        RUT = d.RUT,
                        NOMBRE = Pers.APATERNO+" "+Pers.AMATERNO+" "+Pers.NOMBRE,
                        FECHA = d.FECHA.ToString("dd'-'MM'-'yyyy"),
                        MARCACION = marca,
                        HENTRADA = d.H_ENTRADA.ToString("dd'-'MM'-'yyyy"),
                        HSALIDA = d.H_SALIDA.ToString("dd'-'MM'-'yyyy"),
                        HENTRADA2 = d.T_ENTRADA,
                        HSALIDA2 = d.T_SALIDA,
                        MINUTOS = Convert.ToString(d.M_TARDA),
                        HHDD = hhdd
                        });
                      }

                    }
                }

            }
            else
            {
                Diastrabajador = AsistenciaDiaria(empresa,trabajador, fechaInicio, fechaFin, faena, infoferia, holguraent);
                Pers = db.PERSONA.Where(x => x.RUT == trabajador).SingleOrDefault();
                foreach (var d in Diastrabajador)
                {
                    if (d.M_TARDA > 0)
                    {
                        int horas = d.M_TARDA / 60;
                        int minut = d.M_TARDA - horas * 60;
                        string hhdd = horas.ToString("0#") + ':' + minut.ToString("0#");
                        string marca = d.H_ENTRADA.ToString("HH':'mm");
                        Resultado.Add(new HorasDescuento()
                        {
                            RUT = d.RUT,
                            NOMBRE = Pers.APATERNO + " " + Pers.AMATERNO + " " + Pers.NOMBRE,
                            FECHA = d.FECHA.ToString("dd'-'MM'-'yyyy"),
                            MARCACION = marca,
                            HENTRADA = d.H_ENTRADA.ToString("dd'-'MM'-'yyyy"),
                            HSALIDA = d.H_SALIDA.ToString("dd'-'MM'-'yyyy"),
                            HENTRADA2 = d.T_ENTRADA,
                            HSALIDA2 = d.T_SALIDA,
                            MINUTOS = Convert.ToString(d.M_TARDA),
                            HHDD = hhdd
                        });
                    }

                }
            }
            return View(Resultado);
        }

        public ActionResult MarcacionesTrabajadores()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string trabaj = System.Web.HttpContext.Current.Session["sessionTrabajador"] as String;

            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            if (trabaj == "N")
            {
                var Trabajadores = f.filtroTrabajadoresVigencia(0, Convert.ToBoolean(1));
                var TrabajadoresSL = new SelectList(Trabajadores.OrderBy(x => x.NOMBRE), "RUT", "NOMBRE");
                ViewBag.Trabajadores = TrabajadoresSL;
                ViewBag.vigenteSelected = 1;
           }
            else
            {
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
                var Trabajadores = (db.PERSONA.Where(x => x.RUT == usuario)).SingleOrDefault();
                ViewBag.Trabajadores = Trabajadores;
                ViewBag.vigenteSelected = 1;

                ViewBag.Nombre = "Trabajador " + Trabajadores.APATERNO + " " + Trabajadores.AMATERNO + " " + Trabajadores.NOMBRE;

            }


            ViewBag.vigenteSelected = 1;

            ViewBag.FechaInicio = DateTime.Now.Date.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaFin = DateTime.Now.Date.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaMostrar1 = DateTime.Now.Date.ToString("dd'-'MM'-'yyyy");
            ViewBag.FechaMostrar2 = DateTime.Now.Date.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fechamala = "N";

            ViewBag.Visible = "display:none";

            return View(db.JVC_MARCACIONES(DateTime.Now, DateTime.Now, 0, "0", empresa));
        }

        [HttpPost]
        public ActionResult MarcacionesTrabajadores(string vigentes, string faena, string trabajador, DateTime fechaInicio, DateTime fechaFin, string todos, string buttonsubmit)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            int vigentesi = Convert.ToInt32(vigentes);
            int faenai = Convert.ToInt32(faena);
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var FaenaSL = f.FaenasEmpresaTodos(vigentesi);
            ViewBag.Faenas = FaenaSL;

            ViewBag.vigenteSelected = Convert.ToBoolean(  vigentesi) ? 1 : 0;
            if (faenai > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(fechaInicio, fechaFin, faenai);
                if (fechaInicio < Fechasfaena[0])
                    fechaInicio = Fechasfaena[0];
                if (fechaFin > Fechasfaena[1])
                    fechaFin = Fechasfaena[1];
            }
            if (fechaInicio > fechaFin)
            {
                ViewBag.Fechamala = "S";
                return View(db.JVC_MARCACIONES(DateTime.Now, DateTime.Now, 0, "0", empresa));
            }
            else
            {
                ViewBag.Fechamala = "N";
            }



            ViewBag.FechaInicio = fechaInicio.Date.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaFin = fechaFin.Date.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaMostrar1 = fechaInicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.FechaMostrar2 = fechaFin.ToString("dd'-'MM'-'yyyy");

            //Nuevo
            string trabaj = System.Web.HttpContext.Current.Session["sessionTrabajador"] as String;

            if (trabaj == "N")
            {
                var Trabajadores = f.filtroTrabajadoresVigencia(0, Convert.ToBoolean(1));
                var TrabajadoresSL = new SelectList(Trabajadores.OrderBy(x => x.NOMBRE), "RUT", "NOMBRE");
                ViewBag.Trabajadores = TrabajadoresSL;
                ViewBag.vigenteSelected = 1;
            }
            else
            {
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
                var Trabajadores = (db.PERSONA.Where(x => x.RUT == usuario)).SingleOrDefault();
                ViewBag.Trabajadores = Trabajadores;
                ViewBag.vigenteSelected = 1;
                trabajador = usuario;
                ViewBag.Nombre = "Trabajador " + Trabajadores.APATERNO + " " + Trabajadores.AMATERNO + " " + Trabajadores.NOMBRE;
            }


            ViewBag.Visible = "";

            if (trabajador != "1")
            {
                if (todos == "true")
                {
                    trabajador = "";
                }
                return View(db.JVC_MARCACIONES(fechaInicio, fechaFin, faenai, trabajador, empresa));
            }
            return View(db.JVC_MARCACIONES(DateTime.Now, DateTime.Now, 0, "0", empresa));

        }

        public ActionResult InformeAsistencia()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            if (System.Web.HttpContext.Current.Session["sessionTrabajador"] == "N")
            {
                var Trabajadores = f.filtroTrabajadoresVigencia(0, Convert.ToBoolean(1));
                var TrabajadoresSL = new SelectList(Trabajadores.OrderBy(x => x.NOMBRE), "RUT", "NOMBRE");
                ViewBag.Trabajadores = TrabajadoresSL;
                ViewBag.vigenteSelected = 1;
            }
            else
            {
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
                var Trabajadores = (db.PERSONA.Where(x => x.RUT == usuario)).SingleOrDefault();
                ViewBag.Trabajadores = Trabajadores;
                ViewBag.vigenteSelected = 1;
                ViewBag.Nombre = "Trabajador " + Trabajadores.APATERNO + " " + Trabajadores.AMATERNO + " " + Trabajadores.NOMBRE;
            }

            ViewBag.vigenteSelected = 1;

            ViewBag.FechaInicio = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaFin = DateTime.Now.ToString("yyyy'-'MM'-'dd");

            ViewBag.Visible = "display:none";


            ViewBag.Empresa = string.Empty;
            ViewBag.Trabajador = string.Empty;
            ViewBag.Fechamala = "N";

            //var Resultado = db2.INFORME_MARCACIONES_EMPRESA_FINAL(string.Empty, DateTime.Now, DateTime.Now, string.Empty, 0, 1).ToList();
            List<InformeFinal> Resultado = new List<InformeFinal>();

            return View(Resultado);
        }

        [HttpPost]
        public ActionResult InformeAsistencia(string vigentes, string faena, string trabajador, DateTime fechaInicio, DateTime fechaFin, string todos, string buttonsubmit, string btnPDF)
        {

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            int vigentesi = Convert.ToInt32(vigentes);
            int faenai = Convert.ToInt32(faena);
            List<INFORME_MARCACIONES_EMPRESA_FINALResult> Resultadovacio = new List<INFORME_MARCACIONES_EMPRESA_FINALResult>();
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(vigentesi);
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> TrabajadorList = new List<SelectListItem>();


            // nuevo
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
                trabajador = usuario;
                var persona = (db.PERSONA.Where(x => x.RUT == usuario)).SingleOrDefault();

            List<InformeFinal> Resultado = new List<InformeFinal>();
            List<InformeFinal> TablaTrabajador = new List<InformeFinal>();


            Resultado = InformeIndividual(empresa, trabajador, fechaInicio, fechaFin, faenai);

            ViewBag.Resultado2 = Resultado;
            return View(Resultado);
        }

        public List<InformeFinal> InformeIndividual(string empresa, string trabajador, DateTime fechaInicio, DateTime fechaFin, int faena)
        {
            List<InformeFinal> Resultado = new List<InformeFinal>();
            detcabezera Cabezera = new detcabezera();
            List<DetalleDias> individual = new List<DetalleDias>();
            int horas, minut;
            int holguraent;
            remepage dettiemp = new remepage();
            var infoferia = new List<GYCEmpresa.Models.remepage>();
            infoferia = db.remepage.Where(x => x.nom_tabla == "FERIADOS" && x.fec_param >= fechaInicio && x.fec_param <= fechaFin).ToList();
            var infoTiemp = new List<GYCEmpresa.Models.remepage>();
            infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
            dettiemp = infoTiemp.Where(x => "1         " == x.cod_param).SingleOrDefault();
            holguraent = 0;
            if (dettiemp != null)
                holguraent = Convert.ToInt32(dettiemp.val_param);


            DateTime mindia = Convert.ToDateTime("2000-01-01");
            individual = AsistenciaDiaria(empresa,trabajador, fechaInicio, fechaFin, faena, infoferia,holguraent);

            foreach (var i in individual)
            {
                string tardanza = null, hextras = null;
                DateTime turent = Convert.ToDateTime(i.T_ENTRADA);
                int tar = i.H_ENTRADA.Subtract(turent).Minutes;
                DateTime fec1 = i.FECHA.Date;
                string fechas = fec1.ToString("dd'-'MM'-'yyyy");
                string entrada = "";
                string salida = "";
                

                if (i.H_ENTRADA > mindia) entrada = Convert.ToString(i.H_ENTRADA.TimeOfDay);
                if (i.H_SALIDA > mindia) salida = Convert.ToString(i.H_SALIDA.TimeOfDay);
                if(i.M_EXTRA >0)
                {
                    horas = i.M_EXTRA / 60;
                    minut = i.M_EXTRA - horas * 60;
                    hextras = horas.ToString("0#") +":"+ minut.ToString("0#");
                }
                if (i.M_TARDA > 0)
                {
                    horas = i.M_TARDA / 60;
                    minut = i.M_TARDA - horas * 60;
                    tardanza = horas.ToString("0#") + ":" + minut.ToString("0#");
                }
                if (tar > 0) tardanza = Convert.ToString(tar);
                if (i.M_ENTRADA == null) i.M_ENTRADA = "";
                if (i.M_SALIDA == null) i.M_SALIDA = "";
                if (i.C_INASI == null) i.C_INASI = "";
                if (tardanza == null) tardanza = "";
                if (hextras == null) hextras = "";


                Resultado.Add(new InformeFinal()
                {
                    id = 1,
                    fecha = fechas,
                    diasem = f.Diasem(i.FECHA),
                    inasistencia = i.C_INASI,
                    turnoentrada = i.T_ENTRADA,
                    turnosalida = i.T_SALIDA,
                    marcaentrada = entrada,
                    manualentrada = i.M_ENTRADA,
                    marcasalida = salida,
                    manualsalida = i.M_SALIDA,
                    horasturno = f.Difhoras(Convert.ToDateTime(i.T_ENTRADA), Convert.ToDateTime(i.T_SALIDA)),
                    horastrabajadas = f.Difhoras(Convert.ToDateTime(i.H_ENTRADA), Convert.ToDateTime(i.H_SALIDA)),
                    retrasos = tardanza,
                    sobretiempos = hextras
    });
            }
                return Resultado;
        }
        public detcabezera CabezeraInforme(string empresa, string trabajador, DateTime finicio, DateTime ffinal)
        {
            List<InformeFinal> cabezera = new List<InformeFinal>();
            detcabezera cabeza = new detcabezera();
            if (trabajador==null) return cabeza;

            DateTime hoy = DateTime.Now.Date;
            string fec1 = finicio.ToString("dd'-'MM'-'yyyy");
            string fec2 = ffinal.ToString("dd'-'MM'-'yyyy");
            EMPRESA detemp = db.EMPRESA.Where(x => x.RUT == empresa).SingleOrDefault();
            string razon = detemp.RSOCIAL;
            CONTRATO detcon = db.CONTRATO.Where(x => x.PERSONA == trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA==true && x.FIRMATRABAJADOR==true && x.RECHAZADO==false).SingleOrDefault();
            string cargo = detcon.CARGO;
            DateTime feci = detcon.FINICIO.Date;
            string fecing = feci.ToString("dd'-'MM'-'yyyy");
            PERSONA detper = db.PERSONA.Where(x => x.RUT == trabajador).SingleOrDefault();
            string nombre = detper.APATERNO + " " + detper.AMATERNO + " " + detper.NOMBRE;
            //Titulo Empresa
            cabeza.empresa = razon;
            cabeza.cargo = cargo;
            cabeza.nombre = nombre;
            cabeza.ingreso = feci;
            cabeza.rut = trabajador;
            cabeza.desde = finicio;
            cabeza.hasta = ffinal;


            return cabeza;
        }
        public ActionResult InformePDF(DateTime fechaInicio, DateTime fechaFin, string trabajador, int faena, string todos)
        {

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            return new ActionAsPdf("TablaInforme", new { empresa, fechaInicio, fechaFin, trabajador, faena, todos });
        }

        public ActionResult TablaInforme(string empresa, DateTime fechaInicio, DateTime fechaFin, string trabajador, int faena, string todos)
        {
            if (todos == "true")
            {
                trabajador = "";
            }
            return View(db2.INFORME_MARCACIONES_EMPRESA_FINAL(empresa, fechaInicio, fechaFin, trabajador, 0, 1));
        }

        public JsonResult fetchTrabajadores(int faena)
        {
            var trabajadores = db2.TRABAJADORESVIGENTESPORFAENA(faena).ToList();

            var trabajadoresResultantes = trabajadores.Select(item => new
            {
                NOMBRE = item.NOMBRE,
                APATERNO = item.APATERNO,
                AMATERNO = item.AMATERNO,
                RUT = item.PERSONA,
                FAENA = item.FAENA
            });

            return Json(trabajadoresResultantes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetalleHorariosTrabjadores(int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            ViewBag.Empresa = empresa;

            var Faena = (from FAENA in db.FAENA
                         where FAENA.EMPRESA == empresa
                         orderby FAENA.DESCRIPCION
                         select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();


            var Trabajadores = (from CONTRATO in db.CONTRATO
                                where CONTRATO.FAENA == faena
                                select new
                                {
                                    Rut = CONTRATO.PERSONA
                                }).ToList();

            List<SelectListItem> trab = new List<SelectListItem>();

            foreach (var trabajador in Trabajadores)
            {
                var nombreTrabajador = (from PERSONA in db.PERSONA
                                        where PERSONA.RUT == trabajador.Rut
                                        select new
                                        {
                                            NOMBRES = PERSONA.NOMBRE,
                                            APATERNO = PERSONA.APATERNO,
                                            AMATERNO = PERSONA.AMATERNO
                                        }).First();

                string labelPersona = nombreTrabajador.APATERNO + " " + nombreTrabajador.AMATERNO + " " + nombreTrabajador.NOMBRES;

                SelectListItem itemS = new SelectListItem();
                itemS.Text = labelPersona;
                itemS.Value = trabajador.Rut;

                trab.Add(itemS);
            }
            var listaPersonas = new SelectList(trab, "Value", "Text");
            listaPersonas.OrderBy(item => item.Text);
            var FaenaSL = new SelectList(Faena, "Id", "Descripcion");

            ViewBag.Faenas = FaenaSL;
            ViewBag.ListaTrabajadores = listaPersonas;




            MarcacionModel ta = new MarcacionModel();
            ta.marcs = new List<MARCACIONyRUT>();

            return View(ta);
        }

        [HttpPost]
        public ActionResult DetalleHorariosTrabjadores(DateTime? finicio, DateTime? ftermino, int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            string rut = Request.Form["trabajadores"].ToString();
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            ViewBag.Empresa = empresa;

            var Faena = (from FAENA in db.FAENA
                         where FAENA.EMPRESA == empresa
                         orderby FAENA.DESCRIPCION
                         select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();


            var FaenaSL = new SelectList(Faena, "Id", "Descripcion");
            ViewBag.Faenas = FaenaSL;
            if (finicio == null)
            {
                finicio = new DateTime(1900, 01, 01);
                ftermino = new DateTime(2100, 01, 01);
            }


            var Trabajadores = (from CONTRATO in db.CONTRATO
                                where CONTRATO.FAENA == faena
                                select new
                                {
                                    Rut = CONTRATO.PERSONA
                                }).ToList();

            List<SelectListItem> trab = new List<SelectListItem>();

            foreach (var trabajador in Trabajadores)
            {
                var nombreTrabajador = (from PERSONA in db.PERSONA
                                        where PERSONA.RUT == trabajador.Rut
                                        select new
                                        {
                                            NOMBRES = PERSONA.NOMBRE,
                                            APATERNO = PERSONA.APATERNO,
                                            AMATERNO = PERSONA.AMATERNO
                                        }).First();

                string labelPersona = nombreTrabajador.APATERNO + " " + nombreTrabajador.AMATERNO + " " + nombreTrabajador.NOMBRES;

                SelectListItem itemS = new SelectListItem();
                itemS.Text = labelPersona;
                itemS.Value = trabajador.Rut;

                trab.Add(itemS);
            }

            var listaPersonas = new SelectList(trab, "Value", "Text");
            listaPersonas.OrderBy(item => item.Text);

            ViewBag.ListaTrabajadores = listaPersonas;


            DateTime? Finicio = finicio;
            DateTime? Ftermino = ftermino;

            PERSONA p = db.PERSONA.Where(item => item.RUT == rut).First();


            /*
            var Marcacion = (from MARCACIONRELOJ in db.MARCACIONRELOJ
                             from CONTRATO in db.CONTRATO
                             where
                               MARCACIONRELOJ.TRABAJADOR == CONTRATO.PERSONA &&
                               MARCACIONRELOJ.FECHAHORA >= Finicio && MARCACIONRELOJ.FECHAHORA <= Ftermino &&
                               CONTRATO.PERSONA == rut
                             orderby
                               MARCACIONRELOJ.TRABAJADOR
                             select new
                             {
                                 APATERNO = MARCACIONRELOJ.PERSONA.APATERNO,
                                 AMATERNO = MARCACIONRELOJ.PERSONA.AMATERNO,
                                 NOMBRE = MARCACIONRELOJ.PERSONA.NOMBRE,
                                 RUT = MARCACIONRELOJ.PERSONA.RUT,
                                 CARGO = CONTRATO.CARGO,
                                 MARCACION = MARCACIONRELOJ.FECHAHORA.ToString()
                             }).ToList();

            var Marcacion2 = (from marca in db.MARCACIONyRUT
                              from cont in db.CONTRATO
                              where marca.RUT == rut &&
                              marca.CHECKTIME >= Finicio && marca.CHECKTIME <= Ftermino &&
                              marca.RUT == cont.PERSONA
                              select new
                              {
                                  FECHA = marca.CHECKTIME,
                                  HORA = marca.CHECKTIME
                              }).OrderBy(item => item.FECHA).ToList();


            var MarcacionesArray = Marcacion2.ToArray();
            ViewBag.Marcaciones = MarcacionesArray;*/


            //ViewBag.horasExtra = db.HHEE.Where(item => item.TRABAJADOR == rut).ToList().ToArray();
            ViewBag.horasExtra = db.HHEE.Where(item => item.TRABAJADOR == rut).Select(e => new
            {
                e.FINICIO,
                e.FTERMINO,
                e.TRABAJADOR
            }
            ).ToList().ToArray();

            MarcacionModel trabajadorActual = new MarcacionModel();
            trabajadorActual.RUT = rut;
            var nombres = db.PERSONA.Where(item => item.RUT == rut).Select(e => new
            {
                e.NOMBRE,
                e.APATERNO,
                e.AMATERNO
            }).First();
            trabajadorActual.NOMBRES = nombres.NOMBRE + " " + nombres.APATERNO + " " + nombres.AMATERNO;
            ftermino = ftermino.Value.AddDays(1);
            trabajadorActual.marcs = db.MARCACIONyRUT.Where(item => item.RUT == rut && item.CHECKTIME > finicio && item.CHECKTIME < ftermino).ToList();
            trabajadorActual.horasExtra = db.HHEE.Where(item => item.TRABAJADOR == rut).ToList();


            return View(trabajadorActual);
        }

        public ActionResult ResumenHorasTotales()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var nr = new List<ResumenDias>();



            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            ViewBag.FechaInicio = DateTime.Now.AddDays(-1).ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.Fecha1 = DateTime.Now.AddDays(-1).ToString("dd'-'MM'-'yyyy");
            ViewBag.Fecha2 = DateTime.Now.ToString("dd'-'MM'-'yyyy");

            ViewBag.Visible = "display:none";
            try
            {
                ViewBag.TotalTrabajados = 0;
                ViewBag.TotalVacaciones = 0;
                ViewBag.TotalCompensados = 0;
                ViewBag.TotalDescanso = 0;
                ViewBag.TotalPermiso = 0;
                ViewBag.TotalLicencia = 0;
                ViewBag.TotalAusencia =0;
                ViewBag.TotalTotales = 0;
                ViewBag.TotalHHDD = "";
                ViewBag.TotalHHEE = "";
                ViewBag.TotalHHPP = "";
            }
            catch (Exception)
            {
            }
            ViewBag.Fechamala = "N";

            //return View(db2.DIASTOTALES(DateTime.Now.AddDays(-1), DateTime.Now, 0));
            return View(nr);
        }

        [HttpPost]
        public ActionResult ResumenHorasTotales(DateTime finicio, DateTime ftermino, int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var newreg = new List<ResumenDias>();

            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            if (Convert.ToDateTime(ftermino) > DateTime.Now.Date)
            {
                ftermino = DateTime.Now.Date;
            }

            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, faena);
                if (finicio < Fechasfaena[0])
                    finicio = Fechasfaena[0];
                if (ftermino > Fechasfaena[1])
                    ftermino = Fechasfaena[1];
            }

            if (finicio > ftermino)
            {
                ViewBag.Fechamala = "S";
                return View(newreg);
            }
            else
            {
                ViewBag.Fechamala = "N";
            }
            ViewBag.Fecha1 = finicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fecha2 = ftermino.ToString("dd'-'MM'-'yyyy");
            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            ViewBag.Visible = " ";
            try
            {
                var DiasTotales = AsistenciaDiariaFaena(Convert.ToDateTime(finicio), Convert.ToDateTime(ftermino), Convert.ToInt32(faena), 1);
                newreg = DiasTotales;
                int Trabajados = 0;
                int Vacaciones = 0;
                int TotalCompensados = 0;
                int Descanso = 0;
                int Permiso = 0;
                int Licencia = 0;
                int TotalAusencias = 0;
                int Totales = 0;
                int TotalHHEE = 0;
                int TotalHHDD = 0;
                int TotalHHPP = 0;
                int horas, minut;
                string extr, tard,perm;
                string TotHHDD = string.Empty;
                string TotHHEE = string.Empty;
                string TotHHPP = string.Empty;
                string Horas = string.Empty;
                string Minutos = string.Empty;

                foreach (var item in newreg)
                {
                    Trabajados = Trabajados + item.DIAS_TRABAJADOS;
                    Vacaciones = Vacaciones + item.DIAS_VACACIONES;
                    TotalCompensados = TotalCompensados + item.DIAS_COMPENSADOS;
                    Descanso = Descanso + item.DIAS_DESCANSO ;
                    Permiso = Permiso + item.DIAS_PERMISO;
                    Licencia = Licencia + item.DIAS_LICENCIA;
                    Totales = Totales + item.DIAS_TOTALES;
                    TotalAusencias = TotalAusencias + item.DIAS_AUSENCIA;
                    TotalHHEE = TotalHHEE + item.HHEE;
                    TotalHHDD = TotalHHDD + item.HDESCUENTO;
                    TotalHHPP = TotalHHPP + item.HORAS_PERMISO;


                }
                //TOTAL HORA DE DESCUENTO*************************
                extr = null;
                tard = null;
                perm = null;
                if (TotalHHEE > 0)
                {
                    horas = TotalHHEE / 60;
                    minut = TotalHHEE - horas * 60;
                    extr = horas.ToString("00") + ":" + minut.ToString("0#");
                }
                if (TotalHHDD > 0)
                {
                    horas = TotalHHDD / 60;
                    minut = TotalHHDD - horas * 60;
                    tard = horas.ToString("00") + ":" + minut.ToString("0#");
                }
                if (TotalHHPP > 0)
                {
                    horas = TotalHHPP / 60;
                    minut = TotalHHPP - horas * 60;
                    extr = horas.ToString("00") + ":" + minut.ToString("0#");
                }
                ViewBag.TotalTrabajados = Trabajados;
                ViewBag.TotalVacaciones = Vacaciones;
                ViewBag.TotalCompensados = TotalCompensados;
                ViewBag.TotalDescanso = Descanso;
                ViewBag.TotalPermiso = Permiso;
                ViewBag.TotalLicencia = Licencia;
                ViewBag.TotalAusencia = TotalAusencias;
                ViewBag.TotalTotales = Totales;
                ViewBag.TotalHHDD = tard;
                ViewBag.TotalHHEE = extr;
                ViewBag.TotalHHPP = TotHHPP;

            }
            catch (Exception e3)
            {
                var error = e3.Message;
            }
            //return View(db2.DIASTOTALES(Convert.ToDateTime(finicio), Convert.ToDateTime(ftermino), Convert.ToInt32(faena)));
            return View(newreg);
        }

        public ActionResult DotacionPresente()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            var newdia = new List<Informedia>();

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = "display:none";

            ViewBag.Fecha = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaMostrar = DateTime.Now.ToString("dd'-'MM'-'yyyy");

            return View(newdia);
        }

        [HttpPost]
        public ActionResult DotacionPresente(DateTime fecha, int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var newreg = new List<ResumenDias>();
            var newdia = new List<Informedia>();
            var con = new CONTRATO();

            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = " ";
            ViewBag.FechaMostrar = fecha.ToString("dd'-'MM'-'yyyy");

            DateTime finicio = fecha;
            DateTime ftermino = fecha;
            DateTime hoy = DateTime.Now.Date;
            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, faena);
                if (finicio < Fechasfaena[0])
                    finicio = Fechasfaena[0];
                if (ftermino > Fechasfaena[1])
                    ftermino = Fechasfaena[1];
            }
            //var Dotacion = db.JVC_DOTACION(faena, Convert.ToDateTime(fecha), empresa);
            var DiasTotales = AsistenciaDiariaFaena(Convert.ToDateTime(finicio), Convert.ToDateTime(ftermino), Convert.ToInt32(faena), 1);
            newreg = DiasTotales;

            int Presentes = 0;
            int Vacaciones = 0;
            int Licencia = 0;
            int Permiso = 0;
            int Contratado = 0;
            int Ausencia = 0;

            foreach (var d in newreg)
            {
                con = db.CONTRATO.Where(x => x.PERSONA == d.RUT && x.FTERMNO > hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true&& x.RECHAZADO == false).SingleOrDefault();

                Contratado = Contratado + d.DIAS_TOTALES;
                Presentes = Presentes + d.DIAS_TRABAJADOS ;
                Ausencia = Ausencia + d.DIAS_AUSENCIA + d.DIAS_DESCANSO + d.DIAS_COMPENSADOS;
                Permiso = Permiso + d.DIAS_PERMISO;
                Licencia = Licencia + d.DIAS_LICENCIA;
                Vacaciones = Vacaciones + d.DIAS_VACACIONES;
                newdia.Add(new Informedia()
                {
                    CARGO = con.CARGO,
                    TOTAL_CONTRATADOS=d.DIAS_TOTALES,
                    PRESENTES=d.DIAS_TRABAJADOS,
                    AUSENTES= d.DIAS_AUSENCIA + d.DIAS_DESCANSO + d.DIAS_COMPENSADOS,
                    PERMISOINASISTENCIA=d.DIAS_PERMISO,
                    LICENCIA=d.DIAS_LICENCIA,
                    VACACIONES=d.DIAS_VACACIONES,
                    TIPO="Directo"
                });

            }

            ViewBag.TotContratado = Contratado;
            ViewBag.TotPresentes = Presentes;
            ViewBag.TotAusencia = Ausencia;
            ViewBag.TotPermiso = Permiso;
            ViewBag.TotLicencia = Licencia;
            ViewBag.TotVacaciones = Vacaciones;

            ViewBag.Fecha = fecha.ToString("yyyy'-'MM'-'dd");
            var neword = new List<Informedia>();
            int totcon = 0;
            int totpre = 0;
            int totaus = 0;
            int totper = 0;
            int totlic = 0;
            int totvac = 0;
            string oldcar = null;
            foreach(var o in newdia.OrderBy(x => x.CARGO))
            {
                if (o.CARGO != oldcar)
                {
                    if(oldcar != null)
                    {
                       neword.Add(new Informedia()
                       {
                        CARGO = oldcar,
                        TOTAL_CONTRATADOS = totcon,
                        PRESENTES = totpre,
                        AUSENTES = totaus,
                        PERMISOINASISTENCIA = totper,
                        LICENCIA = totlic,
                        VACACIONES = totvac,
                        TIPO = "Directo"
                       });
                    }
                    totcon = 0;
                    totpre = 0;
                    totaus = 0;
                    totper = 0;
                    totlic = 0;
                    totvac = 0;
                    oldcar = o.CARGO;
                }
                    totcon = totcon + o.TOTAL_CONTRATADOS;
                    totpre = totpre + o.PRESENTES;
                    totaus = totaus + o.AUSENTES;
                    totper = totper + o.PERMISOINASISTENCIA;
                    totlic = totlic + o.LICENCIA;
                    totvac = totvac + o.VACACIONES;
            }
            if (oldcar != null)
            {
                neword.Add(new Informedia()
                {
                    CARGO = oldcar,
                    TOTAL_CONTRATADOS = totcon,
                    PRESENTES = totpre,
                    AUSENTES = totaus,
                    PERMISOINASISTENCIA = totper,
                    LICENCIA = totlic,
                    VACACIONES = totvac,
                    TIPO = "Directo"
                });
                int paso = 0;
            }

            return View(neword);
            //return View(db.JVC_DOTACION(faena, Convert.ToDateTime(fecha), empresa));
        }

        public ActionResult ReporteAusencias()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }


            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            ViewBag.FechaInicio = DateTime.Now.AddDays(-1).ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaMostrar1 = DateTime.Now.AddDays(-1).ToString("dd'-'MM'-'yyyy");
            ViewBag.FechaMostrar2 = DateTime.Now.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fechamala = "N";

            ViewBag.Visible = "display:none";



            return View(db2.REPORTEAUSENCIAS(DateTime.Now.AddDays(-1), DateTime.Now, 0));
        }

        [HttpPost]
        public ActionResult ReporteAusencias(DateTime finicio, DateTime ftermino, int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorServicio");
            }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var FaenaSL = f.FaenasEmpresaTodos(faena);
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = " ";

            if (Convert.ToDateTime(ftermino) > DateTime.Now)
            {
                ftermino = DateTime.Now;
            }
            /*
            ViewBag.FechaInicio = DateTime.Now.ToShortDateString();
            ViewBag.FechaTermino = DateTime.Now.AddDays(-1).ToShortDateString();*/

            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, faena);
                if (finicio < Fechasfaena[0])
                    finicio = Fechasfaena[0];
                if (ftermino > Fechasfaena[1])
                    ftermino = Fechasfaena[1];
            }

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaMostrar1 = finicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.FechaMostrar2 = ftermino.ToString("dd'-'MM'-'yyyy");

            if (finicio > ftermino)
            {
                ViewBag.Fechamala = "S";
                return View(db2.REPORTEAUSENCIAS(DateTime.Now.AddDays(-1), DateTime.Now, 0));
            }
            else
            {
                ViewBag.Fechamala = "N";
            }




            return View(db2.REPORTEAUSENCIAS(Convert.ToDateTime(finicio), Convert.ToDateTime(ftermino).AddHours(23), faena));
        }
        public ActionResult ModificaMarcacion()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false) { return Redirect("~/Home/ErrorPorPermiso"); }




            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;


            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            var Trabajadores = f.filtroTrabajadoresVigencia(0, Convert.ToBoolean(1));

            var TrabajadoresSL = new SelectList(Trabajadores.OrderBy(x => x.NOMBRE), "RUT", "NOMBRE");

            ViewBag.Trabajadores = TrabajadoresSL;

            ViewBag.vigenteSelected = 1;

            ViewBag.Fecha = DateTime.Now.ToString("yyyy'-'MM'-'dd");

            ViewBag.FterminoFaena = DateTime.Now.AddYears(100).ToString("yyyy'-'MM'-'dd");
            ViewBag.FinicioFaena = DateTime.Now.AddYears(-100).ToString("yyyy'-'MM'-'dd");

            ViewBag.Visible = "display:none";
            return View(db2.DETALLE_MARCACIONyRUT("X", DateTime.Now));
        }

        [HttpPost]
        public ActionResult ModificaMarcacion(int Faena, string Fecha1, string Trabajador, int vigentes, string Buttonsubmit)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            DateTime Fecha = Convert.ToDateTime(Fecha1);
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(vigentes);
            ViewBag.Faenas = FaenaSL;

            ViewBag.vigenteSelected = Convert.ToBoolean(vigentes) ? 1 : 0;

            DateTime finicio = DateTime.Now.Date;
            DateTime ftermino = DateTime.Now.Date;
            if (Buttonsubmit == null)
            {
                return View();
            }

            System.Web.HttpContext.Current.Session["sessionElegido"] = Trabajador;

            if (Faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, Faena);
                if (finicio < Fechasfaena[0])
                    finicio = Fechasfaena[0];
                if (ftermino > Fechasfaena[1])
                    ftermino = Fechasfaena[1];
            }

            //if (Fecha < finicio)
            //    {
            //        Fecha = finicio;
            //    }
            //    if (Fecha > ftermino)
            //    {
            //        Fecha = ftermino;
            //    }
            //ViewBag.Fecha = Fecha.ToString("yyyy'-'MM'-'dd");
            ViewBag.Fecha = Fecha1;

            ViewBag.Visible = "";

            if (Trabajador != "1" /*&& buttonsubmit != null*/)
            {

                return View(db2.DETALLE_MARCACIONyRUT(Trabajador, Fecha));

            }

            return View(db2.DETALLE_MARCACIONyRUT("X", DateTime.Now));



        }

        public virtual JsonResult MarcacionModificada(string RUT, DateTime MARCACION, DateTime MARCACION_NUEVA, string TIPO, string TIPO_ORIGINAL)
        {
            RUT = Convert.ToString(System.Web.HttpContext.Current.Session["sessionElegido"]);

            if (f.PoseePermiso(102) != 14 && !f.isLogged())
            {
                return Json(new
                {
                    Mensaje = "No posee permiso para modificar marcaciones"
                });
            }
            else
            {
                if (MARCACION_NUEVA.DayOfYear > DateTime.Now.AddDays(1).DayOfYear)
                {
                    return Json(new
                    {
                        Mensaje = "La nueva fecha no puede ser mayor a " + DateTime.Now.AddDays(1).ToShortDateString()
                    });
                }



                if (MARCACION_NUEVA.Hour == 00)
                {

                    MARCACION_NUEVA = MARCACION;

                }

                string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

                LOG_MARCACIONES MarcaOriginal = new LOG_MARCACIONES
                {
                    EMPRESA = empresa,
                    USUARIO = usuario,
                    TRABAJADOR = RUT,
                    FECHA_MOD = DateTime.Now,
                    MARCA_ORIGINAL = MARCACION,
                    TIPO_MARCA_ORIGINAL = TIPO_ORIGINAL,
                    MARCA_NUEVA = MARCACION_NUEVA,
                    TIPO_MARCA_NUEVA = TIPO,
                    ACCION = "ESPERAMOD"

                };

                ///  Se cambia ejecucion de procedimiento para ser activado en API
                ///  
                /// db2.MODIFICA_MARCACION(RUT, MARCACION, MARCACION_NUEVA, TIPO);
                db.LOG_MARCACIONES.Add(MarcaOriginal);
                db.SaveChanges();


                var LeeCorreo = db.PERSONA.Where(x => x.RUT == RUT).Select(x => new { x.CORREO, x.APATERNO, x.AMATERNO, x.NOMBRE }).First();

                int Vid = MarcaOriginal.ID;
                string VTipo = "M";
                string Vtoken = Guid.NewGuid().ToString();
                string VMailTrabajador = LeeCorreo.CORREO;
                string vfullname = LeeCorreo.APATERNO + " " + LeeCorreo.AMATERNO + "," + LeeCorreo.NOMBRE;


                LOG_ACEPTATIEMPO MarcaTiempo = new LOG_ACEPTATIEMPO
                {
                    LID = Vid,
                    LESTADO = "ESPERA",
                    LACCION = VTipo,
                    LFECHACAMBIO = DateTime.Now,
                    LCORREO = VMailTrabajador,
                    LTOKEN = Vtoken,
                };

                db.LOG_ACEPTATIEMPO.Add(MarcaTiempo);
                db.SaveChanges();

                Notifica_modificacion notifica_Modificacion = new Notifica_modificacion();
                notifica_Modificacion.Notifica_trabajador(TIPO, TIPO_ORIGINAL, RUT, MARCACION, MARCACION_NUEVA, VMailTrabajador, VTipo, Vtoken, Vid, 0, vfullname);



                return Json(new
                {
                    Mensaje = "Marcación modificada"
                });
            }
        }

        public virtual JsonResult MarcacionEliminada(string RUT, DateTime MARCACION, string TIPO_ORIGINAL)
        {
            RUT = Convert.ToString(System.Web.HttpContext.Current.Session["sessionElegido"]);
            if (f.PoseePermiso(102) != 14 && !f.isLogged())
            {
                return Json(new
                {
                    Mensaje = "No posee permiso para modificar marcaciones"
                });
            }
            else
            {
                string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

                LOG_MARCACIONES MarcaOriginal = new LOG_MARCACIONES
                {
                    EMPRESA = empresa,
                    USUARIO = usuario,
                    TRABAJADOR = RUT,
                    FECHA_MOD = DateTime.Now,
                    MARCA_ORIGINAL = MARCACION,
                    TIPO_MARCA_ORIGINAL = TIPO_ORIGINAL,
                    ACCION = "ELIMINADA"
                };

                db2.ELIMINA_MARCACION(RUT, MARCACION, TIPO_ORIGINAL);
                String marca = MARCACION.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'FFF");
                string coma= "DELETE FROM [gycsolcl_dbgycP].[dbo].[LOG_MARCACIONES] WHERE [EMPRESA] = '" + empresa+"' AND [TRABAJADOR] = '" + RUT+ "' AND [MARCA_ORIGINAL] = '" + marca+ "' AND [ACCION] = 'NUEVA'" ;
                db.Database.ExecuteSqlCommand(coma);

                db.LOG_MARCACIONES.Add(MarcaOriginal);
                db.SaveChanges();


                /*
           * REM solo si se requiere correos  informativo al momento de eliminar una marcacion
           * 
           * --------------
           * 
           * var LeeCorreo = db.PERSONA.Where(x => x.RUT == RUT).Select(x => new { x.CORREO, x.APATERNO, x.AMATERNO, x.NOMBRE }).First();
               string VMailTrabajador = LeeCorreo.CORREO;
               string vfullname = LeeCorreo.APATERNO + " " + LeeCorreo.AMATERNO + "," + LeeCorreo.NOMBRE;


               Notifica_modificacion notifica_Modificacion = new Notifica_modificacion();
               notifica_Modificacion.Notifica_trabajador("D", TIPO_ORIGINAL, RUT, MARCACION,MARCACION, VMailTrabajador, " ", " ", 0, 0, vfullname);


               REM Hasta aca------
          */
                return Json(new
                {
                    Mensaje = "Marcación eliminada"
                });
            }
        }
        public ActionResult ModificaTerminoContrato(int id)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var info = db.CONTRATO.Where(x => x.ID == id).Select(x => new { x.FAENA1.ID, x.FAENA1.DESCRIPCION, RUT = x.PERSONA, NOMBRE = x.PERSONA1.APATERNO + " " + x.PERSONA1.APATERNO + ", " + x.PERSONA1.NOMBRE }).FirstOrDefault();
            int IdFaena = info.ID;

            ViewBag.NombreFaena = info.DESCRIPCION;
            ViewBag.NombreTrabajador = info.NOMBRE;
            ViewBag.RutTrabajador = info.RUT;

            List<MUESTRA_FECHA_CONTRATOSResult> InfoContrato = db2.MUESTRA_FECHA_CONTRATOS(id).ToList();

            ViewBag.Alert = false;
            ViewBag.Visible = "display:none";
            ViewBag.Id = InfoContrato.FirstOrDefault().ID;
            return View(InfoContrato);
        }

        [HttpPost]
        public ActionResult ModificaTerminoContrato(string FAENA, string TRABAJADOR, HttpPostedFileBase ARCHIVO, string NUEVO_TERMINO, string RUT, int ID)
        {

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

            ViewBag.Visible = "";


            var InfoContrato = db.CONTRATO.Where(x => x.ID == ID).Select(x => new { x.ID, x.FTERMNO, x.FAENA, NOMBRETRABAJADOR = x.PERSONA1.APATERNO + " " + x.PERSONA1.AMATERNO + ", " + x.PERSONA1.NOMBRE, x.PERSONA }).FirstOrDefault();

            try
            {

                if (Convert.ToDateTime(NUEVO_TERMINO) > InfoContrato.FTERMNO)
                {
                    if (ARCHIVO != null)
                    {
                        ANEXOS Nuevo = new ANEXOS();
                        Nuevo.CONTRATO = InfoContrato.ID;
                        Nuevo.FECHA = DateTime.Now;
                        Nuevo.FIRMATRABAJADOR = true;
                        Nuevo.FIRMAEMPRESA = true;
                        Nuevo.OBSERVACIONES = "";
                        Nuevo.RECHAZADO = false;
                        Nuevo.FTERMINO = DateTime.Parse("2099-1-1");


                        byte[] bytesArchivo = null;

                        using (var binaryReader = new BinaryReader(ARCHIVO.InputStream))
                        {
                            bytesArchivo = binaryReader.ReadBytes(ARCHIVO.ContentLength);
                            Nuevo.PDF = bytesArchivo;
                        }

                        db.ANEXOS.Add(Nuevo);
                    }

                    CONTRATO UpdateContrato = db.CONTRATO.Where(x => x.ID == InfoContrato.ID).SingleOrDefault();
                    UpdateContrato.FTERMNO = Convert.ToDateTime(NUEVO_TERMINO);

                    db.SaveChanges();
                }
                else
                {
                    int BajaTrabajador = db2.DAR_BAJA_TRABAJADOR(TRABAJADOR, InfoContrato.FAENA, DateTime.Parse(NUEVO_TERMINO), InfoContrato.ID, usuario);
                    var Finiquito = db.FINIQUITO.Where(x => x.ID_CONTRATO == BajaTrabajador).FirstOrDefault();

                    if (ARCHIVO != null)
                    {
                        byte[] bytesArchivo = null;

                        using (var binaryReader = new BinaryReader(ARCHIVO.InputStream))
                        {
                            bytesArchivo = binaryReader.ReadBytes(ARCHIVO.ContentLength);
                            Finiquito.PDF = bytesArchivo;
                        }
                    }

                    db.SaveChanges();


                }

            }
            catch (Exception ex)
            {

                string Mensaje = ex.Message;
            }

            int IdFaena = db.FAENA.Where(x => x.DESCRIPCION == FAENA && x.EMPRESA == empresa).Select(x => x.ID).SingleOrDefault();

            List<MUESTRA_FECHA_CONTRATOSResult> InfoContrato2 = db2.MUESTRA_FECHA_CONTRATOS(ID).ToList();

            ViewBag.Alert = true;

            ViewBag.NombreFaena = FAENA;
            ViewBag.NombreTrabajador = InfoContrato.NOMBRETRABAJADOR;
            ViewBag.RutTrabajador = InfoContrato.PERSONA;

            return View(InfoContrato2);
        }

        public virtual JsonResult ModificacionTerminoContrato(string RUT, int ID, DateTime TERMINO_NUEVO)
        {
            if (f.PoseePermiso(102) != 14 && !f.isLogged())
            {
                return Json(new
                {
                    Mensaje = "No posee permiso para modificar marcaciones"
                });
            }
            else
            {
                db2.MODIFICA_TERMINO_CONTRATO(RUT, ID, TERMINO_NUEVO);
                return Json(new
                {
                    Mensaje = "Marcación modificada"
                });
            }
        }
        //**


        public ActionResult Repo1()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var Faenas = (from FAENA in db.FAENA
                          where FAENA.EMPRESA == empresa
                          orderby FAENA.DESCRIPCION
                          select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();


            var FaenaSL = new SelectList(Faenas, "Id", "Descripcion");
            ViewBag.Faenas = FaenaSL;

            return View();
        }

        public ActionResult Repo2()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var Faenas = (from FAENA in db.FAENA
                          where FAENA.EMPRESA == empresa
                          orderby FAENA.DESCRIPCION
                          select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();


            var FaenaSL = new SelectList(Faenas, "Id", "Descripcion");
            ViewBag.Faenas = FaenaSL;

            return View();
        }

        public ActionResult EventosNoPareados()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            DateTime hoy = DateTime.Now.Date;
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            ViewBag.CountTabla = 0;

            ViewBag.FechaInicio = DateTime.Now.AddDays(-2).ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = DateTime.Now.AddDays(-1).ToString("yyyy'-'MM'-'dd");


            ViewBag.Visible = "display:none";

            return View(db2.EVENTOSNOPAREADOS(empresa, 0, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1)));
        }

        public class NuevaMarcacion
        {
            public string RUT { get; set; }
            public string NUEVAHORA { get; set; }
        }

        [HttpPost]
        public ActionResult EventosNoPareados(int vigentes, int faena, DateTime finicio, DateTime ftermino, List<NuevaMarcacion> Marcaciones)
        {

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }
            DateTime hoy = DateTime.Now.Date;

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, faena);
                if (finicio < Fechasfaena[0])
                    finicio = Fechasfaena[0];
                if (ftermino > Fechasfaena[1])
                    ftermino = Fechasfaena[1];
            }

            var FaenaSL = f.FaenasEmpresaTodos(vigentes);
            ViewBag.Faenas = FaenaSL;

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            ViewBag.Visible = " ";


            return View(db2.EVENTOSNOPAREADOS(empresa, faena, finicio, ftermino));
        }

        public virtual JsonResult EventoPareado(string RUT, DateTime PAREADO, DateTime MARCAORIGINAL)
        {
            if (f.PoseePermiso(102) < 13)
            {
                return Json(new
                {
                    Mensaje = "No posee permiso para parear marcaciones"
                });
            }
            else
            {
                string Tipo = string.Empty;
                string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

                var LeeCorreo = db.PERSONA.Where(x => x.RUT == RUT).Select(x => new { x.CORREO, x.APATERNO, x.AMATERNO, x.NOMBRE }).First();

                string VMailTrabajador = LeeCorreo.CORREO;
                string VTipo = "I";
                string Vtoken = Guid.NewGuid().ToString();
                string vfullname = LeeCorreo.APATERNO + " " + LeeCorreo.AMATERNO + "," + LeeCorreo.NOMBRE;
                int VRenid = 0;
                int VRenSal = 0;
                String VPareado = " ";

                // DateTime MarcaEntrada = Convert.ToDateTime(Fecha + " " + HEntrada);
                // DateTime MarcaSalida = Convert.ToDateTime(Fecha + " " + HSalida);

                DateTime MarcaSalida = PAREADO;
                DateTime MarcaEntrada = PAREADO;
                DateTime MarcaOri = MARCAORIGINAL;



                if (MARCAORIGINAL < PAREADO)
                {
                    Tipo = "O";
                    VTipo = "S";
                    LOG_MARCACIONES MarcaOriginalSalida = new LOG_MARCACIONES
                    {
                        EMPRESA = empresa,
                        USUARIO = usuario,
                        TRABAJADOR = RUT,
                        FECHA_MOD = DateTime.Now,
                        MARCA_ORIGINAL = MarcaSalida,
                        TIPO_MARCA_ORIGINAL = "O",
                        MARCA_NUEVA = MarcaSalida,
                        TIPO_MARCA_NUEVA = "O",
                        ACCION = "ESPERANUE"
                    };
                    db.LOG_MARCACIONES.Add(MarcaOriginalSalida);
                    db.SaveChanges();

                    VRenSal = MarcaOriginalSalida.ID;

                    LOG_ACEPTATIEMPO MarcaNuevaSal = new LOG_ACEPTATIEMPO
                    {
                        LID = VRenSal,
                        LESTADO = "ESPERA",
                        LACCION = Tipo,
                        LFECHACAMBIO = DateTime.Now,
                        LCORREO = VMailTrabajador,
                        LTOKEN = Vtoken,
                    };

                    db.LOG_ACEPTATIEMPO.Add(MarcaNuevaSal);
                    db.SaveChanges();

                }
                else
                {

                    /*
                     * Re Escribir Evento Pareado existente
                     */
                    VPareado = "P";
                    LOG_MARCACIONES MarcaOriginalori = new LOG_MARCACIONES
                    {
                        EMPRESA = empresa,
                        USUARIO = usuario,
                        TRABAJADOR = RUT,
                        FECHA_MOD = DateTime.Now,
                        MARCA_ORIGINAL = MarcaOri,
                        TIPO_MARCA_ORIGINAL = "I",
                        MARCA_NUEVA = MarcaOri,
                        TIPO_MARCA_NUEVA = "O",
                        ACCION = "ESPERAMOD"

                    };

                    db.LOG_MARCACIONES.Add(MarcaOriginalori);
                    db.SaveChanges();
                    VRenSal = MarcaOriginalori.ID;

                    LOG_ACEPTATIEMPO MarcaTiempoori = new LOG_ACEPTATIEMPO
                    {
                        LID = VRenSal,
                        LESTADO = "ESPERA",
                        LACCION = "M",
                        LFECHACAMBIO = DateTime.Now,
                        LCORREO = VMailTrabajador,
                        LTOKEN = Vtoken,
                    };

                    db.LOG_ACEPTATIEMPO.Add(MarcaTiempoori);
                    db.SaveChanges();

                    /* Nuevo Cambio Realizado
                     */
                    LOG_MARCACIONES MarcaOriginalEntrada = new LOG_MARCACIONES
                    {
                        EMPRESA = empresa,
                        USUARIO = usuario,
                        TRABAJADOR = RUT,
                        FECHA_MOD = DateTime.Now,
                        MARCA_ORIGINAL = MarcaEntrada,
                        TIPO_MARCA_ORIGINAL = "I",
                        MARCA_NUEVA = MarcaEntrada,
                        TIPO_MARCA_NUEVA = "I",
                        ACCION = "ESPERANUE"
                    };

                    db.LOG_MARCACIONES.Add(MarcaOriginalEntrada);
                    db.SaveChanges();
                    VRenid = MarcaOriginalEntrada.ID;

                    LOG_ACEPTATIEMPO MarcaNuevaEnt = new LOG_ACEPTATIEMPO
                    {
                        LID = VRenid,
                        LESTADO = "ESPERA",
                        LACCION = "I",
                        LFECHACAMBIO = DateTime.Now,
                        LCORREO = VMailTrabajador,
                        LTOKEN = Vtoken,
                    };

                    db.LOG_ACEPTATIEMPO.Add(MarcaNuevaEnt);
                    db.SaveChanges();

                }

                Notifica_modificacion notifica_Modificacion = new Notifica_modificacion();
                notifica_Modificacion.Notifica_trabajador(VPareado, " ", RUT, MARCAORIGINAL, PAREADO, VMailTrabajador, VTipo, Vtoken, VRenid, VRenSal, vfullname);

                // RROCCO 
                // Se agrega parrafo para link respuesta

                //db2.INSERTMARCAMANUAL(RUT, PAREADO, Tipo);
                return Json(new
                {
                    Mensaje = "Marcación pareada"
                });
            }


        }
        public ActionResult SelAutHHEE()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            if (f.PermisoPorServicio(113) == false)
            {
                return Redirect("~/ControlAsistencia/AutorizacionHHEE");
            }
            return View();
        }

        public ActionResult AutorizacionHHEE()
        {

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(105) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            List<OfertaHorasExtrasTurno> regext = new List<OfertaHorasExtrasTurno>();

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> JornadaList = new List<SelectListItem>
                {
                new SelectListItem() { Text = "Salida" , Value = "T" } ,
                new SelectListItem() { Text = "Entrada" , Value = "M" } ,
                new SelectListItem() { Text = "Fuera de Turno" , Value = "FUERA TURNO" }
                };

            ViewBag.Jornadas = new SelectList(JornadaList, "Value", "Text");

            ViewBag.Visible = "display:none";

            DateTime finicio = DateTime.Now.AddDays(-1).AddHours(-DateTime.Now.Hour);
            DateTime ftermino = DateTime.Now;

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");
            ViewBag.Fecha1 = finicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fecha2= ftermino.ToString("dd'-'MM'-'yyyy");
            ViewBag.MinutosManana = 30;

            string Jornada = string.Empty;
            return View(regext);

        }

        [HttpPost]
        public ActionResult AutorizacionHHEE(DateTime finicio, DateTime ftermino, int minutos, int faena, string jornada)
        {

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(105) != 14) { return Redirect("~/Home/ErrorPorServicio"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            List<OfertaHorasExtrasTurno> regext = new List<OfertaHorasExtrasTurno>();
           var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            List<SelectListItem> JornadaList = new List<SelectListItem>
                {
                new SelectListItem() { Text = "Salida" , Value = "T" } ,
                new SelectListItem() { Text = "Entrada" , Value = "M" } ,
                new SelectListItem() { Text = "Fuera de Turno" , Value = "FUERA TURNO" }
                };

            ViewBag.Jornadas = new SelectList(JornadaList, "Value", "Text");

            ViewBag.Visible = " ";

            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, faena);
                if (finicio < Fechasfaena[0])
                    finicio = Fechasfaena[0];
                if (ftermino > Fechasfaena[1])
                    ftermino = Fechasfaena[1];
            }


            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");
            ViewBag.Fecha1 = finicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fecha2 = ftermino.ToString("dd'-'MM'-'yyyy");

            ViewBag.MinutosManana = minutos;
            regext = OfertaHorasExtrasTurno(finicio, ftermino, faena, minutos, jornada);
            ViewBag.Todos = "S";
            if (regext.Count == 0) ViewBag.Todos = "N";
            return View(regext);
        }


        public virtual JsonResult AutorizaHHEE(string RUT, string FECHA, string HHEE)
        {
            try
            {
                if (f.PoseePermiso(105) != 14)
                {
                    return Json(new
                    {
                        Mensaje = "No posee permisos para autorizar horas extras",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {

                    HHEE extras = new HHEE();
                    DateTime ftermino = Convert.ToDateTime(HHEE);
                    DateTime finicio = Convert.ToDateTime(FECHA);
                    int hrs = ftermino.Hour;
                    int mnt = ftermino.Minute;
                    ftermino = finicio.AddHours(hrs);
                    ftermino = ftermino.AddMinutes(mnt);
                    extras.TRABAJADOR = RUT;
                    extras.FINICIO = finicio;
                    extras.FTERMINO = ftermino;
                    db.HHEE.Add(extras);
                    db.SaveChanges();
                    //db2.INSERTHHEE(RUT, Convert.ToDateTime(fec1), Convert.ToDateTime(fec2));
                    return Json(new
                    {
                        Mensaje = "Autorización exitosa",
                        redirectURL = Url.Action("AutorizacionHHEE", "ControlAsistencia"),
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

        public virtual JsonResult AutorizaTodoHHEE(string FINICIO, string FTERMINO, int MINUTOS, string JORNADA, int FAENA)
        {
            try
            {
                if (f.PoseePermiso(105) != 14)
                {
                    return Json(new
                    {
                        Mensaje = "No posee permisos para autorizar horas extras",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {
                    var Autorizaciones = (from aut in db2.OFERTAHHEE_RELOJ(Convert.ToDateTime(FINICIO), Convert.ToDateTime(FTERMINO), MINUTOS, FAENA, JORNADA)
                                          select aut).ToList();

                    string RUT = string.Empty;
                    string FECHA = string.Empty;
                    string HHEE = string.Empty;

                    foreach (var item in Autorizaciones.ToList())
                    {
                        if (JORNADA == "FUERA TURNO")
                        {
                            FECHA = item.FECHA + ' ' + item.MARCACION.Substring(0, 8);
                        }
                        else
                        {
                            FECHA = item.FECHA + ' ' + item.MARCACION;
                        }

                        db2.INSERTHHEE(item.RUT, Convert.ToDateTime(FECHA), Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + item.HHEE));
                    }


                    return Json(new
                    {
                        Mensaje = "Marcación registrada exitosamente",
                        redirectURL = Url.Action("AutorizacionHHEE", "ControlAsistencia"),
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

        [HttpGet]
        public ActionResult AutorizacionHHEE_HORAS()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(113) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var nrextra = new List<OfertaHorasExtras>();

            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = "display:none";

            int mincolacion;
            remepage dettiemp = new remepage();
            var infoTiemp = new List<GYCEmpresa.Models.remepage>();
            infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
            dettiemp = infoTiemp.Where(x => "3         " == x.cod_param).SingleOrDefault();
            mincolacion = 0;
            if (dettiemp != null)
                mincolacion = Convert.ToInt32(dettiemp.val_param);

            DateTime finicio = DateTime.Now.AddDays(-1).AddHours(-DateTime.Now.Hour);
            DateTime ftermino = DateTime.Now;

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");
            ViewBag.HorasTrabajo = "07:30";
            ViewBag.HoraColacion = "01:00";
            ViewBag.Fecha1 = finicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fecha2 = ftermino.ToString("dd'-'MM'-'yyyy");

            string Jornada = string.Empty;

            return View(nrextra);
        }

        [HttpPost]
        public ActionResult AutorizacionHHEE_HORAS(DateTime finicio, DateTime ftermino, int faena, string horasTrabajo, string horasColacion)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(113) != 14) { return Redirect("~/Home/ErrorPorServicio"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;

            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;


            ViewBag.Visible = " ";
            string[] HoraTrabajo = horasTrabajo.Split(':');
            string[] HoraColacion = horasColacion.Split(':');

            int Horas = int.Parse(HoraTrabajo[0]) + int.Parse(HoraColacion[0]);
            int Minutos = int.Parse(HoraTrabajo[1]) + int.Parse(HoraColacion[1]);

            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, faena);
                if (finicio < Fechasfaena[0])
                    finicio = Fechasfaena[0];
                if (ftermino > Fechasfaena[1])
                    ftermino = Fechasfaena[1];
            }

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");
            ViewBag.Fecha1 = finicio.ToString("dd'-'MM'-'yyyy");
            ViewBag.Fecha2 = ftermino.ToString("dd'-'MM'-'yyyy");

            ViewBag.HorasTrabajo = horasTrabajo;
            ViewBag.HORAcOLACION = horasColacion;

            var regextra = OfertaHorasExtras(finicio, ftermino.AddHours(23), faena, Horas, Minutos);
            return View(regextra);
        }

        public virtual JsonResult AutorizaHHEE_HORAS(string RUT, string MARCACION_ENTRADA, string HHEE)
        {
            try
            {
                if (f.PoseePermiso(113) != 14 || !f.isLogged())
                {
                    return Json(new
                    {
                        Mensaje = "No posee permisos para autorizar horas extras",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {
                    db2.INSERTHHEE(RUT, Convert.ToDateTime(MARCACION_ENTRADA), Convert.ToDateTime(HHEE));
                    return Json(new
                    {
                        Mensaje = "Autorización exitosa",
                        redirectURL = Url.Action("AutorizacionHHEE_HORAS", "ControlAsistencia"),
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

        public virtual JsonResult AutorizaTodoHHEE_HORAS(string FINICIO, string FTERMINO, int MINUTOS, string JORNADA, int FAENA)
        {
            try
            {
                if (f.PoseePermiso(113) != 14 || !f.isLogged())
                {
                    return Json(new
                    {
                        Mensaje = "No posee permisos para autorizar horas extras",
                        redirectURL = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
                else
                {
                    var Autorizaciones = (from aut in db2.OFERTAHHEE_RELOJ(Convert.ToDateTime(FINICIO), Convert.ToDateTime(FTERMINO), MINUTOS, FAENA, JORNADA)
                                          select aut).ToList();

                    string RUT = string.Empty;
                    string FECHA = string.Empty;
                    string HHEE = string.Empty;

                    foreach (var item in Autorizaciones.ToList())
                    {
                        if (JORNADA == "FUERA TURNO")
                        {
                            FECHA = item.FECHA + ' ' + item.MARCACION.Substring(0, 8);
                        }
                        else
                        {
                            FECHA = item.FECHA + ' ' + item.MARCACION;
                        }

                        db2.INSERTHHEE(item.RUT, Convert.ToDateTime(FECHA), Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + item.HHEE));
                    }


                    return Json(new
                    {
                        Mensaje = "Marcación registrada exitosamente",
                        redirectURL = Url.Action("AutorizacionHHEE_HORAS", "ControlAsistencia"),
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

        public ActionResult NuevaMarcacionManual()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(102) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            //int IdFaena = Convert.ToInt32(Faenas.First().Id);

            DateTime FechaLimite = DateTime.Now.AddDays(5);


            List<SelectListItem> TrabajadorList = new List<SelectListItem>();

            TrabajadorList.Add(new SelectListItem() { Text = "Seleccione trabajador", Value = "1" });

            var TrabajadoresSL = new SelectList(TrabajadorList, "Value", "Text");
            ViewBag.Trabajadores = TrabajadoresSL;
            ViewBag.FechaIni = DateTime.Now.Date.ToString("yyyy'-'MM'-'dd");
            ViewBag.vigenteSelected = 1;

            return View();
        }

        [HttpPost]
        public ActionResult NuevaMarcacionManual(int faena, int vigentes)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(102) < 13) { return Redirect("~/Home/ErrorPorServicio"); }

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;

            var Trabajadores = f.filtroTrabajadoresVigencia(0, Convert.ToBoolean(vigentes));

            var TrabajadoresSL = new SelectList(Trabajadores.OrderBy(x => x.NOMBRE), "RUT", "NOMBRE");
            ViewBag.Trabajadores = TrabajadoresSL;
            ViewBag.vigenteSelected = Convert.ToBoolean(vigentes) ? 1 : 0;

            return View();
        }

        public virtual JsonResult MarcaManual(string Rut, string Fecha, string HEntrada, string HSalida)
        {
            try
            {


                if (f.PoseePermiso(102) < 13 || !f.isLogged())
                {
                    return Json(new
                    {
                        Mensaje = "No posee permiso para realizar esta operación"
                    });
                }

                if (DateTime.Parse(Fecha).DayOfYear > DateTime.Now.AddDays(1).DayOfYear)
                {
                    return Json(new
                    {
                        Mensaje = "La fecha de la nueva marcación no puede ser superior a " + DateTime.Now.AddDays(1).ToShortDateString()
                    });
                }

                string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

                DateTime MarcaEntrada = Convert.ToDateTime(Fecha + " " + HEntrada);
                DateTime MarcaSalida = Convert.ToDateTime(Fecha + " " + HSalida);

                var PoseeMarcacion = (db2.PERMITEMARCACIONMANUAL(Rut, MarcaEntrada).SingleOrDefault());
                int CantidadDeMarcaciones = Convert.ToInt32(PoseeMarcacion.Column1.ToString());
                if (CantidadDeMarcaciones == 0)
                {
                    ///            db2.INSERTMARCAMANUAL(Rut, MarcaEntrada, "I");
                    ///             db2.INSERTMARCAMANUAL(Rut, MarcaSalida, "O");

                    LOG_MARCACIONES MarcaOriginalEntrada = new LOG_MARCACIONES
                    {
                        EMPRESA = empresa,
                        USUARIO = usuario,
                        TRABAJADOR = Rut,
                        FECHA_MOD = DateTime.Now,
                        MARCA_ORIGINAL = MarcaEntrada,
                        TIPO_MARCA_ORIGINAL = "I",
                        MARCA_NUEVA = MarcaEntrada,
                        TIPO_MARCA_NUEVA = "I",
                        ACCION = "ESPERANUE"
                    };

                    LOG_MARCACIONES MarcaOriginalSalida = new LOG_MARCACIONES
                    {
                        EMPRESA = empresa,
                        USUARIO = usuario,
                        TRABAJADOR = Rut,
                        FECHA_MOD = DateTime.Now,
                        MARCA_ORIGINAL = MarcaSalida,
                        TIPO_MARCA_ORIGINAL = "O",
                        MARCA_NUEVA = MarcaSalida,
                        TIPO_MARCA_NUEVA = "O",
                        ACCION = "ESPERANUE"
                    };

                    db.LOG_MARCACIONES.Add(MarcaOriginalEntrada);
                    db.LOG_MARCACIONES.Add(MarcaOriginalSalida);

                    db.SaveChanges();

                    int VRenid = MarcaOriginalEntrada.ID;
                    int VRenSal = MarcaOriginalSalida.ID;

                    var LeeCorreo = db.PERSONA.Where(x => x.RUT == Rut).Select(x => new { x.CORREO, x.APATERNO, x.AMATERNO, x.NOMBRE }).First();

                    string VMailTrabajador = LeeCorreo.CORREO;
                    string VTipo = "I";
                    string Vtoken = Guid.NewGuid().ToString();
                    string vfullname = LeeCorreo.APATERNO + " " + LeeCorreo.AMATERNO + "," + LeeCorreo.NOMBRE;


                    LOG_ACEPTATIEMPO MarcaNuevaEnt = new LOG_ACEPTATIEMPO
                    {
                        LID = VRenid,
                        LESTADO = "ESPERA",
                        LACCION = VTipo,
                        LFECHACAMBIO = DateTime.Now,
                        LCORREO = VMailTrabajador,
                        LTOKEN = Vtoken,
                    };

                    db.LOG_ACEPTATIEMPO.Add(MarcaNuevaEnt);
                    db.SaveChanges();

                    LOG_ACEPTATIEMPO MarcaNuevaSal = new LOG_ACEPTATIEMPO
                    {
                        LID = VRenSal,
                        LESTADO = "ESPERA",
                        LACCION = VTipo,
                        LFECHACAMBIO = DateTime.Now,
                        LCORREO = VMailTrabajador,
                        LTOKEN = Vtoken,
                    };

                    db.LOG_ACEPTATIEMPO.Add(MarcaNuevaSal);
                    db.SaveChanges();


                    Notifica_modificacion notifica_Modificacion = new Notifica_modificacion();
                    notifica_Modificacion.Notifica_trabajador(" ", " ", Rut, MarcaEntrada, MarcaSalida, VMailTrabajador, VTipo, Vtoken, VRenid, VRenSal, vfullname);


                    //return RedirectToAction("NuevaMarcacionManual" , "ControlAsistencia");
                    return Json(new
                    {
                        Mensaje = "Marcación registrada exitosamente",
                        redirectURL = Url.Action("NuevaMarcacionManual", "ControlAsistencia"),
                        isRedirect = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        Mensaje = "El trabajador ya posee marcación en el día seleccionado."
                    });
                }
            }
            catch (Exception e2)
            {

                return Json(e2.Message);
            }

        }

        //public ActionResult AutorizaDiascompensados()
        //{
        //    if (!f.isLogged()) { return Redirect("~/Home/Login"); }

        //    if (f.PermisoPorServicio(112) == false)
        //    {
        //        return Redirect("~/Home/ErrorPorPermiso");
        //    }


        //    string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
        //    var Faenas = (from FAENA in db.FAENA
        //                  where FAENA.EMPRESA == empresa
        //                  select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

        //    var FaenaSL = new SelectList(Faenas, "Id", "Descripcion");
        //    ViewBag.Faenas = FaenaSL;

        //    ViewBag.Visible = "display:none";

        //    DateTime finicio = DateTime.Now.AddDays(-1).AddHours(-DateTime.Now.Hour);
        //    DateTime ftermino = DateTime.Now;

        //    ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
        //    ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");


        //    return View(db2.OFERTA_DOMINGOS_TRABAJADOS(empresa, finicio, ftermino, Faenas.First().Id));
        //}

        //[HttpPost]
        //public ActionResult AutorizaDiascompensados(DateTime finicio, DateTime ftermino, int faena)
        //{
        //    if (!f.isLogged()) { return Redirect("~/Home/Login"); }

        //    if (f.PoseePermiso(112) != 14) { return Redirect("~/Home/ErrorPorServicio"); }

        //    string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
        //    var Faenas = (from FAENA in db.FAENA
        //                  where FAENA.EMPRESA == empresa
        //                  select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

        //    var FaenaSL = new SelectList(Faenas, "Id", "Descripcion");
        //    ViewBag.Faenas = FaenaSL;

        //    ViewBag.Visible = " ";

        //    var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, faena);
        //    finicio = Fechasfaena[0];
        //    ftermino = Fechasfaena[1];

        //    ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
        //    ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

        //    return View(db2.OFERTA_DOMINGOS_TRABAJADOS(empresa, finicio, ftermino, faena));
        //}

        //public virtual JsonResult AutorizaDDCC(string RUT, string FECHA)
        //{
        //    try
        //    {

        //        if (f.PoseePermiso(112) != 14 || !f.isLogged())
        //        {
        //            return Json(new
        //            {
        //                Mensaje = "No posee permisos para realizar esta operación"
        //            });
        //        }

        //        string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
        //        string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

        //        //if (Permiso(1) == false)
        //        //{
        //        //    return Json(new
        //        //    {
        //        //        Mensaje = "No posee permisos para autorizar dias compensados",
        //        //        redirectURL = Url.Action("Index", "Home"),
        //        //        isRedirect = true
        //        //    });
        //        //}
        //        //else
        //        //{
        //            //db2.INSERT_DIACOMPENSADO(RUT, Convert.ToDateTime(FECHA), Convert.ToDateTime(HHEE));
        //            db2.INSERT_DIACOMPENSADO(RUT, Convert.ToDateTime(FECHA), usuario, empresa);
        //            return Json(new
        //            {
        //                Mensaje = "Autorización registrada exitosamente",
        //                redirectURL = Url.Action("AutorizaDiascompensados", "ControlAsistencia"),
        //                isRedirect = true
        //            });
        //        //}
        //    }
        //    catch (Exception e2)
        //    {
        //        return Json(new
        //        {
        //            Mensaje = e2.Message
        //        });
        //    }
        //}

        //public virtual JsonResult AutorizaTodoDDCC(string FINICIO, string FTERMINO, int FAENA)
        //{
        //    try
        //    {
        //        if (f.PoseePermiso(112) != 14 || !f.isLogged())
        //        {
        //            return Json(new
        //            {
        //                Mensaje = "No posee permisos para realizar esta operación"
        //            });
        //        }

        //        string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
        //        string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

        //        if (Permiso(1) == false)
        //        {
        //            return Json(new
        //            {
        //                Mensaje = "No posee permisos para autorizar dias compensados",
        //                redirectURL = Url.Action("Index", "Home"),
        //                isRedirect = true
        //            });
        //        }
        //        else
        //        {
        //            var Autorizaciones = (db2.OFERTA_DOMINGOS_TRABAJADOS(empresa, Convert.ToDateTime(FINICIO), Convert.ToDateTime(FTERMINO), FAENA)).ToList();


        //            foreach (var item in Autorizaciones.ToList())
        //            {

        //                db2.INSERT_DIACOMPENSADO(item.RUT, Convert.ToDateTime(item.FECHA), usuario, empresa);
        //            }


        //            return Json(new
        //            {
        //                Mensaje = "Autorizaciones registradas exitosamente",
        //                redirectURL = Url.Action("AutorizaDiascompensados", "ControlAsistencia"),
        //                isRedirect = true
        //            });
        //        }
        //    }
        //    catch (Exception e2)
        //    {
        //        return Json(new
        //        {
        //            Mensaje = e2.Message
        //        });
        //    }

        //}

        public ActionResult ReporteResumenDiasCompensados()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PermisoPorServicio(112) == false)
            {
                return Redirect("~/Home/ErrorPorPermiso");
            }


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var Faenas = (from FAENA in db.FAENA
                          where FAENA.EMPRESA == empresa
                          orderby FAENA.DESCRIPCION
                          select new { Id = FAENA.ID, Descripcion = FAENA.DESCRIPCION }).ToList();

            var FaenaSL = new SelectList(Faenas, "Id", "Descripcion");
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = "display:none";

            DateTime finicio = DateTime.Now.AddDays(-1).AddHours(-DateTime.Now.Hour);
            DateTime ftermino = DateTime.Now;

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");


            return View(db2.OFERTA_DOMINGOS_TRABAJADOS(empresa, finicio, ftermino, Faenas.First().Id));
        }

        [HttpPost]
        public ActionResult ReporteResumenDiasCompensados(DateTime finicio, DateTime ftermino, int faena)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (f.PoseePermiso(112) == 99999) { return Redirect("~/Home/ErrorPorServicio"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;


            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faenas = FaenaSL;

            ViewBag.Visible = " ";

            if (faena > 0)
            {
                var Fechasfaena = f.FechasLimiteFaena(finicio, ftermino, faena);
                if (finicio < Fechasfaena[0])
                    finicio = Fechasfaena[0];
                if (ftermino > Fechasfaena[1])
                    ftermino = Fechasfaena[1];
            }

            ViewBag.FechaInicio = finicio.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaTermino = ftermino.ToString("yyyy'-'MM'-'dd");

            return View(db2.OFERTA_DOMINGOS_TRABAJADOS(empresa, finicio, ftermino, faena));
        }

        /// <summary>
        ///   Clases para Notificar al trabajador al momento de modificar hora Ingreso Salida
        ///   
        /// </summary>
        public class Notifica_modificacion
        {
            /// <summary>
            ///  Se envia RUT para notifica al trabajdor
            ///  se recibe True
            /// </summary>
            /// <param name="Prut"></param>
            /// <returns></returns>
            /// 

            Funciones f = new Funciones();
            public bool Notifica_trabajador(string PTIPO1, string PTIPO_ORIGINAL, string Prut, DateTime PMARCACION, DateTime PMARCACION_NUEVA, String PCORREO, String PTipo, string PToken, int PID1, int PID2, string PFullname)
            {

                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                //string correo_usr = "Ricardo.rocco@gmail.com";
                string correo_usr = PCORREO;
                string nombre_full = PFullname;
                DateTime hoy = DateTime.Now.AddHours(f.AjusteHora());

                string FMARCACION = PMARCACION.ToString("dd'-'MM'-'yyyy HH:mm:ss");
                string FNUEVA = PMARCACION_NUEVA.ToString("dd'-'MM'-'yyyy HH:mm:ss");
                string Fhoy = hoy.ToString("dd'-'MM'-'yyyy HH:mm:ss");
                int enroquepareado = 0;

                string rut_usr = Prut;
                string fvtiponuevo = " ";
                string fvtipooriginal = " ";

                if (PTIPO1 == "I") { fvtiponuevo = "Entrada"; } else { fvtiponuevo = "Salida"; };
                if (PTIPO_ORIGINAL == "I") { fvtipooriginal = "Entrada"; } else { fvtipooriginal = "Salida"; };

                string linkhost = " ";

                if (PTIPO1 == "P")
                {
                    linkhost = "https://api.gycsol.cl/api/RespuestaPareada/GetPERSONA?Token=";
                    enroquepareado = PID2;
                    PID2 = PID1;
                    PID1 = enroquepareado;
                }
                else
                {
                    linkhost = "https://api.gycsol.cl/api/RespuestaTrabajador/GetPERSONA?Token=";
                }

                string LinkSi = linkhost + PToken + "&Answer=SI&Id1=" + PID1 + "&Id2=" + PID2;

                string LinkNo = linkhost + PToken + "&Answer=NO&Id1=" + PID1 + "&Id2=" + PID2;


                msg.To.Add(correo_usr);
                /// msg.Subject = "Envío Modificacion Control  Reloj " + DateTime.Now;
                /// 
                /*
                 * Tipos en Variables Ptipo
                 * 
                 * M =  Modificacion de Tiempo
                 * I =  Nuevo Ingreso de Tiempo
                 * E =  Nueva Entrada en Pareado (PMARCACION = Fecha Marcada x trabajador ; PMARCACION_NUEVA = Fecha Nueva Pareada )
                 * S =  Nueva Salida en Pareado
                 * 
                 * Tipo en Variable PTIPO1
                 * P =  Corresponde a Evento Pareado con tratamiento especial en entrada y salida
                 * D =  Elemento Eliminado
                 */

                if (PTIPO1 == "D")
                {
                    PTipo = "N"; // Se asigna N de Nulo para evitar otro tratamiento //

                    msg.Subject = "Notifica Eliminación Registro Control Tiempo -  GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                                           "Notificamos que se ha efectuado  una eliminación  en el registro control tiempo por parte de su empresa." +
                                           "<br/><br/> Fecha : " + Fhoy + "<br/>  Marcación Eliminada : " + FMARCACION + " " + fvtipooriginal
                                           + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" +
                                           " <br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";


                }

                if (PTIPO1 == "P")
                {
                    PTipo = "N"; // Se asigna N de Nulo para evitar otro tratamiento //

                    msg.Subject = "Autorización de Nuevo Ingreso Pareado - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                                           "Notificamos que el sistema ha detectado un nuevo ingreso en el registro control tiempo por parte de su empresa." +
                                           "<br/><br/> Fecha : " + Fhoy + "<br/>  Marcación Original : " + FMARCACION + " Entrada " + "<br/> Marcación Agregada: " + FNUEVA +
                                           " Entrada " + "<br/><br/> Marcación final a autorizar : " + FNUEVA + " Entrada / " + FMARCACION + " Salida <br/><br/>"
                                           + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                                           " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                                           + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                                       + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                                              + LinkNo +
                                           " <br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";
                }


                if (PTipo == "M")
                {

                    msg.Subject = "Autorización de Modificación - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                        "Notificamos que el sistema ha detectado un cambio en el registro control tiempo por parte de su empresa." +
                        "<br/><br/> Fecha : " + Fhoy + "<br/> Marcación Registrada : " + FMARCACION + " " + fvtipooriginal + "<br/> Nueva Marcación : " + FNUEVA + " " + fvtiponuevo +
                        " <br/>Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                        " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                        + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                    + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                           + LinkNo +
                        " <br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";

                }
                if (PTipo == "I")
                {
                    msg.Subject = "Autorización de Nuevo Ingreso - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                        "Notificamos que el sistema ha detectado un nuevo ingreso en el registro control tiempo por parte de su empresa." +
                        "<br/><br/> Fecha : " + Fhoy + "<br/>  Nuevo Registro : " + FMARCACION + " Entrada " + "<br/> Nuevo Registro: " + FNUEVA +
                        " Salida " + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                        " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                        + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                    + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                           + LinkNo +
                        "<br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";


                }
                if (PTipo == "E")
                {
                    msg.Subject = "Autorización de Nuevo Ingreso Pareado - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                        "Notificamos que el sistema ha detectado un nuevo ingreso en el registro control tiempo por parte de su empresa." +
                        "<br/><br/> Fecha : " + Fhoy + "<br/>  Marcación Registrada : " + FMARCACION + " Salida " + "<br/> Evento Pareado: " + FNUEVA +
                        " Entrada " + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                        " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                        + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                    + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                           + LinkNo +
                        "<br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";

                }
                if (PTipo == "S")
                {
                    msg.Subject = "Autorización de Nuevo Ingreso Pareado - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                        "Notificamos que el sistema ha detectado un nuevo ingreso en el registro control tiempo por parte de su empresa." +
                        "<br/><br/> Fecha : " + Fhoy + "<br/>  Marcación Registrada : " + FMARCACION + " Entrada " + "<br/> Evento Pareado: " + FNUEVA +
                        " Salida " + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                        " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                        + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                    + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                           + LinkNo +
                        "<br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";

                }

                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.From = new System.Net.Mail.MailAddress("plataforma@gycsol.cl");
                //msg.Bcc.Add("gycsol.almacen@gmail.com");
                msg.Bcc.Add("gycsol.almacen@gmail.com");
                System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient()
                {
                    //Credentials = new System.Net.NetworkCredential("plataforma@gycsol.cl", "Lq!6q9g2"),
                    Credentials = new System.Net.NetworkCredential("plataforma@gycsol.cl", "4Ww8y7^j"),
                    EnableSsl = false,
                    Host = "gycsol.cl",
                    Port = 25,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                };

                try
                {
                    cliente.Send(msg);
                    return true;
                }
                catch (Exception e2)
                {
                    //MessageBox.Show("Error al enviar el correo " + e2, "Error");

                    return true;


                }
            }
        }





        public class MarcacionModel
        {
            public string RUT { get; set; }
            public string NOMBRES { get; set; }
            public List<MARCACIONyRUT> marcs { get; set; }
            public List<HHEE> horasExtra { get; set; }
        }

        public class VacacionesList
        {
            public List<Vacaciones> Vacaciones { get; set; }
            public VacacionesList()
            {
                Vacaciones = new List<Vacaciones>();
            }
        }

        public class Vacaciones
        {
            public int ID { get; set; }
            public string TRABAJADOR { get; set; }
            public DateTime FINICIO { get; set; }
            public DateTime FTERMINO { get; set; }
        }



        //Resumen de Asistencia

        public struct marcas
        {
            public string fec;
            public string tip;
            public string mar;
        }

        DateTime[] fechas = new DateTime[100];
        string[] fecstr = new string[100];
        String[] entrada = new string[100];
        String[] salida = new string[100];

        marcas[] marrel = new marcas[1000];
        int dias;
        DateTime fecini, fecfin;
        int ndias;
        string nom;

        [HttpGet]
        public ActionResult ResumenAsistencia()
        {
            ViewBag.FechaInicio = DateTime.Now.Date.ToString("yyyy'-'MM'-'dd");
            ViewBag.FechaFin = DateTime.Now.Date.ToString("yyyy'-'MM'-'dd");
            ViewBag.Fechamala = "N";
            return View();
        }

        [HttpPost]
        public FileResult ResumenAsistencia(string fecini, string fecfin)
        {
            if (fecini == null || fecfin == null) return null;
            DateTime fechini = Convert.ToDateTime(fecini);
            DateTime fechfin = Convert.ToDateTime(fecfin);
            ViewBag.Fechamala = "N";

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string ruta = AppDomain.CurrentDomain.BaseDirectory + "/documento.xlsx";
            MemoryStream file = (MemoryStream)this.GeneraResumenAsistencia(fechini, fechfin, empresa);
            file.Position = 0;
            return File(file, "aplication/xlsx", "ResumenAsistencia.xlsx");

        }


        public Object GeneraResumenAsistencia(DateTime finicio, DateTime ftermino, string empresa)
        {
            string tit="N";
            int holguraent,ind;
            string[] ent = new string[100];
            string[] sal = new string[100];

            var Trabajadores = f.filtroTrabajadoresVigencia(0, Convert.ToBoolean(1));
            var Diastrabajador = new List<DetalleDias>();

            var infoferia = new List<GYCEmpresa.Models.remepage>();
            infoferia = db.remepage.Where(x => x.nom_tabla == "FERIADOS" && x.fec_param >= finicio && x.fec_param <= ftermino).ToList();


            remepage dettiemp = new remepage();
            var infoTiemp = new List<GYCEmpresa.Models.remepage>();
            infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
            dettiemp = infoTiemp.Where(x => "1         " == x.cod_param).SingleOrDefault();
            holguraent = 0;
            if (dettiemp != null)
                holguraent = Convert.ToInt32(dettiemp.val_param);

            SLDocument oSLDocument = new SLDocument();
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            System.Data.DataTable data1 = new System.Data.DataTable();
            int ndias = ftermino.Subtract(finicio).Days + 1;
            if (ftermino >= finicio && ndias <= 99)
            {
                for (ind = 1; ind < 100; ind++)
                {
                    ent[ind] = "Entrada";
                    sal[ind] = "Salida";
                }
                data1.Columns.Add("RUT", typeof(string));
                data1.Columns.Add("Nombre", typeof(string));
                DateTime fec = finicio;
                for(ind=1; ind < 100; ind++)
                {
                data1.Columns.Add(fec.ToString("dd'-'MM'-'yyyy"), typeof(string));
                data1.Columns.Add(Convert.ToString(ind), typeof(string));
                fec = fec.AddDays(1);
                }

                data1.Rows.Add("", ""
                    , ent[1], sal[1], ent[2], sal[2], ent[3], sal[3], ent[4], sal[4], ent[5], sal[5], ent[6], sal[6], ent[7], sal[7], ent[8], sal[8], ent[9], sal[9], ent[10], sal[10]
                    , ent[11], sal[11], ent[12], sal[12], ent[13], sal[13], ent[14], sal[14], ent[15], sal[15], ent[16], sal[16], ent[17], sal[17], ent[18], sal[18], ent[19], sal[19], ent[20], sal[20]
                    , ent[21], sal[21], ent[22], sal[22], ent[23], sal[23], ent[24], sal[24], ent[25], sal[25], ent[26], sal[26], ent[27], sal[27], ent[28], sal[28], ent[29], sal[29], ent[30], sal[30]
                    , ent[31], sal[31], ent[32], sal[32], ent[33], sal[33], ent[34], sal[34], ent[35], sal[35], ent[36], sal[36], ent[37], sal[37], ent[38], sal[38], ent[39], sal[39], ent[40], sal[40]
                    , ent[41], sal[41], ent[42], sal[42], ent[43], sal[43], ent[44], sal[44], ent[45], sal[45], ent[46], sal[46], ent[47], sal[47], ent[48], sal[48], ent[49], sal[49], ent[50], sal[50]
                    , ent[51], sal[51], ent[52], sal[52], ent[53], sal[53], ent[54], sal[54], ent[55], sal[55], ent[56], sal[56], ent[57], sal[57], ent[58], sal[58], ent[59], sal[59], ent[60], sal[60]
                    , ent[61], sal[61], ent[62], sal[62], ent[63], sal[63], ent[64], sal[64], ent[65], sal[65], ent[66], sal[66], ent[67], sal[67], ent[68], sal[68], ent[69], sal[69], ent[70], sal[70]
                    , ent[71], sal[71], ent[72], sal[72], ent[73], sal[73], ent[74], sal[74], ent[75], sal[75], ent[76], sal[76], ent[77], sal[77], ent[78], sal[78], ent[79], sal[79], ent[80], sal[80]
                    , ent[81], sal[81], ent[82], sal[82], ent[83], sal[83], ent[84], sal[84], ent[85], sal[85], ent[86], sal[86], ent[87], sal[87], ent[88], sal[88], ent[89], sal[89], ent[90], sal[90]
                    , ent[91], sal[91], ent[92], sal[92], ent[93], sal[93], ent[94], sal[94], ent[95], sal[95], ent[96], sal[96], ent[97], sal[97], ent[88], sal[98], ent[99], sal[99]);
                tit = "S";
                ind = 0;
                foreach (var t in Trabajadores)
                {

                    Diastrabajador = AsistenciaDiaria(empresa,t.RUT, finicio, ftermino, 0, infoferia, holguraent);
                   foreach (var d in Diastrabajador)
                    {
                        ind++;
                        if (d.C_INASI == "TRABAJADO")
                        {
                            entrada[ind] =d.H_ENTRADA.ToString("HH':'mm");
                            salida[ind] = d.H_SALIDA.ToString("HH':'mm");
                        }
                        else
                        {
                        entrada[ind] = d.C_INASI;
                        salida[ind] = "";
                        if (d.C_INASI == "FERIADO" || d.C_INASI == "DESCANSO") entrada[ind] = "";
                        }
                    }
                    var persona = db.PERSONA.Where(i => i.RUT == t.RUT).SingleOrDefault();
                    nom = persona.APATERNO + " " + persona.AMATERNO + " " + persona.NOMBRE;

                    data1.Rows.Add(t.RUT, nom
                , entrada[1], salida[1], entrada[2], salida[2], entrada[3], salida[3], entrada[4], salida[4], entrada[5], salida[5], entrada[6], salida[6], entrada[7], salida[7], entrada[8], salida[8], entrada[9], salida[9], entrada[10], salida[10]
                , entrada[11], salida[11], entrada[12], salida[12], entrada[13], salida[13], entrada[14], salida[14], entrada[15], salida[15], entrada[16], salida[16], entrada[17], salida[17], entrada[18], salida[18], entrada[19], salida[19], entrada[20], salida[20]
                , entrada[21], salida[21], entrada[22], salida[22], entrada[23], salida[23], entrada[24], salida[24], entrada[25], salida[25], entrada[26], salida[26], entrada[27], salida[27], entrada[28], salida[28], entrada[29], salida[29], entrada[30], salida[30]
                , entrada[31], salida[31], entrada[32], salida[32], entrada[33], salida[33], entrada[34], salida[34], entrada[35], salida[35], entrada[36], salida[36], entrada[37], salida[37], entrada[38], salida[38], entrada[39], salida[39], entrada[40], salida[40]
                , entrada[41], salida[41], entrada[42], salida[42], entrada[43], salida[43], entrada[44], salida[44], entrada[45], salida[45], entrada[46], salida[46], entrada[47], salida[47], entrada[48], salida[48], entrada[49], salida[49], entrada[50], salida[50]
                , entrada[51], salida[51], entrada[52], salida[52], entrada[53], salida[53], entrada[54], salida[54], entrada[55], salida[55], entrada[56], salida[56], entrada[57], salida[57], entrada[58], salida[58], entrada[59], salida[59], entrada[60], salida[60]
                , entrada[61], salida[61], entrada[62], salida[62], entrada[63], salida[63], entrada[64], salida[64], entrada[65], salida[65], entrada[66], salida[66], entrada[67], salida[67], entrada[68], salida[68], entrada[69], salida[69], entrada[70], salida[70]
                , entrada[71], salida[71], entrada[72], salida[72], entrada[73], salida[73], entrada[74], salida[74], entrada[75], salida[75], entrada[76], salida[76], entrada[77], salida[77], entrada[78], salida[78], entrada[79], salida[79], entrada[80], salida[80]
                , entrada[81], salida[81], entrada[82], salida[82], entrada[83], salida[83], entrada[84], salida[84], entrada[85], salida[85], entrada[86], salida[86], entrada[87], salida[87], entrada[88], salida[88], entrada[89], salida[89], entrada[90], salida[90]
                , entrada[91], salida[91], entrada[92], salida[92], entrada[93], salida[93], entrada[94], salida[94], entrada[95], salida[95], entrada[96], salida[96], entrada[97], salida[97], entrada[98], salida[98], entrada[99], salida[99]);
                ind = 0;
            }
                ndias = (ndias) * 2 + 1;
                for (ind = 199; ind > ndias; ind--)
                {
                    data1.Columns.RemoveAt(ind);
                }
        }
         else
        {
                data1.Columns.Add("Fechas ", typeof(string));
                data1.Columns.Add("Erroneas", typeof(string));
                ViewBag.Fechamala = "S";
        }
         oSLDocument.ImportDataTable(1, 1, data1, true);
         var file = new MemoryStream();
         oSLDocument.SaveAs(file);
         return file;
        }



        public JsonResult SelectFaenaVigente(int vigente)
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;

            var infofaena = new List<GYCEmpresa.Models.FAENA>();
            if (vigente == 1)
            {
                infofaena = db.FAENA.Where(i => i.EMPRESA == empresa).OrderBy(x => x.DESCRIPCION).ToList();
            }
            else
            {
                DateTime hoy = DateTime.Now.Date;
                infofaena = db.FAENA.Where(i => i.EMPRESA == empresa && i.FTERMINO < hoy).OrderBy(x => x.DESCRIPCION).ToList();
            }

            var nr = new List<ListaFaenas>();
            nr.Add(new ListaFaenas() { Id = 0, Descripcion = "Todos" });
            foreach (var i in infofaena)
            {
                nr.Add(new ListaFaenas() { Id = i.ID, Descripcion = i.DESCRIPCION });
            }
            var FaenaSL = new SelectList(nr, "Id", "Descripcion");

            List<ElementJsonStringKeyEspeciaidad> lst = new List<ElementJsonStringKeyEspeciaidad>();

            lst = (from c in nr
                   select new ElementJsonStringKeyEspeciaidad
                   {
                       Value = Convert.ToString(c.Id),
                       Text = c.Descripcion
                   }
                   ).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public class ElementJsonStringKeyEspeciaidad
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public List<ResumenDias> AsistenciaDiariaFaena(DateTime finicio, DateTime ftermino, int faena, int vigentes)
        {
            int Descanso = 0;
            int Trabajados = 0;
            int Permiso = 0;
            int Vacaciones = 0;
            int Licencia = 0;
            int Compensado = 0;
            int diastot = 0;
            int MinExtra = 0;
            int MinTarda = 0;
            int holguraent;
            var Trabajadores = f.filtroTrabajadoresVigencia(faena, Convert.ToBoolean(vigentes));
            var Diastrabajador = new List<DetalleDias>();
            remepage dettiemp = new remepage();
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;

            var infoferia = new List<GYCEmpresa.Models.remepage>();
            infoferia = db.remepage.Where(x => x.nom_tabla == "FERIADOS" && x.fec_param >= finicio && x.fec_param <= ftermino).ToList();

            var infoTiemp = new List<GYCEmpresa.Models.remepage>();
            infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
            dettiemp = infoTiemp.Where(x => "1         " == x.cod_param).SingleOrDefault();
            holguraent = 0;
            if(dettiemp != null)
            holguraent = Convert.ToInt32(dettiemp.val_param);

            var nr = new List<ResumenDias>();
            PERSONA Pers = new PERSONA();
            foreach (var t in Trabajadores)
            {
                Descanso = 0;
                Trabajados = 0;
                Permiso = 0;
                Vacaciones = 0;
                Licencia = 0;
                Compensado = 0;
                diastot = 0;
                MinExtra = 0;
                MinTarda = 0;

                //if (t.RUT == "69393527")
                //{
                Diastrabajador = AsistenciaDiaria(empresa,t.RUT, finicio, ftermino, faena, infoferia,holguraent);
                    Pers = db.PERSONA.Where(x => x.RUT == t.RUT).SingleOrDefault();
                    foreach (var d in Diastrabajador)
                    {
                        if (d.C_INASI == "TRABAJADO") Trabajados = Trabajados + 1;
                        if (d.C_INASI == "DESCANSO" || d.C_INASI == "FERIADO") Descanso = Descanso + 1;
                        if (d.C_INASI == "PERMISO") Permiso = Permiso + 1;
                        if (d.C_INASI == "LICENCIA") Licencia = Licencia + 1;
                        if (d.C_INASI == "VACACION") Vacaciones = Vacaciones + 1;
                        MinExtra = MinExtra + d.M_EXTRA;
                        MinTarda = MinTarda + d.M_TARDA;
                        diastot++;
                }

                nr.Add(new ResumenDias()
                    {
                        RUT = t.RUT,
                        A_PATERNO = Pers.APATERNO,
                        A_MATERNO = Pers.AMATERNO,
                        NOMBRE = Pers.NOMBRE,
                        CARGO = "",
                        DIAS_TRABAJADOS = Trabajados,
                        DIAS_VACACIONES = Vacaciones,
                        DIAS_LICENCIA = Licencia,
                        DIAS_PERMISO = Permiso,
                        DIAS_AUSENCIA = diastot - Trabajados - Vacaciones - Descanso - Licencia- Permiso- Compensado,
                        DIAS_DESCANSO = Descanso,
                        DIAS_TOTALES = diastot,
                        DT = 0,
                        X2 = 0,
                        HDESCUENTO = MinTarda,
                        HHEE = MinExtra,
                        HORAS_PERMISO = 0,
                        DIAS_COMPENSADOS = 0,
                    });

                //}
            }
            return nr;
        }


        public List<DetalleDias> AsistenciaDiaria(string empresa,String RUT, DateTime finicio, DateTime ftermino, int faena, List<GYCEmpresa.Models.remepage> infoFeria, int holguraent)
        {
            int Descanso = 0;
            int Trabajados = 0;
            int Permiso = 0;
            int Vacaciones = 0;
            int Licencia = 0;
            int Feriado;

            DateTime fecloop;
            int ind, diasem, extras, tardas;
            DateTime salpro,salrea;
            string rut;
            DateTime entrada, salida;
            string modent, modsal;
            string t_entrada, t_salida, codina;

            var infoMarca = new List<GYCEmpresa.Models.MARCACIONyRUT>();
            var infoLicen = new List<GYCEmpresa.Models.LICENCIAMEDICA>();
            var infoPermi = new List<GYCEmpresa.Models.PERMISOINASISTENCIA>();
            var infoVacac = new List<GYCEmpresa.Models.rhuesolv>();
            var infoTurno = new List<GYCEmpresa.Models.TurnoTrabajador>();
            var infoDescu = new List<GYCEmpresa.Models.COMPENSADO_UTILIZADO>();
            var infoExtra = new List<GYCEmpresa.Models.HHEE>();
            var infodetal = new List<GYCEmpresa.Models.TurnoDetalle>();
            TurnoDetalle turdet = new TurnoDetalle();
            TurnoTrabajador turtra = new TurnoTrabajador();
            TimeSpan TotTar = new TimeSpan();

            var detMarca = new List<GYCEmpresa.Models.MARCACIONyRUT>();
            var detLicen = new List<GYCEmpresa.Models.LICENCIAMEDICA>();
            var detPermi = new List<GYCEmpresa.Models.PERMISOINASISTENCIA>();
            var detVacac = new List<GYCEmpresa.Models.rhuesolv>();
            var detDescu = new List<GYCEmpresa.Models.COMPENSADO_UTILIZADO>();
            var detExtra = new List<GYCEmpresa.Models.HHEE>();
            var detFeria = new List<GYCEmpresa.Models.remepage>();
            var detContr = new CONTRATO();

            int diastot = ftermino.Subtract(finicio).Days + 1;
            var nrdia = new List<DetalleDias>();

            rut = RUT;
            if(rut==null) return nrdia;

            Descanso = 0;
            Trabajados = 0;
            Permiso = 0;
            Vacaciones = 0;
            Licencia = 0;
            Feriado = 0;
            modent = "";
            modsal = "";
            t_entrada = "";
            t_salida = "";

            infoTurno = db.TurnoTrabajador.Where(x => x.RutTrabajador == rut && x.FechaTerminoTurno > finicio).ToList();
            detContr = db.CONTRATO.Where(x => x.PERSONA == rut && x.EMPRESA== empresa  && x.FTERMNO > finicio && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR ==true && x.RECHAZADO ==false).SingleOrDefault();
            int tratur = 1;
            if (infoTurno.Count() > 0)
            {
                turtra = infoTurno.Last();
                if (turtra.IdTurno != null)
                    tratur = (int)turtra.IdTurno;
            }

            DateTime diasig = ftermino.Date.AddDays(1);
            infoMarca = db.MARCACIONyRUT.Where(x => x.RUT == rut && x.CHECKTIME >= finicio && x.CHECKTIME < diasig).ToList();
            infoLicen = db.LICENCIAMEDICA.Where(x => x.TRABAJADOR == rut && !((x.FINICIO < finicio && x.FTERMINO < finicio) || (x.FINICIO > ftermino && x.FTERMINO > ftermino))).ToList();
            infoPermi = db.PERMISOINASISTENCIA.Where(x => x.TRABAJADOR == rut && !((x.FINICIO < finicio && x.FTERMINO < finicio) || (x.FINICIO > ftermino && x.FTERMINO > ftermino))).ToList();
            infoVacac = db.rhuesolv.Where(x => x.nrt_ruttr == rut && !((x.fec_inicio < finicio && x.fec_termin < finicio) || (x.fec_inicio > ftermino && x.fec_termin > ftermino)) && x.est_solici == "Aceptado  ").ToList();
            infodetal = db.TurnoDetalle.Where(x => x.IdTurno == tratur).ToList();

            fecloop = finicio;
            for (ind = 1; ind <= diastot; ind++)
            {
                if(fecloop >= detContr.FINICIO && fecloop <= detContr.FTERMNO)
                {

                entrada = Convert.ToDateTime("1999-09-09");
                salida = Convert.ToDateTime("1999-09-09");
                extras = 0;
                tardas = 0;
                codina = "";
                modsal = "";
                modent = "";
                diasem = (int)fecloop.DayOfWeek;
                if (diasem == 0) diasem = 7;
                turdet = infodetal.Where(x => x.Dia == diasem).SingleOrDefault();
                t_entrada = Convert.ToString(turdet.HoraInicio);
                t_salida = Convert.ToString(turdet.HoraTermino);
                diasig = fecloop.Date.AddDays(1);

                detMarca = infoMarca.Where(x => x.CHECKTIME >= fecloop && x.CHECKTIME < diasig).ToList();
                detLicen = infoLicen.Where(x => fecloop >= x.FINICIO && fecloop <= x.FTERMINO).ToList();
                detPermi = infoPermi.Where(x => fecloop >= x.FINICIO && fecloop <= x.FTERMINO).ToList();
                detVacac = infoVacac.Where(x => fecloop >= x.fec_inicio && fecloop <= x.fec_termin).ToList();
                detExtra = db.HHEE.Where(x => x.TRABAJADOR == rut && (x.FINICIO >= fecloop && x.FINICIO < diasig)).ToList();
                detFeria = infoFeria.Where(x => fecloop == x.fec_param).ToList();
                if (detMarca.Count() > 0)
                {
                    Trabajados++;
                    codina = "TRABAJADO";
                    foreach (var m in detMarca)
                    {
                        if (m.CHECKTYPE == "O")
                        { salida = m.CHECKTIME; if (Convert.ToBoolean(m.MODIFICADA)) modsal = "*"; }
                        else
                        { entrada = m.CHECKTIME; if (Convert.ToBoolean(m.MODIFICADA)){ modent = "*"; }
                            TotTar = (TimeSpan) turdet.HoraInicio;
                            int mintur = TotTar.Minutes+ (TotTar.Hours)*60;
                            if(mintur > 0)
                            {

                            int minent = m.CHECKTIME.Hour*60 + m.CHECKTIME.Minute;
                            tardas = minent - mintur;
                            if (tardas < holguraent) tardas = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (detFeria.Count() > 0) { Feriado++; codina = "FERIADO"; }
                    else
                    {

                        if (detLicen.Count() > 0) { Licencia++; codina = "LICENCIA"; }
                        else
                        {
                            if (detPermi.Count() > 0) { Permiso++; codina = "PERMISO"; }
                            else
                            {
                                if (detVacac.Count() > 0) { Vacaciones++; codina = "VACACION"; }
                                else
                                {
                                    string paso = Convert.ToString(turdet.TotalHora);
                                    int hrs = Convert.ToInt32(paso.Substring(0, 2));

                                    if (hrs == 0 || diasem == 7)
                                    {
                                        Descanso = Descanso + 1; codina = "DESCANSO";
                                    }

                                }
                            }
                        }
                    }
                    //Horas extras
                }
                if (detExtra.Count() > 0)
                {
                    foreach (var t in detExtra)
                    {
                        salpro = t.FINICIO;
                        salrea = t.FTERMINO;
                        extras = (salrea.Subtract(salpro).Hours)*60+ salrea.Subtract(salpro).Minutes;
                    }
                }

                nrdia.Add(new DetalleDias()
                {
                    RUT = rut,
                    FECHA = fecloop,
                    C_INASI = codina,
                    H_ENTRADA = entrada,
                    H_SALIDA = salida,
                    M_ENTRADA = modent,
                    M_SALIDA = modsal,
                    T_ENTRADA = t_entrada,
                    T_SALIDA = t_salida,
                    D_DESCANSO = Descanso,
                    D_TOTALES = Feriado,
                    M_EXTRA = extras,
                    M_TARDA = tardas,
                    M_PERMI=0
                });
                }
                fecloop = fecloop.AddDays(1);
            }
            return nrdia;
        }
        public List<OfertaHorasExtras> OfertaHorasExtras(DateTime finicio, DateTime ftermino, int faena, int horasTrabajo, int minTrabajo)
        {
            int cnt=0,ind;
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;
            var nrextra = new List<OfertaHorasExtras>();
            PERSONA Pers = new PERSONA();
            CONTRATO Cont = new CONTRATO();
            var Extras = new List<HHEE>();

            int holgura;
            remepage dettiemp = new remepage();
            var infoTiemp = new List<GYCEmpresa.Models.remepage>();
            infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
            dettiemp = infoTiemp.Where(x => "2         " == x.cod_param).SingleOrDefault();
            holgura = 0;
            if (dettiemp != null)
                holgura = Convert.ToInt32(dettiemp.val_param);
            DateTime diasig = ftermino.AddDays(1).Date;
            DateTime diasigloop = ftermino.AddDays(1).Date;
            DateTime fecloop,entrada,salida;
            entrada = Convert.ToDateTime("1999-09-09");
            salida = Convert.ToDateTime("1999-09-09");
            var Trabajadores = f.filtroTrabajadoresVigencia(faena, true);
            var detMarca = new List<GYCEmpresa.Models.MARCACIONyRUT>();
            foreach (var t in Trabajadores)
            {
                Pers = db.PERSONA.Where(x => x.RUT == t.RUT).SingleOrDefault();
                Cont = db.CONTRATO.Where(x => x.PERSONA == t.RUT && x.FTERMNO >= finicio && x.FIRMAEMPRESA== true && x.FIRMATRABAJADOR  == true && x.RECHAZADO == false).SingleOrDefault();

                var infoMarca = db.MARCACIONyRUT.Where(x => x.RUT == t.RUT && x.CHECKTIME >= finicio && x.CHECKTIME < diasig).ToList();
                fecloop = finicio;
                cnt = ftermino.Subtract(finicio).Days;
                for(ind=1; ind <= cnt; ind++)
                {
                    diasigloop = fecloop.AddDays(1);
                    detMarca = infoMarca.Where(x => x.CHECKTIME >= fecloop && x.CHECKTIME < diasigloop).ToList();
                    if(detMarca.Count > 1)
                    {
                        foreach (var m in detMarca)
                        {
                            if (m.CHECKTYPE == "O")
                            { salida = m.CHECKTIME; }
                            else
                            {
                              entrada = m.CHECKTIME; 
                            }
                        }
                        int minext = HorasExtrasEmerg(salida, entrada, horasTrabajo,minTrabajo, holgura);
                        if(minext > 0)
                        {
                            Extras = db.HHEE.Where(x => x.TRABAJADOR == t.RUT && x.FINICIO >= fecloop && x.FINICIO < diasigloop).ToList();
                            if (Extras.Count == 0)
                            {

                            DateTime fectur = entrada.AddHours(horasTrabajo);
                            int hrs = minext / 60;
                            int min = minext - hrs * 60;
                            string hrsext = hrs.ToString("0#") + ":" + min.ToString("0#");
                            nrextra.Add(new OfertaHorasExtras()
                            {
                                RUT = t.RUT,
                                NOMBRE = Pers.APATERNO + " " + Pers.AMATERNO + " " + Pers.NOMBRE,
                                CARGO = Cont.CARGO,
                                FECHA = Convert.ToString(fecloop.Date),
                                MARCA_ENTRADA = Convert.ToString(entrada),
                                MARCA_SALIDA = Convert.ToString(salida),
                                FIN_TURNO = Convert.ToString(fectur),
                                HHEE = hrsext
                            }) ;
                           }
                        }
                    }
                    fecloop = fecloop.AddDays(1);
                }

            }
            return nrextra;
        }
       public List<OfertaHorasExtrasTurno> OfertaHorasExtrasTurno(DateTime finicio, DateTime ftermino, int faena, int minutos, string jornada)
        {
            int cnt = 0, ind;
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;
            var nrextra = new List<OfertaHorasExtrasTurno>();
            PERSONA Pers = new PERSONA();
            CONTRATO Cont = new CONTRATO();
            var infoTurno = new List<GYCEmpresa.Models.TurnoTrabajador>();
            TurnoDetalle turdet = new TurnoDetalle();
            TurnoTrabajador turtra = new TurnoTrabajador();
            var infodetal = new List<GYCEmpresa.Models.TurnoDetalle>();
            var Extras = new List<HHEE>();
            string t_entrada, t_salida;
            int t_minut;
            int holgura;
            remepage dettiemp = new remepage();
            var infoTiemp = new List<GYCEmpresa.Models.remepage>();
            infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
            dettiemp = infoTiemp.Where(x => "2         " == x.cod_param).SingleOrDefault();
            holgura = 0;
            if (dettiemp != null)
                holgura = Convert.ToInt32(dettiemp.val_param);
            DateTime diasig = ftermino.AddDays(1).Date;
            DateTime diasigloop = ftermino.AddDays(1).Date;
            DateTime fecloop, entrada, salida;
            entrada = Convert.ToDateTime("1999-09-09");
            salida = Convert.ToDateTime("1999-09-09");
            finicio = finicio.Date;
            ftermino = ftermino.Date;
            var Trabajadores = f.filtroTrabajadoresVigencia(faena, true);
            var detMarca = new List<GYCEmpresa.Models.MARCACIONyRUT>();
 
            foreach (var t in Trabajadores)
            {
                Pers = db.PERSONA.Where(x => x.RUT == t.RUT).SingleOrDefault();
                Cont = db.CONTRATO.Where(x => x.PERSONA == t.RUT && x.FTERMNO >= finicio && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false).SingleOrDefault();
                infoTurno = db.TurnoTrabajador.Where(x => x.RutTrabajador == t.RUT && x.FechaTerminoTurno > finicio).ToList();
                int tratur = 1;
                if (infoTurno.Count() > 0)
                {
                    turtra = infoTurno.Last();
                    if (turtra.IdTurno != null)
                        tratur = (int)turtra.IdTurno;
                }
                infodetal = db.TurnoDetalle.Where(x => x.IdTurno == tratur).ToList();

                var infoMarca = db.MARCACIONyRUT.Where(x => x.RUT == t.RUT && x.CHECKTIME >= finicio && x.CHECKTIME < diasig).ToList();
                fecloop = finicio.Date;
                cnt = ftermino.Subtract(finicio).Days+1;
                for (ind = 1; ind <= cnt; ind++)
                {
                    diasigloop = fecloop.AddDays(1);
                    int diasem = (int)fecloop.DayOfWeek;
                    detMarca = infoMarca.Where(x => x.CHECKTIME >= fecloop && x.CHECKTIME < diasigloop).ToList();
                    if (detMarca.Count > 1 )
                    {
                        if (diasem == 0) diasem = 7;
                        turdet = infodetal.Where(x => x.Dia == diasem).SingleOrDefault();
                        t_entrada =Convert.ToString(turdet.HoraInicio);
                        t_salida = Convert.ToString(turdet.HoraTermino);
                        string hrstur = Convert.ToString(turdet.TotalHora);
                        t_minut = Convert.ToInt32(hrstur.Substring(0, 2)) * 60 + Convert.ToInt32(hrstur.Substring(3, 2));
                        foreach (var m in detMarca)
                        {
                            if (m.CHECKTYPE == "O")
                            { salida = m.CHECKTIME; }
                            else
                            {
                              entrada = m.CHECKTIME;
                            }
                        }
                        int minext = HorasExtrasTurno(salida, entrada, jornada, minutos,t_entrada,t_salida,t_minut);
                        if (minext > 0)
                        {
                            Extras = db.HHEE.Where(x => x.TRABAJADOR == t.RUT && x.FINICIO >= fecloop && x.FINICIO < diasigloop).ToList();
                            if (Extras.Count == 0)
                            {

                                int hrs = minext / 60;
                                int min = minext - hrs * 60;
                                string hrsext = hrs.ToString("0#") + ":" + min.ToString("0#");
                                string fecpas = fecloop.ToString("yyyy'-'MM'-'dd");
                                ViewBag.valor = fecpas;
                                nrextra.Add(new OfertaHorasExtrasTurno()
                                {
                                    RUT = t.RUT,
                                    NOMBRE = Pers.APATERNO + " " + Pers.AMATERNO + " " + Pers.NOMBRE,
                                    CARGO = Cont.CARGO,
                                    FECHA = fecpas,
                                    JORNADA = jornada,
                                    MARCACION = Convert.ToString(salida.TimeOfDay),
                                    FINICIO = salida.ToString("yyyy'-'MM'-'dd' 'HH':'mm"),
                                    HHEE = hrsext
                                });
                            }
                        }
                    }
                    fecloop = fecloop.AddDays(1);
                }
            }
            return nrextra;
        }
        public int HorasExtrasTurno(DateTime fecsal, DateTime fecent,string jornada,int holgura,string tent,string tsal, int t_minut)
        {
            int minut =0, minmar,mintur;
            if(jornada == "T")
            {
                mintur = Convert.ToInt32(tsal.Substring(0, 2))*60+ Convert.ToInt32(tsal.Substring(3, 2));
                minmar = fecsal.Hour*60+ fecsal.Minute;

                if (minmar > mintur)
                {
                    minut = minmar - mintur;
                    if (minut < holgura) { minut = 0; }
                }
            }
            if (jornada == "M")
            {
                mintur = Convert.ToInt32(tent.Substring(0, 2)) * 60 + Convert.ToInt32(tent.Substring(3, 2));
                minmar = fecent.Hour * 60 + fecent.Minute;
                if (mintur > minmar)
                {
                    minut = mintur - minmar;
                    if (minut < holgura) { minut = 0; }
                }
            }
             if (jornada == "FUERA TURNO" || t_minut ==0)
            {
                mintur = (Convert.ToInt32(tsal.Substring(0, 2)) * 60 + Convert.ToInt32(tsal.Substring(3, 2)))-(Convert.ToInt32(tent.Substring(0, 2)) * 60 + Convert.ToInt32(tent.Substring(3, 2)));
                minmar = (fecsal.Hour * 60 + fecsal.Minute )- (fecent.Hour * 60 + fecent.Minute);
                if (minmar > mintur)
                {
                    minut = minmar - mintur;
                    if (minut < holgura) { minut = 0; }
                }
            }
            return minut;
        }
        public int HorasExtrasEmerg(DateTime fecha1, DateTime fecha2,int horas, int minutos, int holgura)
        {
            int minut = 0, mintrab =0, minplan;
            if (fecha1 > fecha2)
            {
                int hrs = fecha1.Subtract(fecha2).Hours;
                int min = fecha1.Subtract(fecha2).Minutes;
                mintrab = hrs * 60 + min;
                minplan = horas * 60+ minutos;
                minut = mintrab - minplan;
                if (minut < holgura) minut = 0;
            }
            return minut;
        }

    }
}


namespace GYCEmpresa.Models
{

    public class ResumenDias
    {
        public string RUT { get; set; }

        public string A_PATERNO { get; set; }

        public string A_MATERNO { get; set; }

        public string NOMBRE { get; set; }

        public string CARGO { get; set; }

        public int DIAS_TRABAJADOS { get; set; }

        public int DIAS_VACACIONES { get; set; }

        public int DIAS_LICENCIA { get; set; }

        public int DIAS_PERMISO { get; set; }

        public int DIAS_DESCANSO { get; set; }
        public int DIAS_TOTALES { get; set; }

        public int DIAS_AUSENCIA { get; set; }

        public int DT { get; set; }

        public int X2 { get; set; }

        public int HDESCUENTO { get; set; }

        public int HHEE { get; set; }

        public int HORAS_PERMISO { get; set; }

        public int DIAS_COMPENSADOS { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class DetalleDias
        {
            public string RUT { get; set; }
            public DateTime FECHA { get; set; }
            public String C_INASI { get; set; }
            public DateTime H_ENTRADA { get; set; }

            public DateTime H_SALIDA { get; set; }
            public string M_ENTRADA { get; set; }

            public string M_SALIDA { get; set; }
            public string T_ENTRADA { get; set; }

            public string T_SALIDA { get; set; }
            public int M_EXTRA { get; set; }

            public int M_TARDA { get; set; }
            public int M_PERMI { get; set; }
            public string H_TRABA { get; set; }
            public string H_TURNO { get; set; }
            public int D_DESCANSO { get; set; }
            public int D_TOTALES { get; set; }

        }
  }
namespace GYCEmpresa.Models
{
    public class InformeFinal
    {
        public int id;
        public string fecha { get; set; }
        public string diasem { get; set; }
        public string inasistencia { get; set; }
        public string turnoentrada { get; set; }
        public string turnosalida { get; set; }
        public string marcaentrada { get; set; }
        public string manualentrada { get; set; }
        public string marcasalida { get; set; }
        public string manualsalida { get; set; }
        public string horasturno { get; set; }
        public string horastrabajadas { get; set; }
        public string retrasos { get; set; }
        public string sobretiempos { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class Informedia
    {
        public string CARGO { get; set; }
        public int TOTAL_CONTRATADOS { get; set; }
        public int PERMISOINASISTENCIA { get; set; }
        public int LICENCIA { get; set; }
        public int VACACIONES { get; set; }
        public int PRESENTES { get; set; }
        public int AUSENTES { get; set; }
        public string TIPO { get; set; }

    }
}
namespace GYCEmpresa.Models
{
    public class OfertaHorasExtras
 {
        public string RUT { get; set; }
        public string NOMBRE { get; set; }
        public string CARGO { get; set; }
        public string FECHA { get; set; }
        public string MARCA_ENTRADA { get; set; }
        public string MARCA_SALIDA { get; set; }
        public string FIN_TURNO { get; set; }
        public string HHEE { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class OfertaHorasExtrasTurno
    {
        public string RUT { get; set; }
        public string NOMBRE { get; set; }
        public string CARGO { get; set; }
        public string FECHA { get; set; }
        public string JORNADA { get; set; }
        public string MARCACION { get; set; }
        public string FINICIO { get; set; }
        public string HHEE { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class HorasDescuento
    {
        public string RUT { get; set; }
        public string NOMBRE { get; set; }
        public string FECHA { get; set; }
        public string MARCACION { get; set; }
        public string HENTRADA { get; set; }
        public string HSALIDA { get; set; }
        public string HENTRADA2 { get; set; }
        public string HSALIDA2 { get; set; }
        public string MINUTOS { get; set; }
        public string HHDD { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class detcabezera
    {
        public string empresa { get; set; }
        public string cargo { get; set; }
        public string rut { get; set; }
        public string nombre { get; set; }
        public DateTime ingreso { get; set; }
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
    }
}
