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
        private ControlAsistenciaController Asistencia= new ControlAsistenciaController();
        private AsAdministController Permiso = new AsAdministController();

        public virtual JsonResult ExistePersonaDetalle(string rut)
        {
            string EXISTE = "N";
            DateTime hoy = DateTime.Now.Date;
            var pers = (db.PERSONA.Where(x => x.RUT == rut)).SingleOrDefault();
            int DIAS;
            string CODIGO;
            DateTime FFERIADO, FSINDICATO;
            string RAZONSOCIAL=null;
            if (pers != null)
            {

                var cont = (db.CONTRATO.Where(x => x.PERSONA == rut && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
                var feri = (db.FERIADOESPECIAL.Where(x => x.nrt_ruttr == rut)).SingleOrDefault();
                var sind = (db.SINDICATOTRABAJADOR.Where(x => x.nrt_ruttr == rut)).SingleOrDefault();
                var ciud = (db.CIUDAD.Where(x => x.ID == pers.CIUDAD)).SingleOrDefault();
                var turn = (db.TurnoTrabajador.Where(x => x.RutTrabajador == rut && hoy >= x.FechaInicioTurno && x.FechaTerminoTurno >= hoy)).ToList();
                int TURNO = 1;
                string empresa = cont.EMPRESA;
                var empr = (db.EMPRESA.Where(x => x.RUT == empresa)).SingleOrDefault();
                RAZONSOCIAL = empr.RSOCIAL;
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
                var trabajador = new detalle();
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
                var trabajador = new detalle();
                trabajador = CreaSalida(contra, pers, FNACIM, NACIONALIDADD, REGIOND, CIUDADD, COMUNAD,
                     BANCOD, TCUENTAD, ECIVILD, SALUDD, PREVISIOND, FAENAD, TURNOD, FINICI,
                     FTERMI, FFERIA, DIASD, FSINDI, SINDICATO, RAZONSOCIAL, TIPOCONTRATOD, SUELDOD, EXISTE);
                return Json(new
                {
                    trabajador
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public detalle CreaSalida(CONTRATO cont, PERSONA pers, string FNACIM, string NACIONALIDADD, string REGIOND, string CIUDADD, string COMUNAD,
            string BANCOD, string TCUENTAD, string ECIVILD, string SALUDD, string PREVISIOND, string FAENAPAS, string TURNOD, string FINICID,
            string FTERMID, string FFERIAD, string DIASD, string FSINDID, string SINDICATOD, string RAZONSOCIALD,
            string TIPOCONTRATOD, string SUELDOD, string EXISTED)
        {
                var nr = new detalle();
                nr.nombre = pers.NOMBRE;
                nr.apaterno = pers.APATERNO;
                nr.amaterno = pers.AMATERNO;
                nr.fnacimiento = FNACIM;
                nr.nacionalidad = NACIONALIDADD;
                nr.telefono1 = Convert.ToString(pers.TELEFONO1);
                nr.telefono2 = Convert.ToString(pers.TELEFONO2);
                nr.correo = pers.CORREO;
                nr.direccion = pers.DIRECCION;
                nr.region = REGIOND;
                nr.ciudad = CIUDADD;
                nr.comuna = COMUNAD;
                nr.sexo = pers.SEXO;
                nr.banco = BANCOD;
                nr.tcuenta = TCUENTAD;
                nr.ncuenta = pers.NCUENTA;
                nr.ecivil = ECIVILD;
                nr.nhijos = Convert.ToString(pers.NHIJOS);
                nr.salud = SALUDD;
                nr.adicionalsalud = Convert.ToString(pers.ADICIONALSALUD);
                nr.prevision = PREVISIOND;
                nr.apv = Convert.ToString(pers.APV);
                nr.ahorro = Convert.ToString(pers.AHORRO);
                nr.empresaactual = pers.EMPRESAACTUAL;
                nr.estadocontractual = pers.ESTADOCONTRATUAL;
                nr.faena = FAENAPAS;
                nr.turno = TURNOD;
                nr.finicio = FINICID;
                nr.ftermi = FTERMID;
                nr.fferia = FFERIAD;
                nr.dias = DIASD;
                nr.fsindi = FSINDID;
                nr.sindicato = SINDICATOD;
                nr.razonsocial = RAZONSOCIALD;
                nr.cargo = cont.CARGO;
                nr.tipocontrato = TIPOCONTRATOD;
                nr.sueldo = SUELDOD;
                nr.existe = EXISTED;
            return nr;
        }
        public virtual JsonResult MarcacionesTrabajador(string trabajador, string fechaInicio, string fechaFin)
        {

            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            DateTime fecIni = Convert.ToDateTime(fechaInicio);
            DateTime fecFin = Convert.ToDateTime(fechaFin);
            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            string empresa = cont.EMPRESA;
            var marcas = db.JVC_MARCACIONES(fecIni, fecFin, 0, trabajador, empresa);
            return Json(new
            {
                marcas
            }, JsonRequestBehavior.AllowGet);

        }

        public virtual JsonResult SolicitudVacaciones(string trabajador, string fechaInicio, string fechaFin)
        {

            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            DateTime fecIni = Convert.ToDateTime(fechaInicio);
            DateTime fecFin = Convert.ToDateTime(fechaFin);
            var vacacion = rhuecalc.ProcesoVacaciones(trabajador, fecIni, fecFin, 1, "N");

            return Json(new
            {
                vacacion
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult SolicitudCompensacion(string trabajador, string dias)
        {

            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            int ndias = Convert.ToInt32(dias);
            var compensacion = rhuecalc.ProcesoCompensacion(trabajador, ndias, 1, "N");

            return Json(new
            {
                compensacion
            }, JsonRequestBehavior.AllowGet);

        }
        public virtual JsonResult Periodos(string Trabajador)
        {
            var persona = (db.PERSONA.Where(x => x.RUT == Trabajador)).SingleOrDefault();
 
            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            var infoPeri = db.rhueperi.Where(v => v.nrt_ruttr == Trabajador).ToList();
            var periodos = new List<GYCEmpresa.Models.rhueperi>();
            int año = 0;
            DateTime hoy = DateTime.Now;
            var con = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            string empresa = con.EMPRESA;
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
            List<ListCuentaCorriente> cta = new List<ListCuentaCorriente>();
            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";

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

        public virtual JsonResult Utilizados(string Trabajador)
        {
            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
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
        public virtual JsonResult LicenciasMedicas(string Trabajador, string fecini, string fecfin)
        {
            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            DateTime finicio = DateTime.Now.Date.AddYears(-20);
            DateTime ftermino = DateTime.Now.Date;
            //List<LICENCIAMEDICA> Licencias = new List<LICENCIAMEDICA>();
            var licencias = db.LICENCIAMEDICA.Where(x => x.TRABAJADOR == Trabajador && x.FINICIO >= finicio && x.FINICIO <= ftermino).ToList();
            return Json(new
            {
                licencias
            }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult Registro(string Trabajador, DateTime finicio, DateTime ffinal)
        {
            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;

            int holguraent;
            remepage dettiemp = new remepage();
            var infoferia = new List<GYCEmpresa.Models.remepage>();
            infoferia = db.remepage.Where(x => x.nom_tabla == "FERIADOS" && x.fec_param >= finicio && x.fec_param <= ffinal).ToList();
            var infoTiemp = new List<GYCEmpresa.Models.remepage>();
            infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
            dettiemp = infoTiemp.Where(x => "1         " == x.cod_param).SingleOrDefault();
            holguraent = 0;
            if (dettiemp != null)
                holguraent = Convert.ToInt32(dettiemp.val_param);

            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            var registro = Asistencia.InformeIndividual(empresa,Trabajador, finicio, ffinal, faena);
            return Json(new
            {
                registro
            }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult Cabezera(string Trabajador, DateTime finicio, DateTime ffinal)
        {
            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;
            var registro = Asistencia.CabezeraInforme(empresa, Trabajador, finicio, finicio);
            return Json(new
            {
                registro
            }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult SolicitudPermiso(string Trabajador, string fechaInicio, string fechaFin, string hora1, string hora2)
        {
            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;

            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            DateTime fecIni = Convert.ToDateTime(fechaInicio);
            DateTime fecFin = Convert.ToDateTime(fechaFin);
            var permiso = Permiso.SolicitudPermisoInasistencia(Trabajador, fechaInicio, fechaFin, "0","0",empresa);

            return Json(new
            {
                permiso
            }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult ConsultaPermiso(string Trabajador, string fechaInicio, string fechaFin)
        {
            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;

            string token = "gyhhbjkk45kljkjlk4545kkkkk7777hghghghjghjghjghghjgh";
            DateTime fecIni = Convert.ToDateTime(fechaInicio);
            DateTime fecFin = Convert.ToDateTime(fechaFin);
            var consulta = Permiso.ConsultaPermisoInasistencia(Trabajador, fecIni, fecFin, empresa);

            return Json(new
            {
                consulta
            }, JsonRequestBehavior.AllowGet);
        }


    }
}

namespace GYCEmpresa.Models
{

    public class detalle
        {
            public string nombre { get; set; }
            public string apaterno { get; set; }
            public string amaterno { get; set; }
            public string fnacimiento { get; set; }
            public string nacionalidad { get; set; }
            public string telefono1 { get; set; }
            public string telefono2 { get; set; }
            public string correo { get; set; }
            public string direccion { get; set; }
            public string region { get; set; }
            public string ciudad { get; set; }
            public string comuna { get; set; }
            public string sexo { get; set; }
            public string banco { get; set; }
            public string tcuenta { get; set; }
            public string ncuenta { get; set; }
            public string ecivil { get; set; }
            public string nhijos { get; set; }
            public string salud { get; set; }
            public string adicionalsalud { get; set; }
            public string prevision { get; set; }
            public string apv { get; set; }
            public string ahorro { get; set; }
            public string empresaactual { get; set; }
            public string estadocontractual { get; set; }
            public string faena { get; set; }
            public string turno { get; set; }
            public string finicio { get; set; }
            public string ftermi { get; set; }
            public string fferia { get; set; }
            public string dias { get; set; }
            public string fsindi { get; set; }
            public string sindicato { get; set; }
            public string razonsocial { get; set; }
            public string cargo { get; set; }
            public string tipocontrato { get; set; }
            public string sueldo { get; set; }
            public string existe { get; set; }

        }

    }
namespace GYCEmpresa.Models
{

    public class respuesta
    {
        public string mensaje { get; set; }
    }
}
