using GYCEmpresa.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GYCEmpresa.Controllers
{
    public class rhuecalc
    {
        private dbgycEntities3 db = new dbgycEntities3();
        public struct diash
        {
            public string horar;
            public int leg;
            public int con;
            public int adm;
            public int fae;
            public int esp;
            public int otr;
        }

        diash[] dias_hor = new diash[100];

        public struct calcc
        {
            public int dLegU;
            public int dContU;
            public int dAdminU;
            public int dFaenaU;
            public int dEspeciU;
            public int dOtrosU;
            public int dProgrU;
            public int dLegP;
            public int dContP;
            public int dAdminP;
            public int dFaenaP;
            public int dEspeciP;
            public int dOtrosP;
            public int dProgrP;
            public int sumaDiasP;
            public int acumulados;
            public int diasPendientes;
            public int diasOcupados;
            public DateTime fechaInivac;
            public DateTime fechaTervac;
            public string tipoUso;
            public int anoInicio;
        }
        calcc[] dias_total = new calcc[100];

        public struct periodos
        {
            public int ini;
            public int fin;
            public int dleg;
            public int dpro;
            public int dcon;
            public int dadm;
            public int dfae;
            public int desp;
            public int dotr;
            public int uleg;
            public int upro;
            public int ucon;
            public int uadm;
            public int ufae;
            public int uesp;
            public int uotr;
            public int sleg;
            public int spro;
            public int scon;
            public int sadm;
            public int sfae;
            public int sesp;
            public int sotr;
            public int sald;
            public int nuev;
        }
        int nperi, bus, ndias;
        DateTime fechoy = DateTime.Now.Date;
        string hoy;
        string salida;
        int corr, habil, corrp;
        periodos[] peri_tra = new periodos[100];


        //Calcula vacaciones
        public List<periodoscalc> ProcesoVacaciones(string ruttr, DateTime fecini, DateTime fecfin, int solici, string graba)
        {
            var perical1 = new List<periodoscalc>();
            fecini = fecini.Date;
            fecfin = fecfin.Date;
            var infoLaboral = new List<GYCEmpresa.Models.CONTRATO>();
            infoLaboral = db.CONTRATO.Where(i => i.PERSONA == ruttr).ToList();
            hoy = Convert.ToString(fechoy);
            int sali = Carga_Dias();
            //FOREACH DE INFORMACION LABORAL DEL TRABAJADOR
            String fechaR = "01-02-1950";
            DateTime fecIngreso = Convert.ToDateTime(fechaR);
            String codHorar = null;
            foreach (var i in infoLaboral)
            {
                fecIngreso = i.FINICIO;
                codHorar = i.AREA;
            }
            if (fecIngreso == Convert.ToDateTime(fechaR))
            {
                return perical1;
            }
            sali = Actualiza_Periodo(ruttr, fecIngreso, codHorar, fecini);
            sali = Calcular_Dias(fecini, fecfin);
            sali = Determinar_Periodos();
            perical1 = Grabar_Usos(ruttr, fecini, fecfin, "V", solici, graba);
            return perical1;
        }


        //Calcula compensacion
        public List<periodoscalc> ProcesoCompensacion(string ruttr, int dias, int solici, string graba)
        {
            DateTime fecini, fecfin;
            int ind;
            String fechaR = "01-02-1950";
            DateTime fecIngreso = Convert.ToDateTime(fechaR);
            String codHorar = null;
            var perical1 = new List<periodoscalc>();

            var infoLaboral = new List<GYCEmpresa.Models.CONTRATO>();
            infoLaboral = db.CONTRATO.Where(i => i.PERSONA == ruttr).ToList();
            hoy = Convert.ToString(fechoy);
            fecini = fechoy;
            for (ind = 1; ind < 8; ind++)
            {
                fecini = fecini.AddDays(1);
                byte diasem = (byte)fecini.DayOfWeek;
                if (diasem == 1) ind = 10;
            }
            fecfin = Calcular_Habiles(fecini, dias);
            int sali = Carga_Dias();
            //FOREACH DE INFORMACION LABORAL DEL TRABAJADOR
            foreach (var i in infoLaboral)
            {
                fecIngreso = i.FINICIO;
                codHorar = i.AREA;
            }
            sali = Actualiza_Periodo(ruttr, fecIngreso, codHorar, fecini);
            sali = Calcular_Dias(fecini, fecfin);
            sali = Determinar_Periodos();
            perical1 = Grabar_Usos(ruttr, fecini, fecfin, "C", solici, graba);

            return perical1;
        }
        public int Actualiza_Periodo(string rut, DateTime ingreso, string horario, DateTime fechai)
        {
            int ind, aini, cumple = 0;
            int ahoy = fechoy.Year;
            int mesdiai = fechai.Month * 100 + fechai.Day;
            int mesdiac = ingreso.Month * 100 + ingreso.Day;
            String hor;
            bus = 0;
            if (mesdiai >= mesdiac) cumple = 1;
            for (ind = 1; ind < 10; ind++)
            {
                hor =  dias_hor[ind].horar;
                if (hor != null)
                {
                 hor = hor.Replace(" ", "");
                 if (String.CompareOrdinal(hor, horario)==0)
                   { bus = ind;
                      ind = 100;
                   }

                }
            }
            if (bus == 0) bus=1;
            aini = ingreso.Year;
            var periInfo = new List<GYCEmpresa.Models.rhueperi>();
            string ruts = Convert.ToString(rut);
            periInfo = db.rhueperi.Where(m => m.nrt_ruttr == ruts).ToList();

            var usosInfo = new List<GYCEmpresa.Models.rhueusos>();
            usosInfo = db.rhueusos.Where(m => m.nrt_ruttr == rut).ToList();

            for (ind = 0; ind < 100; ind++)
            {
                peri_tra[nperi].ini = 0;
                peri_tra[nperi].fin = 0;
                peri_tra[nperi].dleg = 0;
                peri_tra[nperi].dpro = 0;
                peri_tra[nperi].dcon = 0;
                peri_tra[nperi].dadm = 0;
                peri_tra[nperi].dfae = 0;
                peri_tra[nperi].desp = 0;
                peri_tra[nperi].dotr = 0;
                peri_tra[nperi].uleg = 0;
                peri_tra[nperi].upro = 0;
                peri_tra[nperi].ucon = 0;
                peri_tra[nperi].uadm = 0;
                peri_tra[nperi].ufae = 0;
                peri_tra[nperi].uesp = 0;
                peri_tra[nperi].uotr = 0;
                peri_tra[nperi].sleg = 0;
                peri_tra[nperi].spro = 0;
                peri_tra[nperi].scon = 0;
                peri_tra[nperi].sadm = 0;
                peri_tra[nperi].sfae = 0;
                peri_tra[nperi].sesp = 0;
                peri_tra[nperi].sotr = 0;
                peri_tra[nperi].sald = 0;
            }


            nperi = 0;
            foreach (var m in periInfo)
            {
                peri_tra[nperi].ini = m.ano_inicio;
                peri_tra[nperi].fin = m.ano_termino;
                peri_tra[nperi].dleg = (int)m.dias_legal;
                peri_tra[nperi].dpro = (int)m.dias_progr;
                peri_tra[nperi].dcon = (int)m.dias_contr;
                peri_tra[nperi].dadm = (int)m.dias_admin;
                peri_tra[nperi].dfae = (int)m.dias_faena;
                peri_tra[nperi].desp = (int)m.dias_especi;
                peri_tra[nperi].dotr = (int)m.dias_otros;
                peri_tra[nperi].uleg = 0;
                peri_tra[nperi].upro = 0;
                peri_tra[nperi].ucon = 0;
                peri_tra[nperi].uadm = 0;
                peri_tra[nperi].ufae = 0;
                peri_tra[nperi].uesp = 0;
                peri_tra[nperi].uotr = 0;
                peri_tra[nperi].sleg = peri_tra[nperi].dleg - peri_tra[nperi].uleg;
                peri_tra[nperi].spro = peri_tra[nperi].dpro - peri_tra[nperi].upro;
                peri_tra[nperi].scon = peri_tra[nperi].dcon - peri_tra[nperi].ucon;
                peri_tra[nperi].sadm = peri_tra[nperi].dadm - peri_tra[nperi].uadm;
                peri_tra[nperi].sfae = peri_tra[nperi].dfae - peri_tra[nperi].ufae;
                peri_tra[nperi].sesp = peri_tra[nperi].desp - peri_tra[nperi].uesp;
                peri_tra[nperi].sotr = peri_tra[nperi].dotr - peri_tra[nperi].uotr;
                peri_tra[nperi].sald = peri_tra[nperi].sleg + peri_tra[nperi].spro + peri_tra[nperi].scon + peri_tra[nperi].sadm
                    + peri_tra[nperi].sfae + peri_tra[nperi].sesp + peri_tra[nperi].sotr;

            }
            if (nperi == 0)
            {
                int ind1;
                nperi = 1;
                for (ind = aini; ind < ahoy + cumple - 1; ind++)
                {
                    ind1 = ind - aini + 1;
                    peri_tra[nperi].ini = aini + ind1 - 1;
                    peri_tra[nperi].fin = peri_tra[nperi].ini + 1;
                    peri_tra[nperi].dleg = dias_hor[bus].leg;
                    peri_tra[nperi].dpro = Progresivos(ingreso, peri_tra[nperi].ini);
                    peri_tra[nperi].dcon = dias_hor[bus].con;
                    peri_tra[nperi].dadm = dias_hor[bus].adm;
                    peri_tra[nperi].dfae = dias_hor[bus].fae;
                    peri_tra[nperi].desp = dias_hor[bus].esp;
                    peri_tra[nperi].dotr = dias_hor[bus].otr;
                    peri_tra[nperi].uleg = 0;
                    peri_tra[nperi].upro = 0;
                    peri_tra[nperi].ucon = 0;
                    peri_tra[nperi].uadm = 0;
                    peri_tra[nperi].ufae = 0;
                    peri_tra[nperi].uesp = 0;
                    peri_tra[nperi].uotr = 0;
                    peri_tra[nperi].sleg = peri_tra[nperi].dleg - peri_tra[nperi].uleg;
                    peri_tra[nperi].spro = peri_tra[nperi].dpro - peri_tra[nperi].upro;
                    peri_tra[nperi].scon = peri_tra[nperi].dcon - peri_tra[nperi].ucon;
                    peri_tra[nperi].sadm = peri_tra[nperi].dadm - peri_tra[nperi].uadm;
                    peri_tra[nperi].sfae = peri_tra[nperi].dfae - peri_tra[nperi].ufae;
                    peri_tra[nperi].sesp = peri_tra[nperi].desp - peri_tra[nperi].uesp;
                    peri_tra[nperi].sotr = peri_tra[nperi].dotr - peri_tra[nperi].uotr;
                    peri_tra[nperi].sald = peri_tra[nperi].sleg + peri_tra[nperi].spro + peri_tra[nperi].scon + peri_tra[nperi].sadm
                        + peri_tra[nperi].sfae + peri_tra[nperi].sesp + peri_tra[nperi].sotr;
                    peri_tra[nperi].nuev = peri_tra[nperi].sald;
                    nperi++;

                }
            }
            foreach (var u in usosInfo)
            {
                for (ind = 1; ind < nperi; ind++)
                {

                    if (peri_tra[ind].ini == u.ano_inicio)
                    {
                        peri_tra[ind].uleg = peri_tra[ind].uleg + (int)u.dias_legal;
                        peri_tra[ind].upro = peri_tra[ind].upro + (int)u.dias_progr;
                        peri_tra[ind].ucon = peri_tra[ind].ucon + (int)u.dias_contr;
                        peri_tra[ind].uadm = peri_tra[ind].uadm + (int)u.dias_admin;
                        peri_tra[ind].ufae = peri_tra[ind].ufae + (int)u.dias_faena;
                        peri_tra[ind].uesp = peri_tra[ind].uesp + (int)u.dias_especi;
                        peri_tra[ind].uotr = peri_tra[ind].uotr + (int)u.dias_otros;
                        peri_tra[ind].sleg = peri_tra[ind].dleg - peri_tra[ind].uleg;
                        peri_tra[ind].spro = peri_tra[ind].dpro - peri_tra[ind].upro;
                        peri_tra[ind].scon = peri_tra[ind].dcon - peri_tra[ind].ucon;
                        peri_tra[ind].sadm = peri_tra[ind].dadm - peri_tra[ind].uadm;
                        peri_tra[ind].sfae = peri_tra[ind].dfae - peri_tra[ind].ufae;
                        peri_tra[ind].sesp = peri_tra[ind].desp - peri_tra[ind].uesp;
                        peri_tra[ind].sotr = peri_tra[ind].dotr - peri_tra[ind].uotr;

                        peri_tra[ind].sald = peri_tra[ind].sleg + peri_tra[ind].spro + peri_tra[ind].scon + peri_tra[ind].sadm
                            + peri_tra[ind].sfae + peri_tra[ind].sesp + peri_tra[ind].sotr;
                        peri_tra[ind].nuev = peri_tra[ind].sald;
                    }
                }

            }
            return 1;
        }
        int Progresivos(DateTime fecing, int anopro)
        {
            int prog = 0;
            int anoing = fecing.Year;

            int difer = anopro - anoing;
            if (difer >= 13)
            {
                if (difer >= 16)
                {
                    if (difer >= 19)
                    {
                        if (difer >= 21)
                        {
                            if (difer >= 24)
                            {
                                if (difer >= 27)
                                {
                                    if (difer >= 30)
                                    { prog = 7; }
                                    else { prog = 6; }
                                }
                                else { prog = 5; }
                            }
                            else { prog = 4; }
                        }
                        else { prog = 3; }
                    }
                    else { prog = 2; }
                }
                else { prog = 1; }
            }
            return prog;
        }
        int Calcular_Dias(DateTime fechai, DateTime fechaf)
        {
            int ind,ind1,nferi,dferi;
            DateTime fec1;
            fec1 = fechai.Date;
            DateTime[] feri = new DateTime[100];
            corr = 0;
            habil = 0;
            nferi = 0;
            var feriInfo = new List<GYCEmpresa.Models.FERIADO>();
            feriInfo = db.FERIADO.ToList();
            foreach (var w in feriInfo)
            {
                if(w.FECHA>=fechai && w.FECHA<= fechaf)
                {
                    nferi++;
                    feri[nferi] = w.FECHA.Date;
                }
            }


            for (ind = 1; ind < 100; ind++)
            {
                byte diasem = (byte)fec1.DayOfWeek;
                corr++;
                dferi = 0;
                for (ind1 = 1; ind1 <= nferi; ind1++)
                {
                    if (feri[ind1] == fec1) dferi = 1;
                }
                if(dferi==0)
                if (diasem == 1 || diasem == 2 || diasem == 3 || diasem == 4 || diasem == 5) habil++;
                fec1 = fec1.AddDays(1);
                if (fec1 >= fechaf) ind = 100;
            }
            if(fechai != fechaf)
            {
                if (corr > 0)
                {
                    corr++;
                    habil++;
                }
            }
            return 1;
        }
        int Determinar_Periodos()
        {
            int ind;
            for (ind = 1; ind <= nperi; ind++)
            {
                if (habil > 0)
                {
                    if (peri_tra[ind].sald >= habil)
                    {
                        if (peri_tra[ind].sleg >= habil)
                        {
                            peri_tra[ind].uleg = habil;
                            peri_tra[ind].sleg = peri_tra[ind].sleg - habil;
                            peri_tra[ind].sald = peri_tra[ind].sald - habil;
                            habil = 0;
                        }
                        else
                        {
                            peri_tra[ind].uleg = peri_tra[ind].sleg;
                            peri_tra[ind].sald = peri_tra[ind].sald - peri_tra[ind].sleg;
                            habil = habil - peri_tra[ind].sleg;
                            peri_tra[ind].sleg = 0;
                        }
                        if (peri_tra[ind].spro >= habil)
                        {
                            peri_tra[ind].upro = habil;
                            peri_tra[ind].spro = peri_tra[ind].spro - habil;
                            peri_tra[ind].sald = peri_tra[ind].sald - habil;
                            habil = 0;
                        }
                        else
                        {
                            peri_tra[ind].upro = peri_tra[ind].spro;
                            peri_tra[ind].sald = peri_tra[ind].sald - peri_tra[ind].spro;
                            habil = habil - peri_tra[ind].spro;
                            peri_tra[ind].spro = 0;
                        }
                        if (peri_tra[ind].scon >= habil)
                        {
                            peri_tra[ind].ucon = habil;
                            peri_tra[ind].scon = peri_tra[ind].scon - habil;
                            peri_tra[ind].sald = peri_tra[ind].sald - habil;
                            habil = 0;
                        }
                        else
                        {
                            peri_tra[ind].ucon = peri_tra[ind].scon;
                            peri_tra[ind].sald = peri_tra[ind].sald - peri_tra[ind].scon;
                            habil = habil - peri_tra[ind].scon;
                            peri_tra[ind].scon = 0;
                        }
                        if (peri_tra[ind].sadm >= habil)
                        {
                            peri_tra[ind].uadm = habil;
                            peri_tra[ind].sadm = peri_tra[ind].sadm - habil;
                            peri_tra[ind].sald = peri_tra[ind].sald - habil;
                            habil = 0;
                        }
                        else
                        {
                            peri_tra[ind].uadm = peri_tra[ind].sadm;
                            peri_tra[ind].sald = peri_tra[ind].sald - peri_tra[ind].sadm;
                            habil = habil - peri_tra[ind].sadm;
                            peri_tra[ind].sadm = 0;
                        }
                        if (peri_tra[ind].sfae >= habil)
                        {
                            peri_tra[ind].ufae = habil;
                            peri_tra[ind].sfae = peri_tra[ind].sfae - habil;
                            peri_tra[ind].sald = peri_tra[ind].sald - habil;
                            habil = 0;
                        }
                        else
                        {
                            peri_tra[ind].ufae = peri_tra[ind].sfae;
                            peri_tra[ind].sald = peri_tra[ind].sald - peri_tra[ind].sfae;
                            habil = habil - peri_tra[ind].sfae;
                            peri_tra[ind].sfae = 0;
                        }
                        if (peri_tra[ind].sesp >= habil)
                        {
                            peri_tra[ind].uesp = habil;
                            peri_tra[ind].sesp = peri_tra[ind].sesp - habil;
                            peri_tra[ind].sald = peri_tra[ind].sald - habil;
                            habil = 0;
                        }
                        else
                        {
                            peri_tra[ind].uesp = peri_tra[ind].sesp;
                            peri_tra[ind].sald = peri_tra[ind].sald - peri_tra[ind].sesp;
                            habil = habil - peri_tra[ind].sesp;
                            peri_tra[ind].sesp = 0;
                        }
                        if (peri_tra[ind].sotr >= habil)
                        {
                            peri_tra[ind].uotr = habil;
                            peri_tra[ind].sotr = peri_tra[ind].sotr - habil;
                            peri_tra[ind].sald = peri_tra[ind].sald - habil;
                            habil = 0;
                        }
                        else
                        {
                            peri_tra[ind].uotr = peri_tra[ind].sotr;
                            peri_tra[ind].sald = peri_tra[ind].sald - peri_tra[ind].sotr;
                            habil = habil - peri_tra[ind].sotr;
                            peri_tra[ind].sotr = 0;
                        }


                    }
                    else
                    {
                        habil = habil - peri_tra[ind].sald;
                        peri_tra[ind].sald = 0;
                        peri_tra[ind].uleg = peri_tra[ind].sleg;
                        peri_tra[ind].upro = peri_tra[ind].spro;
                        peri_tra[ind].ucon = peri_tra[ind].scon;
                        peri_tra[ind].uadm = peri_tra[ind].sadm;
                        peri_tra[ind].ufae = peri_tra[ind].sfae;
                        peri_tra[ind].uesp = peri_tra[ind].sesp;
                        peri_tra[ind].uotr = peri_tra[ind].sotr;
                        peri_tra[ind].sleg = 0;
                        peri_tra[ind].spro = 0;
                        peri_tra[ind].scon = 0;
                        peri_tra[ind].sadm = 0;
                        peri_tra[ind].sfae = 0;
                        peri_tra[ind].sesp = 0;
                        peri_tra[ind].sotr = 0;

                    }
                }
            }
            return 1;
        }

        public List<periodoscalc> Grabar_Usos(string rut, DateTime fechai, DateTime fechaf, string tipo, int soli, string graba)
        {
            int ind, dif, habil;
            DateTime feccal;
            string feci, fecf, sep;
            DateTime hoy = DateTime.Now.Date;
            var con = (db.CONTRATO.Where(x => x.PERSONA == rut && x.FTERMNO >= hoy && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false)).SingleOrDefault();
            string empresa = con.EMPRESA;
            var perical = new List<periodoscalc>();



            string sal;
            for (ind = 1; ind < nperi; ind++)
            {
                if (peri_tra[ind].sald != peri_tra[ind].nuev)
                {
                    dif = peri_tra[ind].nuev - peri_tra[ind].sald;
                    feccal = Calcular_Habiles(fechai, dif);
                    rhueusos usos = new rhueusos();
                    usos.nrt_ruttr = rut;
                    usos.ano_inicio = peri_tra[ind].ini;
                    usos.ano_termino = peri_tra[ind].fin;
                    usos.tip_uso = tipo;
                    usos.fec_inivac = fechai.Date;
                    usos.fec_tervac = feccal.Date;
                    usos.nro_solici = soli;
                    usos.dias_corri = corrp;
                    usos.dias_legal = peri_tra[ind].uleg;
                    usos.dias_progr = peri_tra[ind].upro;
                    usos.dias_contr = peri_tra[ind].ucon;
                    usos.dias_admin = peri_tra[ind].uadm;
                    usos.dias_faena = peri_tra[ind].ufae;
                    usos.dias_especi = peri_tra[ind].uesp;
                    usos.dias_otros = peri_tra[ind].uotr;
                    habil = peri_tra[ind].uleg + peri_tra[ind].upro + peri_tra[ind].ucon + peri_tra[ind].uadm + peri_tra[ind].ufae + peri_tra[ind].uesp + peri_tra[ind].uotr;
                    usos.fec_transa = fechoy;
                    usos.rut_empr = empresa;
                    feci = Convert.ToString(fechai);
                    fecf = Convert.ToString(feccal);
                    sep = "    ";
                    if (graba == "S")
                    {
                        db.rhueusos.Add(usos);
                        db.SaveChanges();
                    }


                periodoscalc per = new periodoscalc();
                    per.nro_periodo = peri_tra[ind].ini + " " + peri_tra[ind].fin;
                    per.fec_inivac = fechai;
                    per.fec_finvac = feccal;
                    per.dias_habil = habil;
                    per.dias_corri = corrp;
                    per.nrt_ruttr = rut;
                    per.año_inicio = peri_tra[ind].ini;
                    per.año_termino = peri_tra[ind].fin;
                    per.tip_uso = tipo;
                    per.nro_solici = soli;
                    per.dias_legal = peri_tra[ind].uleg;
                    per.dias_progr = peri_tra[ind].upro;
                    per.dias_contr = peri_tra[ind].ucon;
                    per.dias_admin = peri_tra[ind].uadm;
                    per.dias_faena = peri_tra[ind].ufae;
                    per.dias_especi = peri_tra[ind].uesp;
                    per.dias_otros = peri_tra[ind].uotr;


                    perical.Add(per);
                    fechai = feccal.AddDays(1);
                }

                int cont = 0;
                var infoPeri = new List<GYCEmpresa.Models.rhueperi>();
                string ruts = Convert.ToString(rut);
                infoPeri = db.rhueperi.Where(i => i.nrt_ruttr == ruts).ToList();
                foreach (var i in infoPeri)
                {
                    if (i.ano_inicio == peri_tra[ind].ini)
                        cont++;
                }
                if (cont == 0)
                {
                    rhueperi peri = new rhueperi();
                    peri.nrt_ruttr = ruts;
                    peri.ano_inicio = peri_tra[ind].ini;
                    peri.ano_termino = peri_tra[ind].fin;
                    peri.dias_legal = peri_tra[ind].dleg;
                    peri.dias_progr = peri_tra[ind].dpro;
                    peri.dias_contr = peri_tra[ind].dcon;
                    peri.dias_admin = peri_tra[ind].dadm;
                    peri.dias_faena = peri_tra[ind].dfae;
                    peri.dias_especi = peri_tra[ind].desp;
                    peri.dias_otros = peri_tra[ind].dotr;
                    peri.fec_trans = fechoy;
                    peri.rut_empr = empresa;
                    db.rhueperi.Add(peri);
                    db.SaveChanges();
                }
            }
            return perical;
        }

        DateTime Calcular_Habiles(DateTime fechai, int dias)
        {
            int ind,ind1,nferi,dferi;
            DateTime fec1;
            fec1 = fechai.Date;
            DateTime[] feri = new DateTime[100];

            corrp = 0;
            nferi = 0;
            var feriInfo = new List<GYCEmpresa.Models.FERIADO>();
            feriInfo = db.FERIADO.ToList();
            foreach (var w in feriInfo)
            {
                if (w.FECHA >= fechai)
                {
                    nferi++;
                    feri[nferi] = w.FECHA;
                }
            }


            for (ind = 1; ind < 1000; ind++)
            {
                byte diasem = (byte)fec1.DayOfWeek;
                dferi = 0;
                for (ind1 = 1; ind1 <= nferi; ind1++)
                {
                    if (feri[ind1] == fec1) dferi = 1;
                }
                if (dferi == 0)
                    if (diasem == 1 || diasem == 2 || diasem == 3 || diasem == 4 || diasem == 5) dias--;
                if (dias == 0)
                { ind = 1000; }
                else
                { fec1 = fec1.AddDays(1); }
                corrp++;
            }
            return fec1;
        }

        public int Carga_Dias()
        {
            string horar;
            var diasInfo = new List<GYCEmpresa.Models.remepage>();
            diasInfo = db.remepage.Where(x=> x.nom_tabla== "VACACION").OrderBy(x=> x.cod_param).ToList();
            int ind;
            for (ind = 0; ind < 100; ind++)
            {
                dias_hor[ind].horar = null;
                dias_hor[ind].leg = 0;
                dias_hor[ind].con = 0;
                dias_hor[ind].adm = 0;
                dias_hor[ind].fae = 0;
                dias_hor[ind].esp = 0;
                dias_hor[ind].otr = 0;
            }
            ind = 0;
            horar = null;
            foreach (var n in diasInfo)
            {
                if(horar != n.cod_param)
                {
                    ind++;
                    dias_hor[ind].horar = Convert.ToString(n.cod_param);
                    horar = n.cod_param;
                }
                if (n.gls_param == "DIAS LEGALES")dias_hor[ind].leg = Convert.ToInt32( n.val_param);
                if (n.gls_param == "DIAS CONTRATO") dias_hor[ind].con = Convert.ToInt32(n.val_param);
                if (n.gls_param == "DIAS FAENA") dias_hor[ind].fae = Convert.ToInt32(n.val_param);
                if (n.gls_param == "DIAS ADMINISTRATIVOS") dias_hor[ind].adm = Convert.ToInt32(n.val_param);
                if (n.gls_param == "DIAS ESPECIALES") dias_hor[ind].esp = Convert.ToInt32(n.val_param);
                if (n.gls_param == "OTROS DIAS") dias_hor[ind].otr = Convert.ToInt32(n.val_param);
            }
            return 1;
        }
     }

    }
