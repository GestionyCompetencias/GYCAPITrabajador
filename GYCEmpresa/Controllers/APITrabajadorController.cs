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
using System.Net;

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
        public Reply acceso(string trabajador, string clave,string ID_DISPOSITIVO)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "Usuario no registrado";
            DateTime hoy = DateTime.Now.Date;
            DateTime finicio = f.PrimerDia(hoy);
            DateTime ffinal = f.UltimoDia(hoy);
            if (ffinal > hoy) ffinal = hoy;
            int dias_vac=0, dias_per=0, dias_lic=0, dias_ina=0, dias_des=0;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            if(cont != null)
            {
                oR.message = "Clave invalida";

                string empresa = cont.EMPRESA;
                //var lst = (db.USUARIO_ANDROID.Where(x => x.RUT == trabajador && x.CONTRASEÑA == clave && x.HABILITADO == true)).SingleOrDefault();
                var lst = (db.USUARIO_ANDROID.Where(x => x.RUT == trabajador && x.CONTRASEÑA == clave )).SingleOrDefault();
                if(lst != null)
                {
                    InfoRegistraDispositivo Info = RegistraDispositivo(ID_DISPOSITIVO);

                    if (Info.Token != "Error")
                    {
                        //USUARIO_ANDROID usuarioToken = lst.First();
                        USUARIO_ANDROID usuarioToken = lst;
                        DateTime hoydia = DateTime.Now.Date;
                        usuarioToken.TOKEN = Info.Token;
                        usuarioToken.DISPOSITIVO = Info.Id_dispositivo;
                        db.Entry(usuarioToken).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        oR.result = 1;
                        string mensaje = "Token:" + Info.Token + "#Tipo:" + lst.TIPO.ToString();

                        int id_trabajador = db.CONTRATO.Where(x => x.PERSONA == trabajador && x.FIRMAEMPRESA== true && x.FIRMATRABAJADOR== true && x.RECHAZADO== false && x.FTERMNO >=hoydia ).Select(x => x.ID).SingleOrDefault();
                        mensaje = mensaje + "#IdTrabajador:" + id_trabajador + "#Touchless:" + usuarioToken.TOUCHLESS;
                        List<InformeFinal> asistencia = new List<InformeFinal>();
                        asistencia = Registro(trabajador, finicio, ffinal);
                        foreach(var d in asistencia)
                        {
                            if (d.inasistencia == "") dias_ina = dias_ina + 1;
                            if (d.inasistencia == "DESCANSO" || d.inasistencia == "FERIADO") dias_des = dias_des + 1;
                            if (d.inasistencia == "PERMISO") dias_per = dias_per + 1;
                            if (d.inasistencia == "LICENCIA") dias_lic = dias_lic + 1;
                            if (d.inasistencia == "VACACION") dias_vac = dias_vac + 1;

                        }
                        InfoLogin Respuesta = new InfoLogin();
                        Respuesta.token = Info.Token;
                        //Respuesta.modo = lst.FirstOrDefault().TIPO.ToString();
                        Respuesta.modo = lst.TIPO.ToString();
                        Respuesta.idTrabajador = id_trabajador.ToString();
                        Respuesta.touchless = usuarioToken.TOUCHLESS.ToString();
                        Respuesta.urlLogo = "https://logosgycsol.s3-sa-east-1.amazonaws.com/GYCSolLogo.png";
                        Respuesta.diaslicencia = Convert.ToString(dias_lic);
                        Respuesta.diaspermiso = Convert.ToString(dias_per);
                        Respuesta.diasvacacion = Convert.ToString(dias_vac);
                        Respuesta.diasfalla = Convert.ToString(dias_ina);
                        Respuesta.diasdescanso = Convert.ToString(dias_des);
                        oR.data = mensaje;
                        oR.data2 = Respuesta;
                        oR.message = "Acceso Correcto";


                    }
                    else
                    {
                        oR.message = "Credenciales incorrectas";
                    }

                }


            }
            return oR;

        }
        public int ValidaToken(string rut, string token)
        {
            int valido = 0;
            var usua = (db.USUARIO_ANDROID.Where(x => x.RUT == rut && x.TOKEN == token)).SingleOrDefault();
            if (usua != null) valido = 1;
            return valido;
        }
        public Reply ExistePersonaDetalle(string rut, string token)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(rut, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }
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
                oR.result = 1;
                oR.data = token;
                oR.data2 = trabajador;
                oR.message = "Consulta exitosa";
                return oR;
            }
            else
            {
                oR.data2 = null;
                return oR;
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
        public Reply MarcacionesTrabajador(string token,string trabajador, string fechaInicio, string fechaFin)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime fecIni = Convert.ToDateTime(fechaInicio);
            DateTime fecFin = Convert.ToDateTime(fechaFin);
            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            string empresa = cont.EMPRESA;
            if(cont != null)
            {
                oR.data2 = db.JVC_MARCACIONES(fecIni, fecFin, 0, trabajador, empresa);
                oR.result = 1;
                oR.data = token;
                oR.message = "Consulta exitosa";

            }

            return oR;

        }

        public Reply SolicitudVacaciones(string token, string trabajador, string fechaInicio, string fechaFin)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime fecIni = Convert.ToDateTime(fechaInicio);
            DateTime fecFin = Convert.ToDateTime(fechaFin);
            var vaca = rhuecalc.ProcesoVacaciones(trabajador, fecIni, fecFin, 1, "N");
            var vacacion = new List<GYCEmpresa.Models.detvacacion>();
            DateTime hoy = DateTime.Now.Date;
            int idsolicitud,nrosol;
            if (vaca.Count() != 0)
            {
                SOLICITUD_ANDROID newsol = new SOLICITUD_ANDROID();
                SOLICITUD_ANDROID ultsol = new SOLICITUD_ANDROID();
                var todas = db.SOLICITUD_ANDROID.OrderBy(x => x.correl).ToList();
                if (todas.Count == 0)
                {
                    nrosol = 1;
                }
                else
                {
                    ultsol = todas.Last();
                    nrosol = ultsol.correl++;
                }
                foreach (var v in vaca)
                {
                    vacacion.Add(new detvacacion
                    {
                        nrt_ruttr = v.nrt_ruttr,
                        nro_periodo = v.nro_periodo,
                        fec_inivac = v.fec_inivac.ToString("dd'-'MM'-'yyyy"),
                        fec_finvac = v.fec_finvac.ToString("dd'-'MM'-'yyyy"),
                        dias_habil = Convert.ToString(v.dias_habil),
                        dias_corri = Convert.ToString(v.dias_corri),
                        ano_inicio = Convert.ToString(v.año_inicio),
                        ano_termino = Convert.ToString(v.año_termino),
                        tip_uso = Convert.ToString(v.tip_uso),
                        nro_solici = Convert.ToString(v.nro_solici),
                        dias_legal = Convert.ToString(v.dias_legal),
                        dias_progr = Convert.ToString(v.dias_progr),
                        dias_contr = Convert.ToString(v.dias_contr),
                        dias_admin = Convert.ToString(v.dias_admin),
                        dias_faena = Convert.ToString(v.dias_faena),
                        dias_especi = Convert.ToString(v.dias_especi),
                        dias_otros = Convert.ToString(v.dias_otros),
                        idsolicitud = Convert.ToString(nrosol)
                    }); ;
            }
                newsol.correl = nrosol;
                newsol.nrt_ruttr = trabajador;
                newsol.flg_tipo = "V";
                newsol.fec_desde = fecIni.Date;
                newsol.fec_hasta = fecFin.Date;
                newsol.nro_dias = 0;
                newsol.fec_proce = hoy;
                newsol.flg_estado = "S";
                db.SOLICITUD_ANDROID.Add(newsol);
                db.SaveChanges();
                idsolicitud = newsol.correl;
                oR.result = 1;
                oR.data = token;
                oR.data2 =vacacion;
                oR.message = "Solicitud de vacaciones";
            }
            return oR;
        }

        public Reply SolicitudCompensacion(string token,string trabajador, string dias)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }


            int ndias = Convert.ToInt32(dias);
            var comp = rhuecalc.ProcesoCompensacion(trabajador, ndias, 1, "N");
            var compensacion = new List<GYCEmpresa.Models.detvacacion>();
            DateTime hoy = DateTime.Now.Date;
            DateTime feci = hoy;
            DateTime fecf = hoy; ;
            int idsolicitud, nrosol;
            if (comp.Count() != 0)
            {
                SOLICITUD_ANDROID newsol = new SOLICITUD_ANDROID();
                SOLICITUD_ANDROID ultsol = new SOLICITUD_ANDROID();
                var todas = db.SOLICITUD_ANDROID.OrderBy(x => x.correl).ToList();
                if (todas.Count == 0)
                {
                    nrosol = 1;
                }
                else
                {
                    ultsol = todas.Last();
                    nrosol = ultsol.correl;
                    nrosol++;
                }
                foreach (var v in comp)
                {
                compensacion.Add(new detvacacion
                {
                    nrt_ruttr = v.nrt_ruttr,
                    nro_periodo = v.nro_periodo,
                    fec_inivac = v.fec_inivac.ToString("dd'-'MM'-'yyyy"),
                    fec_finvac = v.fec_finvac.ToString("dd'-'MM'-'yyyy"),
                    dias_habil = Convert.ToString(v.dias_habil),
                    dias_corri = Convert.ToString(v.dias_corri),
                    ano_inicio = Convert.ToString(v.año_inicio),
                    ano_termino = Convert.ToString(v.año_termino),
                    tip_uso = Convert.ToString(v.tip_uso),
                    nro_solici = Convert.ToString(v.nro_solici),
                    dias_legal = Convert.ToString(v.dias_legal),
                    dias_progr = Convert.ToString(v.dias_progr),
                    dias_contr = Convert.ToString(v.dias_contr),
                    dias_admin = Convert.ToString(v.dias_admin),
                    dias_faena = Convert.ToString(v.dias_faena),
                    dias_especi = Convert.ToString(v.dias_especi),
                    dias_otros = Convert.ToString(v.dias_otros),
                    idsolicitud = Convert.ToString(nrosol)
                });
                    feci = v.fec_inivac;
                    fecf = v.fec_finvac;
                }
                newsol.correl = nrosol;
                newsol.nrt_ruttr = trabajador;
                newsol.flg_tipo = "C";
                newsol.fec_desde = feci;
                newsol.fec_hasta = fecf;
                newsol.nro_dias = ndias;
                newsol.fec_proce = hoy;
                newsol.flg_estado = "S";
                db.SOLICITUD_ANDROID.Add(newsol);
                db.SaveChanges();
                idsolicitud = newsol.correl;
                oR.result = 1;
                oR.data = token;
                oR.data2 = compensacion;
                oR.message = "Solicitud de vacaciones";

            }

            return oR;

        }
        public Reply ConfirmaSolicitud(string token,string trabajador, string idsol)
        {
            DateTime hoy = DateTime.Now.Date;
            DateTime finicio = f.PrimerDia(hoy);
            DateTime ffinal = f.UltimoDia(hoy);
            var cont = (db.CONTRATO.Where(x => x.PERSONA == trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            Reply oR = new Reply();
            var compensacion = new List<GYCEmpresa.Models.detvacacion>();
            int solic = Convert.ToInt32(idsol);
            rhuesolv infosolv = new rhuesolv();
            rhuesolc infosolc = new rhuesolc();
            var solicitud = db.SOLICITUD_ANDROID.Where(x => x.correl == solic).SingleOrDefault();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "Solicitud rechazada";
            if (cont == null)
            {
                oR.message = "Falta contrato";
                return oR;
            }
            string empresa = cont.EMPRESA;
            if (ValidaToken(trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            if (solicitud != null)
            { 
                if(solicitud.flg_tipo == "C")
                {
                    int nroSolicitud = db.Database.SqlQuery<int>("SELECT nro_solici FROM rhuesolc").FirstOrDefault();
                    if (nroSolicitud == 0)
                    {
                        infosolc.nro_solici = 1;
                    }
                    else
                    {
                        int numsol = db.Database.SqlQuery<int>("SELECT MAX( nro_solici) FROM rhuesolc").First() + 1;
                        infosolc.nro_solici = numsol;
                    }
                    infosolc.nrt_ruttr = trabajador;
                    int ndias = (int)solicitud.nro_dias;
                    DateTime fechaSol = DateTime.Now;
                    infosolc.fec_solici = fechaSol;
                    infosolc.est_solici = "Peticion";
                    infosolc.rut_empr = empresa;
                    infosolc.dias_solici = ndias;
                    var comp = rhuecalc.ProcesoCompensacion(trabajador, ndias, infosolc.nro_solici, "S");
                    if(comp.Count !=0)
                    {
                    foreach(var c in comp)
                        {
                            infosolc.dias_habil = c.dias_habil;
                            infosolc.fec_inicio = c.fec_inivac;
                        }
                    db.rhuesolc.Add(infosolc);
                    db.SaveChanges();
                    oR.message = "Solicitud de compensación ingresada";
                    }
                }
                if (solicitud.flg_tipo == "V")
                {
                    int nroSolicitud = db.Database.SqlQuery<int>("SELECT nro_solici FROM rhuesolv").FirstOrDefault();
                    if (nroSolicitud == 0)
                    {
                        infosolv.nro_solici = 1;
                    }
                    else
                    {
                        int numsol = db.Database.SqlQuery<int>("SELECT MAX( nro_solici) FROM rhuesolv").First() + 1;
                        infosolv.nro_solici = numsol;
                    }
                    DateTime fecIni = (DateTime)solicitud.fec_desde;
                    DateTime fecFin = (DateTime)solicitud.fec_hasta;

                    DateTime fechaSol = DateTime.Today;
                    String fechActual = fechaSol.ToShortDateString();
                    infosolv.nrt_ruttr = trabajador;
                    infosolv.fec_solici = fechaSol;
                    infosolv.est_solici = "Peticion";
                    infosolv.fec_inicio = fecIni;
                    infosolv.fec_termin = fecFin;
                    infosolv.rut_empr = empresa;
                    db.rhuesolv.Add(infosolv);
                    db.SaveChanges();

                    var vaca = rhuecalc.ProcesoVacaciones(trabajador, fecIni, fecFin, infosolv.nro_solici, "S");
                    oR.message = "Solicitud de vacacion ingresada";
                }
                oR.result = 1;
                oR.data = token;
                oR.data2 = null;
            }

            return oR;

        }
        public Reply Periodos(string token,string Trabajador)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(Trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }


            var persona = (db.PERSONA.Where(x => x.RUT == Trabajador)).SingleOrDefault();
            var infoPeri = db.rhueperi.Where(v => v.nrt_ruttr == Trabajador).ToList();
            var periodos = new List<GYCEmpresa.Models.detperiodo>();
            var rhueperi = new List<GYCEmpresa.Models.rhueperi>();
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
                    periodos.Add(new detperiodo
                    {
                        nrt_ruttr = Trabajador,
                        ano_inicio = p.ano_inicio,
                        ano_termino = p.ano_termino,
                        dias_legal = p.dias_legal,
                        dias_progr = 0,
                        dias_contr = p.dias_contr,
                        dias_admin = p.dias_admin,
                        dias_faena = p.dias_faena,
                        dias_especi = p.dias_especi,
                        dias_otros = p.dias_otros,
                        fec_trans = p.fec_trans.ToString("dd'-'MM'-'yyyy"),
                        rut_empr = empresa
                    });

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
                periodos.Add(new detperiodo
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
                    fec_trans = DateTime.Now.Date.ToString("dd'-'MM'-'yyyy"),
                    rut_empr = empresa
                });
                oR.result = 1;
                oR.data = token;
                oR.data2 = periodos;
                oR.message = "Consulta exitosa";
                }
            return oR;
        }
        public Reply CtaCte(string token,string Trabajador)
        {
            List<ListCuentaCorriente> cta = new List<ListCuentaCorriente>();
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(Trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

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
                if(sol!= null)
                {

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
            }
            var infoComp = new List<GYCEmpresa.Models.rhueusos>();
            infoComp = db.rhueusos.Where(i => i.nrt_ruttr == Trabajador && i.tip_uso == "C").ToList();

            foreach (var p in infoComp)
            {
                var solco = (db.rhuesolc.Where(x => x.nrt_ruttr == p.nrt_ruttr && x.nro_solici == p.nro_solici)).SingleOrDefault();
                if(solco != null)
                {

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
            oR.result = 1;
            oR.data = token;
            oR.data2 = cuentacorriente;
            oR.message = "Consulta exitosa";
            return oR;
        }

        public Reply Utilizados(string token,string Trabajador)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(Trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }


            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;
            var persona = (db.PERSONA.Where(x => x.RUT == Trabajador)).SingleOrDefault();
            var infoUsos = new List<GYCEmpresa.Models.rhueusos>();
            infoUsos = db.rhueusos.Where(i => i.nrt_ruttr == Trabajador && i.tip_uso == "V").ToList();
            var usados = new List<detusados>();
            foreach (var p in infoUsos)
            {
                var sol = (db.rhuesolv.Where(x => x.nrt_ruttr == p.nrt_ruttr && x.nro_solici == p.nro_solici)).SingleOrDefault();
                if (sol != null)
                {
                    if (sol.est_solici.Substring(0, 2) == "Ac")
                    {
                        usados.Add(new detusados()
                        {
                            nrt_ruttr = p.nrt_ruttr,
                            fec_inivac = p.fec_inivac.ToString("dd'-'MM'-'yyyy"),
                            fec_tervac = p.fec_tervac.ToString("dd'-'MM'-'yyyy"),
                            fec_transa = p.fec_transa.ToString("dd'-'MM'-'yyyy"),
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
            }
            oR.result = 1;
            oR.data = token;
            oR.data2 = usados;
            oR.message = "Consulta exitosa";


            return oR;
        }
        public Reply LicenciasMedicas(string token,string Trabajador, string fecini, string fecfin)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(Trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime finicio = Convert.ToDateTime(fecini);
            DateTime ftermino = Convert.ToDateTime(fecfin);
            //List<LICENCIAMEDICA> Licencias = new List<LICENCIAMEDICA>();
            db.Configuration.ProxyCreationEnabled = false;
            var licen = db.LICENCIAMEDICA.Where(x => x.TRABAJADOR == Trabajador && x.FINICIO >= finicio && x.FINICIO <= ftermino).ToList();
            var licencias = new List<detlicencia>();

            if (licen.Count != 0)
            {
                foreach (var l in licen)
                {
                    licencias.Add(new detlicencia()
                    {
                        TRABAJADOR = l.TRABAJADOR,
                        CODIGO_LICENCIA = l.CODIGO_LICENCIA,
                        FINICIO = l.FINICIO.ToString("dd'-'MM'-'yyyy"),
                        FTERMINO = l.FTERMINO.ToString("dd'-'MM'-'yyyy"),
                        DIAS = l.DIAS,
                        //PDF = l.PDF,
                        TIPOLICENCIASMEDICAS = l.TIPOLICENCIASMEDICAS,
                        COMENTARIO = l.COMENTARIO
                    });

                }
                oR.result = 1;
                oR.data = token;
                oR.data2 = licencias;
                oR.message = "Consulta exitosa";
            }
            return oR;
        }
        public virtual JsonResult RegistroAsistencia(string token,string trabajador, DateTime finicio, DateTime ffinal)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            //if (ValidaToken(trabajador, token) == 0)
            //{
            //    oR.message = "Problema de acceso";
            //    return oR;
            //}
            detcabezera cabezera = new detcabezera();
            List<InformeFinal> asistencia = new List<InformeFinal>();
            cabezera = Cabezera(trabajador, finicio, ffinal);
            asistencia = Registro(trabajador, finicio, ffinal);
            return Json(new
            {
                cabezera, asistencia
            }, JsonRequestBehavior.AllowGet);
        }
        public List<InformeFinal> Registro(string Trabajador, DateTime finicio, DateTime ffinal)
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
            List<InformeFinal> registro = new List<InformeFinal>();
            registro = Asistencia.InformeIndividual(empresa,Trabajador, finicio, ffinal, faena);
            return registro;
        }
        public detcabezera Cabezera(string Trabajador, DateTime finicio, DateTime ffinal)
        {
            detcabezera cabezera = new detcabezera();
            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;
            cabezera = Asistencia.CabezeraInforme(empresa, Trabajador, finicio, finicio);
            return cabezera;
        }
        public Reply SolicitudPermiso(string token,string Trabajador, string fechaInicio, string fechaFin, string hora1, string hora2)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(Trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;

            DateTime fecIni = Convert.ToDateTime(fechaInicio);
            DateTime fecFin = Convert.ToDateTime(fechaFin);
            var permiso = Permiso.SolicitudPermisoInasistencia(Trabajador, fechaInicio, fechaFin, "0","0",empresa);
            oR.result = 1;
            oR.data = token;
            oR.data2 = permiso;
            oR.message = "Solicitud existosa";

            return oR;
        }
        public Reply ConsultaPermiso(string token,string Trabajador, string fechaInicio, string fechaFin)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(Trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;

            DateTime fecIni = Convert.ToDateTime(fechaInicio);
            DateTime fecFin = Convert.ToDateTime(fechaFin);
            var persona = (db.PERSONA.Where(x => x.RUT == Trabajador)).SingleOrDefault();
            string nom = persona.APATERNO + " " + persona.AMATERNO + " " + persona.NOMBRE;

            var querySolicitudes = (from spi in db.SOLICITUDPERMISOINASISTENCIA
                                    where spi.EMPRESA == empresa && spi.TRABAJADOR==Trabajador && ((spi.FINICIO >= fecIni && spi.FINICIO < fecFin) || (spi.FTERMINO >= fecIni && spi.FTERMINO < fecFin))
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

            List<detconsulta> consulta = new List<detconsulta>();
            foreach (var v in querySolicitudes)
            {
                consulta.Add(new detconsulta()
                {
                    ID = v.ID,
                    FECHA = v.FECHA.ToString("dd'-'MM'-'yyyy"),
                    FINICIO = v.FINICIO.ToString("dd'-'MM'-'yyyy"),
                    FTERMINO = v.FTERMINO.ToString("dd'-'MM'-'yyyy"),
                    AUTORIZAEMPRESA = v.AUTORIZAEMPRESA,
                    AUTORIZATRABAJADOR = v.AUTORIZATRABAJADOR,
                    COMPENSADO = v.COMPENSADO,
                    TRABAJADOR = v.TRABAJADOR,
                    MOTIVO = v.MOTIVO

                });
            }
            oR.result = 1;
            oR.data = token;
            oR.data2 = consulta;
            oR.message = "Consulta exitosa";

            return oR;
        }
        private InfoRegistraDispositivo RegistraDispositivo(string Id_Dispositivo)
        {
            {
                try
                {


                    var infoDisp = db.DISPOSITIVO_ANDROID.Where(x => x.ID_DISPOSITIVO == Id_Dispositivo).ToList();
                    DISPOSITIVO_ANDROID Dispositivo = new DISPOSITIVO_ANDROID();

                    if (infoDisp.Count > 0)
                    {
                        Dispositivo = infoDisp.First();
                        Dispositivo.ID_DISPOSITIVO = Id_Dispositivo;
                    }
                    else
                    {
                        //Dispositivo.DESCRIPCION = "Equipo agregado el " + DateTime.Now.AddHours(AjusteHora());
                        Dispositivo.DESCRIPCION = "Equipo agregado el " + DateTime.Now.AddHours(3);
                        Dispositivo.ID_DISPOSITIVO = Id_Dispositivo;
                        db.DISPOSITIVO_ANDROID.Add(Dispositivo);
                    }

                    db.SaveChanges();

                    InfoRegistraDispositivo info = new InfoRegistraDispositivo();
                    info.Token = Guid.NewGuid().ToString();
                    info.Id_dispositivo = Dispositivo.ID;
                    return info;

                }
                catch (Exception)
                {
                    InfoRegistraDispositivo info = new InfoRegistraDispositivo();
                    info.Token = "Error";
                    info.Id_dispositivo = 0;
                    return info;
                }
            }
        }
        public Reply BuscaDocumentos(string token,string Trabajador)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(Trabajador, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == Trabajador && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;

            var lstdocumento = (db.DOCUMENTOS_CLOUD.Where(x => x.TRABAJADOR == Trabajador && x.EMPRESA == empresa)).ToList();
            var per = (db.PERSONA.Where(x => x.RUT == Trabajador)).SingleOrDefault();
            string nom = per.APATERNO + " " + per.AMATERNO + " " + per.NOMBRE;
            var documentos = new List<detdoc>();
            foreach (var p in lstdocumento)
            {
                DateTime fec1 = (DateTime) p.FINICIO;
                DateTime fec2 = (DateTime) p.FTERMINO;
                documentos.Add(new detdoc()
                {
                    trabajador = p.TRABAJADOR,
                    nombre = nom,
                    tipo = p.S3_DIRECTORIO,
                    descripcion = p.DESCRIPCION,
                    inicio = fec1.ToString("dd'-'MM'-'yyyy"),
                    termino = fec2.ToString("dd'-'MM'-'yyyy"),
                    archivo = p.ARCHIVO
                });
            }
            oR.result = 1;
            oR.data = token;
            oR.data2 = documentos;
            oR.message = "Consulta exitosa";



            return oR;
        }
        public Reply CambioClave(solcambio data)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No se pudo cambiar la clave";
            if (ValidaToken(data.rut, data.token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == data.rut && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;

            var claves = (db.USUARIO_ANDROID.Where(x => x.RUT == data.rut)).ToList();
            if(claves.Count != 0)
            {
            USUARIO_ANDROID ele = new USUARIO_ANDROID();
            ele = claves.First();
            string clave = data.antigua+"                                                                                                        ";
            clave = clave.Substring(0, 100);
            if(clave== ele.CONTRASEÑA)
            {
                ele.CONTRASEÑA = data.nueva;

                string comando = "SET [CONTRASEÑA] = '" + data.nueva + "' WHERE [RUT] = '" + data.rut + "'";
                db.Database.ExecuteSqlCommand("UPDATE USUARIO_ANDROID " + comando);
                    oR.result = 1;
                    oR.data = data.token;
                    oR.data2 = null;
                    oR.message = "Cambio de clave exitoso";
            }
            }
            return oR;
        }
        public Reply MotivoPermiso(string token,string rut)
        {
            Reply oR = new Reply();
            oR.result = 1;
            oR.data = token;
            oR.data2 = null;
            oR.message = "Tabla se cargo";
            if (ValidaToken(rut, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == rut && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;
            var Tipper = db.remepage.Where(x => x.nom_tabla == "PERMISO" && x.rut_empr == empresa).Select(x => new { Id = x.cod_param, Descripcion = x.gls_param }).ToList().OrderBy(x => x.Descripcion);
            oR.data2 = Tipper;

            return oR;
        }
        public Reply Notificaciones(string token,string rut)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe información";
            if (ValidaToken(rut, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime hoy = DateTime.Now.Date;
            var cont = (db.CONTRATO.Where(x => x.PERSONA == rut && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;
            var noti = new List<GYCEmpresa.Models.NOTIFICACION>();
            db.Configuration.ProxyCreationEnabled = false;
            noti = db.NOTIFICACION.Where(item => item.EMPRESA == empresa && item.VISTO == false && item.NOTIFTRABAJADOR == true && item.TRABAJADOR == rut).ToList();
            var notifica = new List<detnotifica>();
            foreach (var n in noti)
            {

                    notifica.Add(new detnotifica()
                    {
                    ID=n.ID,
                    TIPO =n.TIPO,
                    FECHA=n.FECHA.ToString("dd'-'MM'-'yyyy"),
                    OBSERVACION=n.OBSERVACION,
                    GYC = n.GYC,
                    NOTIFTRABAJADOR = n.NOTIFTRABAJADOR,
                    NOTIFEMPRESA = n.NOTIFEMPRESA,
                    TRABAJADOR =n.TRABAJADOR,
                    EMPRESA = n.EMPRESA,
                    USUARIO =n.USUARIO,
                    TABLA = n.TABLA,
                    IDTABLA = n.IDTABLA,
                    ESTADO = n.ESTADO,
                    VISTO = n.VISTO,
                    USUARIO1 = n.USUARIO1
                    });
            }
            oR.result = 1;
            oR.data = token;
            oR.data2 = notifica;
            oR.message = "Consulta exitosa";

            return oR;
        }
        public Reply TipoDocumento(string token,string rut)
        {
            Reply oR = new Reply();
            oR.result = 1;
            oR.data = token;
            oR.data2 = null;
            oR.message = "Tabla se cargo";
            if (ValidaToken(rut, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }

            DateTime hoy = DateTime.Now.Date;
            int[] tipo = new int[1000];
            string[] desc = new string[1000];
            int ind;
            for (ind = 0; ind < 1000; ind++)
            { tipo[ind] = 0;
                desc[ind] = null;
            }

            var cont = (db.CONTRATO.Where(x => x.PERSONA == rut && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            int faena = cont.FAENA;
            string empresa = cont.EMPRESA;

            var lstdocumento = (db.DOCUMENTOS_CLOUD.Where(x => x.TRABAJADOR == rut && x.EMPRESA == empresa)).ToList();
            var per = (db.PERSONA.Where(x => x.RUT == rut)).SingleOrDefault();
            string nom = per.APATERNO + " " + per.AMATERNO + " " + per.NOMBRE;
            var documentos = new List<detdoc>();
            var TipoBucket = db.TIPO_BUCKET.Select(x => new { x.ID, x.DESCRIPCION }).ToList();
            foreach(var t in TipoBucket)
            {
                ind = t.ID;
                if (ind < 1000)
                {
                    desc[ind] = t.DESCRIPCION;
                }
            }
            int indb;
            foreach (var p in lstdocumento)
            {
                indb = 0;
                for (ind = 0; ind < 1000; ind++)
                {
                    if(p.S3_DIRECTORIO== desc[ind])
                    {
                        indb = ind;
                        ind = 1000;
                    }
                }
                tipo[indb] = 1;
            }
            var tipodocum = new List<tipodoc>();
            for (ind = 0; ind < 999; ind++)
            {
              if(tipo[ind]!= 0)
                {

                tipodocum.Add(new tipodoc()
                {
                    id= Convert.ToString(ind),
                    descripcion= desc[ind]
                });
                }
            }
            oR.data2 = tipodocum;
            return oR;
        }
        public ActionResult RecuperaContraseña(string Usuario, string Empresa)
        {
            try
            {
                var UltimaSolicitud = db2.LOG_MAIL.Where(x => x.USUARIO == Usuario).OrderByDescending(x => x.FECHA_ENVIO).Take(1).SingleOrDefault();
                if (UltimaSolicitud != null)
                {
                    if (UltimaSolicitud.FECHA_ENVIO > DateTime.Now.AddMinutes(-5))
                    {
                        TimeSpan minutos = TimeSpan.Parse((UltimaSolicitud.FECHA_ENVIO - DateTime.Now.AddMinutes(-5)).ToString());
                        ViewBag.Redireccionar = false;
                        ViewBag.ErrorInfo = "La recuperacion ya fue solicitada. Espere " + minutos.Minutes + " minutos para intentar nuevamente";
                        return View();
                    }
                }

                var InfoUsuario = (from usuario in db.USUARIO
                                   from cargo in db.CARGOEMPRESA
                                   where usuario.RUT == cargo.RUT
                                   && usuario.RUT == Usuario
                                   && cargo.EMPRESA == Empresa
                                   select new
                                   {
                                       cargo.CORREO,
                                       cargo.NOMBRE
                                   }).ToList();

                if (InfoUsuario.Count > 0)
                {
                    Random rnd = new Random();
                    string Pass = rnd.Next(1000, 9999).ToString();

                    var ModPass = db.USUARIO.Where(x => x.RUT == Usuario).SingleOrDefault();
                    ModPass.CONTRASENA = Pass;

                    db.SaveChanges();


                    String Firma = "<br /> Atte., <br /> Gestión y Competencias SPA.  <br/> <a href='https://www.gestionycompetencias.cl'>www.gestionycompetencias.cl</a> <br/> <img src='https://www.gycsol.cl/Content/img/logo-mail-gyc.jpg' alt='Gestion y Competencias'>";

                    foreach (var usuario in InfoUsuario)
                    {

                        if (ModelState.IsValid)
                        {
                            var senderEmail = new MailAddress("plataforma@gycsol.cl", "Plataforma GYCSol");
                            var receiverEmail = new MailAddress(usuario.CORREO, usuario.NOMBRE);
                            var password = "Plataf0rma!!";
                            var sub = "Recuperación de Contraseña";
                            var body = "Estimados sr(a). " + usuario.NOMBRE + ": <br />Su nueva contraseña es: " + Pass + Firma;
                            var smtp = new SmtpClient
                            {
                                Host = "gycsol.cl",
                                Port = 25,
                                EnableSsl = false,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                 Credentials = new NetworkCredential(senderEmail.Address, password)
                            };
                            using (var mess = new MailMessage(senderEmail, receiverEmail)
                            {
                                Subject = sub,
                                Body = body,
                                SubjectEncoding = System.Text.Encoding.UTF8,
                                BodyEncoding = System.Text.Encoding.UTF8,
                                IsBodyHtml = true
                            })
                            {
                                smtp.Send(mess);
                            }
                            //REGISTRO RECUPERACION Y ENVIO MAIL
                            var IP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                            if (string.IsNullOrEmpty(IP))
                            {
                                IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            }

                            using (dbgyc2DataContext db = new dbgyc2DataContext())
                            {
                                db2.LOGMAIL(DateTime.Now, "NO MAC", IP, Usuario, usuario.CORREO, "RECUPERACION DE CONTRASEÑA USUARIO EMPRESA");
                            }
                        }

                        ViewBag.Redireccionar = true;
                        ViewBag.ErrorInfo = false;
                        return View();
                    }

                }
                else
                {
                    ViewBag.ErrorInfo = true;
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }



        private class InfoRegistraDispositivo
{
    public int Id_dispositivo { get; set; }
    public string Token { get; set; }
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
namespace GYCEmpresa.Models
{
    internal class InfoLogin
    {
        public string token { get; set; }
        public string modo { get; set; }
        public string idTrabajador { get; set; }
        public string touchless { get; set; }
        public string urlLogo { get; set; }
        public string diaslicencia { get; set; }
        public string diaspermiso { get; set; }
        public string diasvacacion { get; set; }
        public string diasfalla { get; set; }
        public string diasdescanso { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class detdoc
    {
        public string trabajador { get; set; }
        public string nombre { get; set; }
        public string tipo { get; set; }
        public string descripcion { get; set; }
        public object inicio { get; set; }
        public string termino { get; set; }
        public object archivo { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class Reply
    {
        public int result { get; set; }
        public object data { get; set; }
        public string message { get; set; }
        public object data2 { get; set; }
    }
}
namespace GYCEmpresa.Models
{

    public class tipodoc
    {
        public string id { get; set; }
        public string descripcion { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class detvacacion
    {
        public string nrt_ruttr { get; set; }
        public string nro_periodo { get; set; }
        public string fec_inivac { get; set; }
        public string fec_finvac { get; set; }
        public string dias_habil { get; set; }
        public string dias_corri { get; set; }
        public string ano_inicio { get; set; }
        public string ano_termino { get; set; }
        public string tip_uso { get; set; }
        public string nro_solici { get; set; }
        public string dias_legal { get; set; }
        public string dias_progr { get; set; }
        public string dias_contr { get; set; }
        public string dias_admin { get; set; }
        public string dias_faena { get; set; }
        public string dias_especi { get; set; }
        public string dias_otros { get; set; }
        public string idsolicitud { get; set; }
    }
}

namespace GYCEmpresa.Models
{
    public class detperiodo
    {
        public int correl { get; set; }
        public string nrt_ruttr { get; set; }
        public int ano_inicio { get; set; }
        public int ano_termino { get; set; }
        public int dias_legal { get; set; }
        public Nullable<int> dias_progr { get; set; }
        public Nullable<int> dias_contr { get; set; }
        public Nullable<int> dias_admin { get; set; }
        public Nullable<int> dias_faena { get; set; }
        public Nullable<int> dias_especi { get; set; }
        public Nullable<int> dias_otros { get; set; }
        public string fec_trans { get; set; }
        public string rut_empr { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class detusados
    {
        public int correl { get; set; }
        public string nrt_ruttr { get; set; }
        public int ano_inicio { get; set; }
        public int ano_termino { get; set; }
        public string tip_uso { get; set; }
        public string fec_inivac { get; set; }
        public string fec_tervac { get; set; }
        public int nro_solici { get; set; }
        public int dias_corri { get; set; }
        public Nullable<int> dias_legal { get; set; }
        public Nullable<int> dias_progr { get; set; }
        public Nullable<int> dias_contr { get; set; }
        public Nullable<int> dias_admin { get; set; }
        public Nullable<int> dias_faena { get; set; }
        public Nullable<int> dias_especi { get; set; }
        public Nullable<int> dias_otros { get; set; }
        public string fec_transa { get; set; }
        public string rut_empr { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class detconsulta
    {
        public int ID { get; set; }
        public string FECHA { get; set; }
        public string FINICIO { get; set; }
        public string FTERMINO { get; set; }
        public bool? AUTORIZAEMPRESA { get; set; }
        public bool? AUTORIZATRABAJADOR { get; set; }
        public int? COMPENSADO { get; set; }
        public string TRABAJADOR { get; set; }
        public string MOTIVO { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class detlicencia
    {
        public int ID { get; set; }
        public string CODIGO_LICENCIA { get; set; }
        public string TRABAJADOR { get; set; }
        public string FINICIO { get; set; }
        public string FTERMINO { get; set; }
        public Nullable<int> DIAS { get; set; }
        public int TIPO_LICENCIA { get; set; }
        public string COMENTARIO { get; set; }
        public Nullable<int> TIPO_MEDICO { get; set; }
        public byte[] PDF { get; set; }

        public virtual TIPOLICENCIASMEDICAS TIPOLICENCIASMEDICAS { get; set; }
        public virtual TIPOMEDICO TIPOMEDICO { get; set; }
        public virtual PERSONA PERSONA { get; set; }
    }
}
namespace GYCEmpresa.Models
{
    public class detnotifica
    {
        public int ID { get; set; }
        public string TIPO { get; set; }
        public string FECHA { get; set; }
        public string OBSERVACION { get; set; }
        public Nullable<bool> GYC { get; set; }
        public Nullable<bool> NOTIFTRABAJADOR { get; set; }
        public Nullable<bool> NOTIFEMPRESA { get; set; }
        public string TRABAJADOR { get; set; }
        public string EMPRESA { get; set; }
        public string USUARIO { get; set; }
        public string TABLA { get; set; }
        public int IDTABLA { get; set; }
        public Nullable<int> ESTADO { get; set; }
        public Nullable<bool> VISTO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}
