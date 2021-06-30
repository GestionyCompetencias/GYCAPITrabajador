using System;
using GYCEmpresa.App_Start;
using GYCEmpresa.Models;
using Rotativa;
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
    public class APITrabajadorController : Controller
    {
        // GET: APITrabajador
        dbgycEntities3 db = new dbgycEntities3();
        dbgyc2DataContext db2 = new dbgyc2DataContext();
        Funciones f = new Funciones();
        private rhuecalc rhuecalc = new rhuecalc();

        public virtual JsonResult ExistePersonaDetalle(string rut)
        {
            //string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            string empresa = "76895853K";
            rut = "69393527";
            string EXISTE = "N";
            DateTime hoy = DateTime.Now.Date;
            var pers = (db.PERSONA.Where(x => x.RUT == rut)).SingleOrDefault();
            int DIAS;
            string CODIGO;
            DateTime FFERIADO, FSINDICATO;
            var empr = (db.EMPRESA.Where(x => x.RUT == empresa)).SingleOrDefault();
            string RAZONSOCIAL = empr.RSOCIAL;
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
                EXISTE = "S";
                String FNACIM = pers.FNACIMIENTO.ToString("yyyy'-'MM'-'dd");
                String FINICI = cont.FINICIO.ToString("yyyy'-'MM'-'dd");
                String FTERMI = cont.FTERMNO.ToString("yyyy'-'MM'-'dd");
                String FFERIA = FFERIADO.ToString("yyyy'-'MM'-'dd");
                String FSINDI = FSINDICATO.ToString("yyyy'-'MM'-'dd");
                string CIUDADD = ciud.DESCRIPCION;
                int FAENAPAS = cont.FAENA;

                int SUELDO = cont.MBRUTO;
                string codsin = Convert.ToString(CODIGO) + "          ";
                codsin = codsin.Substring(0, 10);
                int codnac = Convert.ToInt32(pers.NACIONALIDAD);
                string REGIOND = db.REGION.Where(x => x.ID == pers.REGION).Select(x => x.DESCRIPCION).SingleOrDefault();
                string COMUNAD = db.COMUNA.Where(x => x.ID == pers.COMUNA).Select(x => x.DESCRIPCION).SingleOrDefault();
                string BANCOD = db.BANCO.Where(x => x.ID == pers.BANCO).Select(x => x.DESCRIPCION).SingleOrDefault();
                string TCUENTAD = db.TIPOCUENTA.Where(x => x.ID == pers.TCUENTA).Select(x => x.DESCRIPCION).SingleOrDefault();
                string ECIVILD = db.ECIVIL.Where(x => x.ID == pers.ECIVIL).Select(x => x.DESCRIPCION).SingleOrDefault();
                string SALUDD = db.SALUD.Where(x => x.ID == pers.SALUD).Select(x => x.DESCRIPCION).SingleOrDefault();
                string PREVISIOND = db.PREVISION.Where(x => x.ID == pers.PREVISION).Select(x => x.DESCRIPCION).SingleOrDefault();
                var tur = db.TurnoTrabajador.Where(x => x.RutTrabajador == pers.RUT).ToList();
                int turno = 1;
                foreach (var u in tur)
                {
                    turno = (int)u.IdTurno;
                }
                string TURNOD = db.Turno.Where(x => x.IdTurno == turno).Select(x => x.Descripcion).SingleOrDefault();
                string SINDICATO = db.remepage.Where(x => x.nom_tabla == "SINDICATO" && x.cod_param == codsin).Select(x => x.gls_param).SingleOrDefault();
                string TIPOCONTRATOD = db.TIPOCONTRATO.Where(x => x.ID == cont.TCONTRATO).Select(x => x.DESCRIPCION).SingleOrDefault();
                string NACIONALIDADD = db.NACIONALIDAD.Where(x => x.ID == codnac).Select(x => x.PAIS).SingleOrDefault();
                string FAENAD = db.FAENA.Where(x => x.ID == FAENAPAS).Select(x => x.DESCRIPCION).SingleOrDefault();
                string SUELDOD = Convert.ToString(SUELDO);
                string DIASD = Convert.ToString(DIAS);
                var trabajador = new List<detalle>();
                trabajador = CreaSalida(cont, pers, FNACIM, NACIONALIDADD, REGIOND, CIUDADD, COMUNAD,
                     BANCOD, TCUENTAD, ECIVILD, SALUDD, PREVISIOND, FAENAD, TURNOD, FINICI,
                     FTERMI, FFERIA, DIASD, FSINDI, SINDICATO, RAZONSOCIAL, TIPOCONTRATOD, SUELDOD, EXISTE);
                return Json(new
                {
                    trabajador
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                PERSONA pers1 = new PERSONA();
                pers1.NOMBRE = null;
                pers1.APATERNO = null;
                pers1.AMATERNO = null;
                string FNACIM = null;
                pers1.NACIONALIDAD = null;
                pers1.TELEFONO1 = 0;
                pers1.TELEFONO2 = 0;
                pers1.CORREO = null;
                pers1.DIRECCION = null;
                pers1.REGION = 0;
                pers1.CIUDAD = 0;
                pers1.COMUNA = 0;
                pers1.SEXO = null;
                pers1.BANCO = 0;
                pers1.TCUENTA = 0;
                pers1.NCUENTA = null;
                pers1.ECIVIL = 0;
                pers1.NHIJOS = 0;
                pers1.SALUD = 0;
                pers1.ADICIONALSALUD = 0;
                pers1.PREVISION = 0;
                pers1.APV = 0;
                pers1.AHORRO = 0;
                pers1.EMPRESAACTUAL = null;
                pers1.ESTADOCONTRATUAL = null;
                string FINICI = hoy.ToString("yyyy'-'MM'-'dd");
                string FTERMI = "2099-12-31";
                string FFERIA = hoy.ToString("yyyy'-'MM'-'dd");
                DIAS = 0;
                string FSINDI = hoy.ToString("yyyy'-'MM'-'dd");
                String SUELDOD = "0";
                string NACIONALIDADD = null;
                string DIASD = null;
                string FAENAD = null;
                string REGIOND = null;
                string COMUNAD = null;
                string CIUDADD = null;
                string BANCOD = null;
                string TCUENTAD = null;
                string ECIVILD = null;
                string SALUDD = null;
                string PREVISIOND = null;
                string TURNOD = null;
                string SINDICATO = null;
                string TIPOCONTRATOD = null;
                CONTRATO contra = new CONTRATO();
                var trabajador = new List<detalle>();
                trabajador = CreaSalida(contra, pers, FNACIM, NACIONALIDADD, REGIOND, CIUDADD, COMUNAD,
                     BANCOD, TCUENTAD, ECIVILD, SALUDD, PREVISIOND, FAENAD, TURNOD, FINICI,
                     FTERMI, FFERIA, DIASD, FSINDI, SINDICATO, RAZONSOCIAL, TIPOCONTRATOD, SUELDOD, EXISTE);
                return Json(new
                {
                    trabajador
                }, JsonRequestBehavior.AllowGet);

            }
        }
        public List<detalle> CreaSalida(CONTRATO cont, PERSONA pers, string FNACIM, string NACIONALIDADD, string REGIOND, string CIUDADD, string COMUNAD,
            string BANCOD, string TCUENTAD, string ECIVILD, string SALUDD, string PREVISIOND, string FAENAPAS, string TURNOD, string FINICID,
            string FTERMID, string FFERIAD, string DIASD, string FSINDID, string SINDICATOD, string RAZONSOCIALD,
            string TIPOCONTRATOD, string SUELDOD, string EXISTED)
        {
            var nr = new List<detalle>();
            nr.Add(new detalle()
            {
                NOMBRE = pers.NOMBRE,
                APATERNO = pers.APATERNO,
                AMATERNO = pers.AMATERNO,
                FNACIMIENTO = FNACIM,
                NACIONALIDAD = NACIONALIDADD,
                TELEFONO1 = Convert.ToString(pers.TELEFONO1),
                TELEFONO2 = Convert.ToString(pers.TELEFONO2),
                CORREO = pers.CORREO,
                DIRECCION = pers.DIRECCION,
                REGION = REGIOND,
                CIUDAD = CIUDADD,
                COMUNA = COMUNAD,
                SEXO = pers.SEXO,
                BANCO = BANCOD,
                TCUENTA = TCUENTAD,
                NCUENTA = pers.NCUENTA,
                ECIVIL = ECIVILD,
                NHIJOS = Convert.ToString(pers.NHIJOS),
                SALUD = SALUDD,
                ADICIONALSALUD = Convert.ToString(pers.ADICIONALSALUD),
                PREVISION = PREVISIOND,
                APV = Convert.ToString(pers.APV),
                AHORRO = Convert.ToString(pers.AHORRO),
                EMPRESAACTUAL = pers.EMPRESAACTUAL,
                ESTADOCONTRATUAL = pers.ESTADOCONTRATUAL,
                FAENA = FAENAPAS,
                TURNO = TURNOD,
                FINICI = FINICID,
                FTERMI = FTERMID,
                FFERIA = FFERIAD,
                DIAS = DIASD,
                FSINDI = FSINDID,
                SINDICATO = SINDICATOD,
                RAZONSOCIAL = RAZONSOCIALD,
                CARGO = cont.CARGO,
                TIPOCONTRATO = TIPOCONTRATOD,
                SUELDO = SUELDOD,
                EXISTE = EXISTED
            });
            return nr;
        }
        public virtual JsonResult MarcacionesTrabajador(string trabajador, string fechaInicio, string fechaFin)
        {

            string empresa = "76895853K";
            trabajador = "69393527";
            DateTime fecIni = Convert.ToDateTime("2021-06-01");
            DateTime fecFin = Convert.ToDateTime("2021-06-30");
            var marcas = db.JVC_MARCACIONES(fecIni, fecFin, 0, trabajador, empresa);
            return Json(new
            {
                marcas
            }, JsonRequestBehavior.AllowGet);

        }

        public virtual JsonResult SolicitudVacaciones(string trabajador, string fechaInicio, string fechaFin)
        {

            trabajador = "69393527";
            DateTime fecIni = Convert.ToDateTime("2021-06-01");
            DateTime fecFin = Convert.ToDateTime("2021-06-30");
            var vacacion = rhuecalc.ProcesoVacaciones(trabajador, fecIni, fecFin, 1, "N");

            return Json(new
            {
                vacacion
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult SolicitudCompensacion(string trabajador, string dias)
        {

            trabajador = "69393527";
            int ndias = Convert.ToInt32(dias);
            var compensacion = rhuecalc.ProcesoCompensacion(trabajador, ndias, 1, "N");

            return Json(new
            {
                compensacion
            }, JsonRequestBehavior.AllowGet);

        }
        public virtual JsonResult Periodos(string Trabajador)
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            var persona = (db.PERSONA.Where(x => x.RUT == Trabajador)).SingleOrDefault();
            Trabajador = "69393527";
            var infoPeri = db.rhueperi.Where(v => v.nrt_ruttr == Trabajador).ToList();
            var periodos = new List<GYCEmpresa.Models.rhueperi>();
            int año = 0;
            DateTime hoy = DateTime.Now;
            CONTRATO con = db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO > hoy).SingleOrDefault();
            DateTime ingreso = con.FINICIO;
            string horario = "ADM";
            rhuedias dias = db.rhuedias.Where(x => x.cod_horar == horario).SingleOrDefault();
            int legal = 0;
            int contr = 0;
            int admin = 0;
            int dfaena = 0;
            int especi = 0;
            int otros = 0;

            if (con != null)
            {
                foreach (var p in infoPeri)
                {
                    periodos.Add(p);
                    año = p.ano_termino;
                }
                if (año == 0)
                {
                    año = DateTime.Now.Year;
                    string fecs = año + "-" + ingreso.Month + "-" + ingreso.Day;
                    ingreso = Convert.ToDateTime(fecs);
                    if (ingreso > hoy)
                        año = año - 1;
                }
                int diastot = (int)(dias.dias_legal + dias.dias_contr + dias.dias_admin + dias.dias_faena + dias.dias_especi + dias.dias_otros);
                int propor = f.Dias_Proporcionales(con.FINICIO, diastot);
                if (propor > dias.dias_legal)
                { legal = (int)dias.dias_legal; propor = propor - (int)dias.dias_legal; }
                else { legal = propor; propor = 0; }

                if (propor > dias.dias_contr)
                { contr = (int)dias.dias_contr; propor = propor - (int)dias.dias_contr; }
                else { contr = propor; propor = 0; }

                if (propor > dias.dias_admin)
                { admin = (int)dias.dias_admin; propor = propor - (int)dias.dias_admin; }
                else { admin = propor; propor = 0; }

                if (propor > dias.dias_faena)
                { dfaena = (int)dias.dias_faena; propor = propor - (int)dias.dias_faena; }
                else { dfaena = propor; propor = 0; }

                if (propor > dias.dias_especi)
                { especi = (int)dias.dias_especi; propor = propor - (int)dias.dias_especi; }
                else { especi = propor; propor = 0; }

                if (propor > dias.dias_otros)
                { otros = (int)dias.dias_otros; propor = propor - (int)dias.dias_otros; }
                else { otros = propor; propor = 0; }
                periodos.Add(new rhueperi
                {
                    nrt_ruttr = Trabajador,
                    ano_inicio = año,
                    ano_termino = año + 1,
                    dias_legal = legal,
                    dias_progr = 0,
                    dias_contr = contr,
                    dias_admin = admin,
                    dias_faena = dfaena,
                    dias_especi = especi,
                    dias_otros = otros,
                    fec_trans = DateTime.Now,
                    rut_empr = empresa
                });
            }
            return Json(new
            {
                periodos
            }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult CtaCte(string Trabajador)
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            List<ListCuentaCorriente> cta = new List<ListCuentaCorriente>();
            Trabajador = "69393527";

            var ctacte = new List<ListCuentaCorriente>();

            var infoPeriodos = new List<GYCEmpresa.Models.rhueperi>();
            infoPeriodos = db.rhueperi.Where(i => i.nrt_ruttr == Trabajador).ToList();

            foreach (var p in infoPeriodos)
            {
                ctacte.Add(new ListCuentaCorriente()
                {
                    nrt_ruttr = Convert.ToString(p.nrt_ruttr),
                    periodo = p.ano_inicio + "-" + p.ano_termino,
                    diastot = Convert.ToString((int)(p.dias_legal + p.dias_progr + p.dias_admin + p.dias_contr + p.dias_especi + p.dias_faena + p.dias_otros)),
                    finivac = " ",
                    ftervac = " ",
                    diasusa = " ",
                    diasacu = " ",
                    fcompe = " ",
                    diascom = " ",
                    diasocu = " ",
                    diaspen = " ",
                    tipouso = "P",
                    tiporden = "P"
                });
            }
            var infoUsos = new List<GYCEmpresa.Models.rhueusos>();
            infoUsos = db.rhueusos.Where(i => i.nrt_ruttr == Trabajador && i.tip_uso == "V").ToList();

            foreach (var p in infoUsos)
            {

                var sol = (db.rhuesolv.Where(x => x.nrt_ruttr == p.nrt_ruttr && x.nro_solici == p.nro_solici)).SingleOrDefault();
                if (sol.est_solici.Substring(0, 2) == "Ac")
                {
                    ctacte.Add(new ListCuentaCorriente()
                    {
                        nrt_ruttr = Convert.ToString(p.nrt_ruttr),
                        periodo = p.ano_inicio + "-" + p.ano_termino,
                        diastot = " ",
                        finivac = p.fec_inivac.ToString("dd'-'MM'-'yyyy"),
                        ftervac = p.fec_tervac.ToString("dd'-'MM'-'yyyy"),
                        diasusa = Convert.ToString((int)(p.dias_legal + p.dias_progr + p.dias_admin + p.dias_contr + p.dias_especi + p.dias_faena + p.dias_otros)),
                        diasacu = " ",
                        fcompe = " ",
                        diascom = " ",
                        diasocu = " ",
                        diaspen = " ",
                        tipouso = "V",
                        tiporden = "X"
                    });
                }
            }
            var infoComp = new List<GYCEmpresa.Models.rhueusos>();
            infoComp = db.rhueusos.Where(i => i.nrt_ruttr == Trabajador && i.tip_uso == "C").ToList();

            foreach (var p in infoComp)
            {
                var solco = (db.rhuesolc.Where(x => x.nrt_ruttr == p.nrt_ruttr && x.nro_solici == p.nro_solici)).SingleOrDefault();
                if (solco.est_solici.Substring(0, 2) == "Ac")
                {
                    ctacte.Add(new ListCuentaCorriente()
                    {
                        nrt_ruttr = Convert.ToString(p.nrt_ruttr),
                        periodo = p.ano_inicio + "-" + p.ano_termino,
                        diastot = " ",
                        finivac = p.fec_inivac.ToString("dd'-'MM'-'yyyy"),
                        ftervac = " ",
                        diasusa = " ",
                        diasacu = " ",
                        fcompe = p.fec_inivac.ToString("dd'-'MM'-'yyyy"),
                        diascom = Convert.ToString((int)(p.dias_legal + p.dias_progr + p.dias_admin + p.dias_contr + p.dias_especi + p.dias_faena + p.dias_otros)),
                        diasocu = " ",
                        diaspen = " ",
                        tipouso = "C",
                        tiporden = "X"
                    });
                }
            }
            var ctapas = new List<ListCuentaCorriente>(ctacte.OrderBy(x => x.periodo).ThenBy(x => x.finivac).ThenBy(x => x.finivac));
            int acumu = 0;
            int ocupa = 0;
            int pendi = 0;
            string perio = null;
            foreach (var c in ctapas)
            {
                if (perio == c.periodo)
                {
                    if (c.tipouso == "P")
                    {
                        perio = c.periodo;
                        pendi = Convert.ToInt32(c.diastot);
                    }
                    if (c.tipouso == "V")
                    {
                        acumu = acumu + Convert.ToInt32(c.diasusa);
                        ocupa = ocupa + Convert.ToInt32(c.diasusa);
                        pendi = pendi - Convert.ToInt32(c.diasusa);
                        c.diasocu = Convert.ToString(ocupa);
                        c.diasacu = Convert.ToString(acumu);
                        c.diaspen = Convert.ToString(pendi);
                    }
                    if (c.tipouso == "C")
                    {
                        ocupa = ocupa + Convert.ToInt32(c.diascom);
                        pendi = pendi - Convert.ToInt32(c.diascom);
                        c.diasocu = Convert.ToString(ocupa);
                        c.diaspen = Convert.ToString(pendi);
                        //c.finivac = "";
                    }
                }
                else
                {
                    perio = c.periodo;
                    pendi = Convert.ToInt32(c.diastot);
                    ocupa = 0;
                    acumu = 0;
                }

            }
            var cuentacorriente = new List<ListCuentaCorriente>(ctacte.OrderByDescending(x => x.periodo).ThenBy(x => x.tiporden).ThenByDescending(x => x.finivac));
            return Json(new
            {
                cuentacorriente
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Utilizados(string Trabajador)
        {
            Trabajador = "69393527";
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            var persona = (db.PERSONA.Where(x => x.RUT == Trabajador)).SingleOrDefault();
            var infoUsos = new List<GYCEmpresa.Models.rhueusos>();
            infoUsos = db.rhueusos.Where(i => i.nrt_ruttr == Trabajador && i.tip_uso == "V").ToList();
            var usados = new List<rhueusos>();
            foreach (var p in infoUsos)
            {
                var sol = (db.rhuesolv.Where(x => x.nrt_ruttr == p.nrt_ruttr && x.nro_solici == p.nro_solici)).SingleOrDefault();
                if (sol.est_solici.Substring(0, 2) == "Ac")
                {
                    usados.Add(new rhueusos()
                    {
                        nrt_ruttr = p.nrt_ruttr,
                        fec_inivac = p.fec_inivac,
                        fec_tervac = p.fec_tervac,
                        fec_transa = p.fec_transa,
                        ano_inicio = p.ano_inicio,
                        ano_termino = p.ano_termino,
                        tip_uso = p.tip_uso,
                        nro_solici = p.nro_solici,
                        dias_admin = p.dias_admin,
                        dias_contr = p.dias_contr,
                        dias_corri = p.dias_corri,
                        dias_especi = p.dias_especi,
                        dias_faena = p.dias_faena,
                        dias_legal = p.dias_legal,
                        dias_otros = p.dias_otros,
                        dias_progr = p.dias_progr,
                        rut_empr = p.rut_empr,
                    });
                }
            }
            return Json(new
            {
                usados
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LicenciasMedicas(string Trabajador, string fecini, string fecfin)
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            Trabajador = "69393527";
            DateTime finicio = DateTime.Now.Date.AddYears(-20);
            DateTime ftermino = DateTime.Now.Date;
            //List<LICENCIAMEDICA> Licencias = new List<LICENCIAMEDICA>();
            var licencias = db.LICENCIAMEDICA.Where(x => x.TRABAJADOR == Trabajador && x.FINICIO >= finicio && x.FINICIO <= ftermino).ToList();
            return Json(new
            {
                licencias
            }, JsonRequestBehavior.AllowGet);
        }
    }
}

namespace GYCEmpresa.Models
{

    public class detalle
        {
            public string NOMBRE { get; set; }
            public string APATERNO { get; set; }
            public string AMATERNO { get; set; }
            public string FNACIMIENTO { get; set; }
            public string NACIONALIDAD { get; set; }
            public string TELEFONO1 { get; set; }
            public string TELEFONO2 { get; set; }
            public string CORREO { get; set; }
            public string DIRECCION { get; set; }
            public string REGION { get; set; }
            public string CIUDAD { get; set; }
            public string COMUNA { get; set; }
            public string SEXO { get; set; }
            public string BANCO { get; set; }
            public string TCUENTA { get; set; }
            public string NCUENTA { get; set; }
            public string ECIVIL { get; set; }
            public string NHIJOS { get; set; }
            public string SALUD { get; set; }
            public string ADICIONALSALUD { get; set; }
            public string PREVISION { get; set; }
            public string APV { get; set; }
            public string AHORRO { get; set; }
            public string EMPRESAACTUAL { get; set; }
            public string ESTADOCONTRATUAL { get; set; }
            public string FAENA { get; set; }
            public string TURNO { get; set; }
            public string FINICI { get; set; }
            public string FTERMI { get; set; }
            public string FFERIA { get; set; }
            public string DIAS { get; set; }
            public string FSINDI { get; set; }
            public string SINDICATO { get; set; }
            public string RAZONSOCIAL { get; set; }
            public string CARGO { get; set; }
            public string TIPOCONTRATO { get; set; }
            public string SUELDO { get; set; }
            public string EXISTE { get; set; }

        }

    }
