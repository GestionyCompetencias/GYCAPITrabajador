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
    public class MaestrosController : Controller
    {
        dbgycEntities3 db = new dbgycEntities3();
        dbgyc2DataContext db2 = new dbgyc2DataContext();
        Funciones f = new Funciones();

        public virtual JsonResult ExistePersona(string rut)
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            string Existe = "N";
            DateTime hoy = DateTime.Now.Date;
            var pers = (db.PERSONA.Where(x => x.RUT == rut)).SingleOrDefault();
            int DIAS;
            string CODIGO;
            DateTime FFERIADO, FSINDICATO;
            var empr = (db.EMPRESA.Where(x => x.RUT == empresa)).SingleOrDefault();
            string Rsocial = empr.RSOCIAL;
            if (pers != null)
            {

                var cont = (db.CONTRATO.Where(x => x.PERSONA == rut && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
                var feri = (db.FERIADOESPECIAL.Where(x => x.nrt_ruttr == rut)).SingleOrDefault();
                var sind = (db.SINDICATOTRABAJADOR.Where(x => x.nrt_ruttr == rut)).SingleOrDefault();
                var ciud = (db.CIUDAD.Where(x => x.ID == pers.CIUDAD)).SingleOrDefault();
                var turn = (db.TurnoTrabajador.Where(x => x.RutTrabajador == rut && hoy >= x.FechaInicioTurno && x.FechaTerminoTurno >= hoy)).ToList();
                int TURNO = 1;
                if (turn == null)
                {
                    foreach (var t in turn)
                    {
                        TURNO = (int)t.IdTurno;
                    }
                }
                DIAS = 0;
                FFERIADO = hoy;
                if (feri != null)
                {
                    DIAS = (int)feri.dias_especi;
                    FFERIADO = (DateTime)feri.fec_feriado;
                }
                FSINDICATO = hoy;
                CODIGO = null;
                if (sind != null)
                {
                    FSINDICATO = (DateTime)sind.fec_afili;
                    CODIGO = sind.cod_sindi;
                }
                Existe = "S";
                String FNACIM = pers.FNACIMIENTO.ToString("yyyy'-'MM'-'dd");
                String FINICI = cont.FINICIO.ToString("yyyy'-'MM'-'dd");
                String FTERMI = cont.FTERMNO.ToString("yyyy'-'MM'-'dd");
                String FFERIA = FFERIADO.ToString("yyyy'-'MM'-'dd");
                String FSINDI = FSINDICATO.ToString("yyyy'-'MM'-'dd");
                string CIUD = ciud.DESCRIPCION;
                int FAENAPAS = cont.FAENA;
                int Sueldo = cont.MBRUTO;
                string Tipcon = Convert.ToString(cont.TCONTRATO) + "          ";
                Tipcon = Tipcon.Substring(0, 10);
                return Json(new
                {
                    pers.NOMBRE,
                    pers.APATERNO,
                    pers.AMATERNO,
                    FNACIM,
                    pers.NACIONALIDAD,
                    pers.TELEFONO1,
                    pers.TELEFONO2,
                    pers.CORREO,
                    pers.DIRECCION,
                    pers.REGION,
                    pers.CIUDAD,
                    pers.COMUNA,
                    pers.SEXO,
                    pers.BANCO,
                    pers.TCUENTA,
                    pers.NCUENTA,
                    pers.ECIVIL,
                    pers.NHIJOS,
                    pers.SALUD,
                    pers.ADICIONALSALUD,
                    pers.PREVISION,
                    pers.APV,
                    pers.AHORRO,
                    pers.EMPRESAACTUAL,
                    pers.ESTADOCONTRATUAL,
                    FAENAPAS,
                    TURNO,
                    FINICI,
                    FTERMI,
                    FFERIA,
                    DIAS,
                    FSINDI,
                    CODIGO,
                    Rsocial,
                    cont.CARGO,
                    cont.TCONTRATO,
                    Sueldo,
                    Tipcon,
                    Existe

                });
            }
            else
            {
                PERSONA pers1 = new PERSONA();
                pers1.NOMBRE = null;
                pers1.APATERNO = null;
                pers1.AMATERNO = null;
                string FNACIM = null;
                pers1.NACIONALIDAD = "40";
                pers1.TELEFONO1 = 0;
                pers1.TELEFONO2 = 0;
                pers1.CORREO = null;
                pers1.DIRECCION = null;
                pers1.REGION = 1;
                pers1.CIUDAD = 1;
                pers1.COMUNA = 1;
                pers1.SEXO = "MASCULINO";
                pers1.BANCO = 6;
                pers1.TCUENTA = 1;
                pers1.NCUENTA = null;
                pers1.ECIVIL = 2;
                pers1.NHIJOS = 0;
                pers1.SALUD = 1;
                pers1.ADICIONALSALUD = 0;
                pers1.PREVISION = 1;
                pers1.APV = 0;
                pers1.AHORRO = 0;
                pers1.EMPRESAACTUAL = null;
                pers1.ESTADOCONTRATUAL = null;
                int FAENAPAS = 1;
                int TURNO = 1;
                string FINICI = hoy.ToString("yyyy'-'MM'-'dd");
                string FTERMI = "2099-12-31";
                string FFERIA = hoy.ToString("yyyy'-'MM'-'dd");
                DIAS = 0;
                string FSINDI = hoy.ToString("yyyy'-'MM'-'dd");
                CODIGO = "2";
                string CARGO = null;
                string TCONTRATO = "1";
                int Sueldo = 0;
                return Json(new
                {
                    pers1.NOMBRE,
                    pers1.APATERNO,
                    pers1.AMATERNO,
                    FNACIM,
                    pers1.NACIONALIDAD,
                    pers1.TELEFONO1,
                    pers1.TELEFONO2,
                    pers1.CORREO,
                    pers1.DIRECCION,
                    pers1.REGION,
                    pers1.CIUDAD,
                    pers1.COMUNA,
                    pers1.SEXO,
                    pers1.BANCO,
                    pers1.TCUENTA,
                    pers1.NCUENTA,
                    pers1.ECIVIL,
                    pers1.NHIJOS,
                    pers1.SALUD,
                    pers1.ADICIONALSALUD,
                    pers1.PREVISION,
                    pers1.APV,
                    pers1.AHORRO,
                    pers1.EMPRESAACTUAL,
                    pers1.ESTADOCONTRATUAL,
                    FAENAPAS,
                    TURNO,
                    FINICI,
                    FTERMI,
                    FFERIA,
                    DIAS,
                    FSINDI,
                    CODIGO,
                    Rsocial,
                    CARGO,
                    TCONTRATO,
                    Sueldo,
                    Existe
                });

            }
        }
        public virtual JsonResult GuardarTrabajador(string rut, string exi)
        {
            ViewBag.Mensaje = "Trabajador registrado existosamente";

            return Json(new { });

        }
        //Prueba de nuevo template.
        [HttpGet]
        public ActionResult EditaTrabajador(String RUT)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            //if (f.PermisoPorServicio(90) == false)
            //{
            //    return Redirect("~/Home/ErrorPorPermiso");
            //}
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            var model = (db.remepers.Where(x => x.nrt_ruttr == RUT)).SingleOrDefault();

            var Region = db.REGION.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var RegionSL = new SelectList(Region, "Id", "Descripcion");
            ViewBag.Region = RegionSL;


            var Comuna = db.COMUNA.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var ComunaSL = new SelectList(Comuna, "Id", "Descripcion");
            ViewBag.Comuna = ComunaSL;

            var Ciudad = db.CIUDAD.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var CiudadSL = new SelectList(Ciudad, "Id", "Descripcion");
            ViewBag.Ciudad = CiudadSL;


            var Nacionalidad = db.NACIONALIDAD.Select(x => new { Id = x.ID, Descripcion = x.PAIS }).ToList().OrderBy(x => x.Descripcion);
            var NacionalidadSL = new SelectList(Nacionalidad, "Id", "Descripcion");
            ViewBag.Nacionalidad = NacionalidadSL;


            var Banco = db.BANCO.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var BancoSL = new SelectList(Banco, "Id", "Descripcion");
            ViewBag.Banco = BancoSL;

            var TipoCuenta = db.TIPOCUENTA.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var TipoCuentaSL = new SelectList(TipoCuenta, "Id", "Descripcion");
            ViewBag.TipoCuenta = TipoCuentaSL;

            var EstadoCivil = db.ECIVIL.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var EstadoCivilSL = new SelectList(EstadoCivil, "Id", "Descripcion");
            ViewBag.EstadoCivil = EstadoCivilSL;

            var Salud = db.SALUD.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var SaludSL = new SelectList(Salud, "Id", "Descripcion");
            ViewBag.Salud = SaludSL;

            var Prevision = db.PREVISION.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var PrevisionSL = new SelectList(Prevision, "Id", "Descripcion");
            ViewBag.Prevision = PrevisionSL;

            List<SelectListItem> Sexo = new List<SelectListItem>();
            Sexo.Add(new SelectListItem() { Text = "MASCULINO", Value = "MASCULINO" });
            Sexo.Add(new SelectListItem() { Text = "FEMENINO", Value = "FEMENINO" });
            var SexoSL = new SelectList(Sexo, "Text", "Text");
            ViewBag.Sexo = SexoSL;

            List<SelectListItem> EstadoContractual = new List<SelectListItem>();
            EstadoContractual.Add(new SelectListItem() { Text = "CONTRATADO", Value = "1" });
            EstadoContractual.Add(new SelectListItem() { Text = "DISPONIBLE", Value = "2" });
            var EstadoContractualSL = new SelectList(EstadoContractual, "Text", "Text");
            ViewBag.EstadoContractual = EstadoContractualSL;

            List<SelectListItem> Genera1 = new List<SelectListItem>();
            Genera1.Add(new SelectListItem() { Text = "GENERA CONTRATO", Value = "S" });
            Genera1.Add(new SelectListItem() { Text = "NO GENERA", Value = "N" });
            var GeneraSL = new SelectList(Genera1, "Text", "Text");
            ViewBag.Genera = GeneraSL;

            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faena = FaenaSL;

            var TurnoSL = f.Turnos_General();
            ViewBag.Turno = TurnoSL;
            ViewBag.feini = DateTime.Now.Date;
            ViewBag.fefer = DateTime.Now.Date;
            ViewBag.fecsin = DateTime.Now.Date;
            ViewBag.fecfin = Convert.ToDateTime("2099-12-31");

            var Sindicatos = db.remepage.Where(x => x.nom_tabla == "SINDICATO" && x.rut_empr == empresa).Select(x => new { Id = x.cod_param, Descripcion = x.gls_param }).ToList().OrderBy(x => x.Descripcion);
            var SindicatosSL = new SelectList(Sindicatos, "Id", "Descripcion");
            ViewBag.Sindicatos = SindicatosSL;

            var Cargos = db.remepage.Where(x => x.nom_tabla == "CARGO" && x.rut_empr == empresa).Select(x => new { Id = x.gls_param, Descripcion = x.gls_param }).ToList().OrderBy(x => x.Descripcion);
            var CargoSL = new SelectList(Cargos, "Id", "Descripcion");
            ViewBag.Cargo = CargoSL;

            var Tipcon = db.remepage.Where(x => x.nom_tabla == "CONTRATO" && x.rut_empr == "1         ").Select(x => new { Id = x.cod_param, Descripcion = x.gls_param }).ToList().OrderBy(x => x.Descripcion);
            var TipconSL = new SelectList(Tipcon, "Id", "Descripcion");
            ViewBag.Tipcon = TipconSL;

            EMPRESA emp = new EMPRESA();
            emp = db.EMPRESA.Where(x => x.RUT == empresa).SingleOrDefault();
            List<SelectListItem> RsocialList = new List<SelectListItem> { new SelectListItem() { Text = emp.RSOCIAL, Value = "E" }, };
            ViewBag.Rsocial = new SelectList(RsocialList, "Text", "Text");
            ViewBag.Mensaje = "x";
            return View(model);
        }

        [HttpPost]
        public ActionResult EditaTrabajador(FormCollection formCollection, PERSONA model, String Genera, int Faena, int Turno, string FINICIO
            , string FTERMINO, string Fecsind, string Sindicato, string Feriado, string Diasfer, string Rsocial, string Tipcon
            , string Cargo, string Observ, string Sueldo, string Existe)
        {
            string btnvolver, btnguardar;
            btnvolver = formCollection["Volver"];
            btnguardar = formCollection["Guardar"];
            if (btnvolver != null) { return Redirect("~/Home/Index"); }

            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            DateTime FINICIOS = Convert.ToDateTime(FINICIO);
            DateTime FTERMINOS = Convert.ToDateTime(FTERMINO);
            DateTime FecsindS = Convert.ToDateTime(Fecsind);
            DateTime FeriadoS = Convert.ToDateTime(Feriado);

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();

            ViewBag.feini = DateTime.Now.Date;
            ViewBag.fecfin = Convert.ToDateTime("2099-12-31");

            var Sindicatos = db.remepage.Where(x => x.nom_tabla == "SINDICATO" && x.rut_empr == empresa).Select(x => new { Id = x.cod_param, Descripcion = x.gls_param }).ToList().OrderBy(x => x.Descripcion);
            var SindicatosSL = new SelectList(Sindicatos, "Id", "Descripcion");
            ViewBag.Sindicatos = SindicatosSL;


            var Region = db.REGION.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var RegionSL = new SelectList(Region, "Id", "Descripcion");
            ViewBag.Region = RegionSL;


            var Comuna = db.COMUNA.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var ComunaSL = new SelectList(Comuna, "Id", "Descripcion");
            ViewBag.Comuna = ComunaSL;

            var Ciudad = db.CIUDAD.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var CiudadSL = new SelectList(Ciudad, "Id", "Descripcion");
            ViewBag.Ciudad = CiudadSL;


            var Nacionalidad = db.NACIONALIDAD.Select(x => new { Id = x.ID, Descripcion = x.PAIS }).ToList().OrderBy(x => x.Descripcion);
            var NacionalidadSL = new SelectList(Nacionalidad, "Id", "Descripcion");
            ViewBag.Nacionalidad = NacionalidadSL;


            var Banco = db.BANCO.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var BancoSL = new SelectList(Banco, "Id", "Descripcion");
            ViewBag.Banco = BancoSL;

            var TipoCuenta = db.TIPOCUENTA.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var TipoCuentaSL = new SelectList(TipoCuenta, "Id", "Descripcion");
            ViewBag.TipoCuenta = TipoCuentaSL;

            var EstadoCivil = db.ECIVIL.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var EstadoCivilSL = new SelectList(EstadoCivil, "Id", "Descripcion");
            ViewBag.EstadoCivil = EstadoCivilSL;

            var Salud = db.SALUD.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var SaludSL = new SelectList(Salud, "Id", "Descripcion");
            ViewBag.Salud = SaludSL;

            var Prevision = db.PREVISION.Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var PrevisionSL = new SelectList(Prevision, "Id", "Descripcion");
            ViewBag.Prevision = PrevisionSL;

            List<SelectListItem> Sexo = new List<SelectListItem>();
            Sexo.Add(new SelectListItem() { Text = "MASCULINO", Value = "MASCULINO" });
            Sexo.Add(new SelectListItem() { Text = "FEMENINO", Value = "FEMENINO" });
            var SexoSL = new SelectList(Sexo, "Text", "Text");
            ViewBag.Sexo = SexoSL;

            List<SelectListItem> EstadoContractual = new List<SelectListItem>();
            EstadoContractual.Add(new SelectListItem() { Text = "CONTRATADO", Value = "1" });
            EstadoContractual.Add(new SelectListItem() { Text = "DISPONIBLE", Value = "2" });
            var EstadoContractualSL = new SelectList(EstadoContractual, "Text", "Text");
            ViewBag.EstadoContractual = EstadoContractualSL;

            List<SelectListItem> Genera1 = new List<SelectListItem>();
            Genera1.Add(new SelectListItem() { Text = "GENERA CONTRATO", Value = "S" });
            Genera1.Add(new SelectListItem() { Text = "NO GENERA", Value = "N" });
            var GeneraSL = new SelectList(Genera1, "Text", "Text");
            ViewBag.Genera = GeneraSL;

            //var Faena = db.FAENA.Where(x => x.EMPRESA == empresa).Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList().OrderBy(x => x.Descripcion);
            var FaenaSL = f.FaenasEmpresa();
            ViewBag.Faena = FaenaSL;
            var TurnoSL = f.Turnos_General();
            ViewBag.Turno = TurnoSL;

            var Cargos = db.remepage.Where(x => x.nom_tabla == "CARGO" && x.rut_empr == empresa).Select(x => new { Id = x.gls_param, Descripcion = x.gls_param }).ToList().OrderBy(x => x.Descripcion);
            var CargoSL = new SelectList(Cargos, "Id", "Descripcion");
            ViewBag.Cargo = CargoSL;

            var Tipcon1 = db.remepage.Where(x => x.nom_tabla == "CONTRATO" && x.rut_empr == "1         ").Select(x => new { Id = x.cod_param, Descripcion = x.gls_param }).ToList().OrderBy(x => x.Descripcion);
            var TipconSL = new SelectList(Tipcon1, "Id", "Descripcion");
            ViewBag.Tipcon = TipconSL;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

            EMPRESA emp = new EMPRESA();
            emp = db.EMPRESA.Where(x => x.RUT == empresa).SingleOrDefault();
            List<SelectListItem> RsocialList = new List<SelectListItem> { new SelectListItem() { Text = emp.RSOCIAL, Value = "E" }, };
            ViewBag.Rsocial = new SelectList(RsocialList, "Text", "Text");
            if (model.NHIJOS == null) model.NHIJOS = 0;

            try
            {

                if (Existe == "S" && model.RUT != null)
                {
                    string rut = model.RUT.Replace("-", "");
                    FINICIO = FINICIOS.ToString("yyyy'-'MM'-'dd");
                    FINICIOS = Convert.ToDateTime(FINICIO);
                    FTERMINO = FTERMINOS.ToString("yyyy'-'MM'-'dd");
                    FTERMINOS = Convert.ToDateTime(FTERMINO);
                    string comando = "SET [Nombre] = '" + model.NOMBRE + "', [APATERNO] = '" + model.APATERNO + "', [AMATERNO] = '" + model.AMATERNO +
                    "', [FNACIMIENTO] = '" + model.FNACIMIENTO + "', [NACIONALIDAD] = '" + model.NACIONALIDAD + "', [TELEFONO1] = '" + model.TELEFONO1 +
                    "', [TELEFONO2] = '" + model.TELEFONO2 + "', [CORREO] = '" + model.CORREO + "', [DIRECCION] = '" + model.DIRECCION +
                    "', [REGION] = '" + model.REGION + "', [CIUDAD] = '" + model.CIUDAD +
                    "', [COMUNA] = '" + model.COMUNA + "', [SEXO] = '" + model.SEXO + "', [BANCO]= '" + model.BANCO +
                    "', [TCUENTA]= '" + model.TCUENTA +
                    "', [NCUENTA]= '" + model.NCUENTA +
                    "', [ECIVIL]= '" + model.ECIVIL +
                    "', [NHIJOS]= '" + model.NHIJOS +
                    "', [SALUD]= '" + model.SALUD +
                    "', [ADICIONALSALUD]= '" + model.ADICIONALSALUD +
                    "', [PREVISION]= '" + model.PREVISION +
                    "', [APV]= '" + model.APV +
                    "', [AHORRO]= '" + model.AHORRO +
                    "', [EMPRESAACTUAL]= '" + model.EMPRESAACTUAL +
                    "', [ESTADOCONTRATUAL]= '" + model.ESTADOCONTRATUAL +
                    "' WHERE [RUT] = '" + rut + "'";
                    db.Database.ExecuteSqlCommand("UPDATE PERSONA " + comando);

                    comando = "SET [FINICIO] = '" + FINICIO + "', [FTERMNO] = '" + FTERMINO + "', [TCONTRATO] = '" + Tipcon +
                    "', [FAENA] = '" + Faena + "', [CARGO] = '" + Cargo + "', [MBRUTO] = '" + Sueldo +
                    "' WHERE [PERSONA] = '" + rut + "'";
                    db.Database.ExecuteSqlCommand("UPDATE CONTRATO " + comando);
                    ViewBag.Mensaje = "Trabajador Modificado existosamente";
                    return View();
                }
                //model.ADICIONALSALUD = double.Parse(model.ADICIONALSALUD.Value.ToString().Replace(",", "."));
                //model.APV = double.Parse(model.APV.Value.ToString().Replace(",", "."));
                //model.AHORRO = double.Parse(model.AHORRO.Value.ToString().Replace(",", "."));
                model.RUT = model.RUT.Replace("-", "");
                if (model.NCONTACTOEM == null)
                {
                    model.NCONTACTOEM = string.Empty;
                }
                db.PERSONA.Add(model);
                db.SaveChanges();
                if (Genera.Substring(0, 1) == "G")
                {
                    CONTRATO con = new CONTRATO();
                    con.NUMCONTRATOEMPRESA = null;
                    con.PERSONA = model.RUT;
                    con.EMPRESA = empresa;
                    con.FINICIO = FINICIOS;
                    con.FTERMNO = FTERMINOS;
                    con.TCONTRATO = 1;
                    con.FAENA = Faena;
                    con.CARGO = "EMPLEADO";
                    con.AREA = "ADMIN";
                    con.JORNADA = 1;
                    con.HORDINARIAS = 45;
                    con.MBRUTO = 0;
                    con.OBSERVACIONES = "CONTRATO GENERADO";
                    con.PDF = null;
                    con.FIRMATRABAJADOR = true;
                    con.FIRMAEMPRESA = true;
                    con.VALIDOPOR = null;
                    con.FECHAVALIDO = null;
                    con.RECHAZADO = false;

                    con.FECHA_CREACION = DateTime.Now.Date;
                    db.CONTRATO.Add(con);
                    db.SaveChanges();

                    //var turtra = db.TurnoTrabajador.Where(x => x.RutTrabajador == model.RUT).SingleOrDefault();
                    //if (turtra != null)
                    //{
                    TurnoTrabajador tur = new TurnoTrabajador();
                    tur.IdTurno = Turno;
                    tur.RutTrabajador = model.RUT;
                    tur.FechaInicioTurno = DateTime.Now.Date;
                    tur.FechaTerminoTurno = Convert.ToDateTime("2021-12-31");
                    tur.HorasSemana = 45;
                    tur.MinutosSemana = 15;
                    tur.FechaCreacion = DateTime.Now.Date;
                    db.TurnoTrabajador.Add(tur);
                    db.SaveChanges();

                    //Feriados Especiales
                    FERIADOESPECIAL fer = new FERIADOESPECIAL();
                    fer.nrt_ruttr = model.RUT;
                    fer.fec_feriado = FeriadoS;
                    fer.dias_especi = Convert.ToInt32(Diasfer);
                    db.FERIADOESPECIAL.Add(fer);
                    db.SaveChanges();

                    //Sindicato
                    SINDICATOTRABAJADOR sin = new SINDICATOTRABAJADOR();
                    sin.nrt_ruttr = model.RUT;
                    sin.cod_sindi = Sindicato;
                    sin.fec_afili = FecsindS;
                    db.SINDICATOTRABAJADOR.Add(sin);
                    db.SaveChanges();


                    //}

                }


                ViewBag.Mensaje = "Trabajador registrado existosamente";

            }
            catch (Exception e2)
            {
                string Mensaje = e2.Message;
                ViewBag.Mensaje = "Error al registrar al trabajador. Verifique la información ingresada o que el trabajador no se encuentre registrado en el sistema";
                return View();
            }



            return View();
        }
        public ActionResult PerfilTrabajador()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;
            List<SelectListItem> TrabajadorList = new List<SelectListItem>();
            TrabajadorList.Add(new SelectListItem() { Text = "Seleccione trabajador", Value = "1" });
            var TrabajadoresSL = new SelectList(TrabajadorList, "Value", "Text");
            ViewBag.Trabajadores = TrabajadoresSL;
            ViewBag.vigenteSelected = 1;
            ViewBag.error0 = "N";
            ViewBag.error1 = "N";
            ViewBag.error2 = "N";
            ViewBag.error3 = "N";
            ViewBag.error4 = "N";
            ViewBag.exito = "N";
            return View();
        }

        [HttpPost]
        public ActionResult PerfilTrabajador(String Trabajador, int perfil)
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            var FaenaSL = f.FaenasEmpresaTodos(1);
            ViewBag.Faenas = FaenaSL;
            PERMISO perm = new PERMISO();
            USUARIO usua = new USUARIO();
            CLIENTE clie = new CLIENTE();
            ViewBag.error0 = "N";
            ViewBag.error1 = "N";
            ViewBag.error2 = "N";
            ViewBag.error3 = "N";
            ViewBag.error4 = "N";
            ViewBag.exito = "N";

            if (Trabajador != null)
            {
                var infoclie = db.CLIENTE.Where(x => x.RUT == Trabajador).ToList();
                if (infoclie.Count != 0)
                {
                    ViewBag.error0 = "S";
                    return View();
                }
                else
                {
                    PERSONA pers = new PERSONA();
                    pers = db.PERSONA.Where(x => x.RUT == Trabajador).SingleOrDefault();
                    clie.RUT = Trabajador;
                    clie.APATERNO = pers.APATERNO;
                    clie.AMATERNO = pers.AMATERNO;
                    clie.NOMBRES = pers.NOMBRE;
                    clie.CONTACTO = pers.CORREO;
                    if (perfil > 3) { return View(); }
                    var infousu = db.USUARIO.Where(x => x.RUT == Trabajador).ToList();
                    if (infousu.Count != 0)
                    {
                        ViewBag.error1 = "S";
                        return View();
                    }
                    else
                    {
                        usua.RUT = Trabajador;
                        usua.CONTRASENA = Trabajador.Substring(0, 4);
                        usua.HABILITADO = true;

                        var infoperm = db.PERMISO.Where(x => x.RUT == Trabajador && x.EMPRESA == empresa).ToList();
                        if (infoperm.Count != 0)
                        {
                            ViewBag.error2 = "S";
                            return View();
                        }
                        else
                        {
                            perm.RUT = Trabajador;
                            perm.EMPRESA = empresa;
                            perm.PERMISO1 = perfil;
                            var infopermv2 = db.PERMISO_V2.Where(x => x.RUT == Trabajador && x.EMPRESA == empresa).ToList();
                            if (infopermv2.Count != 0)
                            {
                                ViewBag.error3 = "S";
                                return View();
                            }
                            else
                            {
                                var infoserv = db.SERVICIOCONTRATADO.Where(x => x.USUARIO == Trabajador && x.EMPRESA == empresa).ToList();
                                if (infoserv.Count != 0)
                                {
                                    ViewBag.error4 = "S";
                                    return View();
                                }
                            }
                        }
                    }
                }
                db.CLIENTE.Add(clie);
                db.SaveChanges();
                db.USUARIO.Add(usua);
                db.SaveChanges();
                db.PERMISO.Add(perm);
                db.SaveChanges();

                PERMISO_V2 permV2 = new PERMISO_V2();
                var infopermv20 = db.PERMISO_V2.Where(x => x.RUT == "69393527" && x.EMPRESA == empresa).ToList();
                foreach (var v2 in infopermv20)
                {
                    permV2.RUT = Trabajador;
                    permV2.EMPRESA = empresa;
                    permV2.PERMISO = v2.PERMISO;
                    permV2.SERVICIO = v2.SERVICIO;
                    db.PERMISO_V2.Add(permV2);
                    db.SaveChanges();

                }
                SERVICIOCONTRATADO serv = new SERVICIOCONTRATADO();
                var infoserv1 = db.SERVICIOCONTRATADO.Where(x => x.USUARIO == "69393527" && x.EMPRESA == empresa).ToList();
                foreach (var s in infoserv1)
                {
                    serv.USUARIO = Trabajador;
                    serv.EMPRESA = empresa;
                    serv.SERVICIO = s.SERVICIO;
                    serv.FINICIO = s.FINICIO;
                    serv.FTERMINO = s.FTERMINO;
                    serv.UNIDADES = s.UNIDADES;
                    serv.OBSERVACION = s.OBSERVACION;
                    db.SERVICIOCONTRATADO.Add(serv);
                    db.SaveChanges();

                }
                ViewBag.exito = "S";

            }
            return View();
        }
        public ActionResult CargaMaestro()
        {
            if (!f.isLogged()) { return Redirect("~/Home/Login"); }

            if (System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString() != "1")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
    }
}