using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Common.CommandTrees;

using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Web.WebSockets;
using GYCEmpresa.App_Start;
using GYCEmpresa.Models;
using Rotativa;
using GYCEmpresa.Controllers;

namespace GYCEmpresa.Models
{        

    public class ComplementoRemuneracion
    {
        private dbgycEntities3 db = new dbgycEntities3();

        string rutEmpresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
        // Datos trabajador
        int dias_trab = 0;
        string tip_trab = "0";
        string tip_linea = "00";
        string cod_mov = "0";
        string fec_inicio = "";
        string fec_termin = "";
        string cod_tramo = "D";
        int cnt_simple = 0;
        int mto_asig = 0;
        int nro_mater = 0;
        int nro_inval = 0;
        int mto_retro = 0;
        int mto_reint = 0;
        int cod_joven = 0;
        // Datos AFP
        string cod_afp = null;
        int mto_impon1 = 0;
        int mto_pension = 0;
        int mto_seguro = 0;
        int mto_cta2 = 0;
        int rta_sustit = 0;
        int por_pact = 0;
        int apo_indem = 0;
        int nro_peri = 0;
        string fec_desde = "";
        string fec_hasta = "";
        string gls_trape = "";
        int por_trape = 0;
        int mto_trape = 0;

        // Datos APVI
        int ins_apvi = 0;
        string con_apvi = null;
        int fla_apvi = 0;
        int mto_apvi = 0;
        int mto_conv = 0;

        // Datos APVC
        int ins_apvc = 0;
        string con_apvc = null;
        int fla_apvc = 0;
        int mto_apvct = 0;
        int mto_apvce = 0;

        // Datos AV
        int rut_afvol = 0;
        String dgv_afvol = null;
        String app_afvol = null;
        String apm_afvol = null;
        String nom_afvol = null;
        String cod_movav = null;
        String fec_desav = null;
        String fec_hasav = null;
        int cod_afpav = 0;
        int mto_penav = 0;
        int mto_ahvav = 0;
        int nro_perav = 0;

        //Datos IPS
        int cod_cajips = 0;
        int tas_cotips = 0;
        int mto_impips = 0;
        int mto_oblips = 0;
        int mto_desips = 0;
        int cod_regips = 0;
        int tas_desips = 0;
        int cot_desips = 0;
        int cot_fonips = 0;
        int cot_accips = 0;
        int bon_leyips = 0;
        int des_carips = 0;
        int bon_gobips = 0;

        //Datos Salud
        int cod_inssal = 0;
        string nro_funsal = null;
        string moneda = null;
        int mto_impsal = 0;
        int mto_plasal = 0;
        int cot_pacsal = 0;
        Decimal cot_oblsal = 0;
        int cot_volsal = 0;
        int mto_gessal = 0;

        //Datos Caja Compensacion
        int cod_cajcc = 0;
        int ren_impcc = 0;
        int cre_percc = 0;
        int des_dencc = 0;
        int des_lescc = 0;
        int des_segcc = 0;
        int des_otrcc = 0;
        int cot_niscc = 0;
        int des_famcc = 0;
        int des_ot1cc = 0;
        int des_ot2cc = 0;
        int bon_gobcc = 0;
        string cod_succc = null;

        //Datos Mutualidad
        int cod_mutual = 0;
        int ren_impmut = 0;
        int cot_accmut = 0;
        int suc_pagmut = 0;

        //Datos seguro Cesantia
        int ren_impsc = 0;
        int apo_trasc = 0;
        int apo_empsc = 0;

        // Datos subsidio
        int rut_subsid = 0;
        string dgv_subsid = null;

        //Datos Otros
        string cen_costo = null;


        public Object Previred()
        {
            SLDocument oSLDocument = new SLDocument();
            System.Data.DataTable dt = new System.Data.DataTable();

            string pathFile = AppDomain.CurrentDomain.BaseDirectory + "Previred.CSV";
            var trabajadores = from m in db.remepers
                               select m;
            if (!String.IsNullOrEmpty(rutEmpresa))
            {
                trabajadores = trabajadores.Where(s => s.rut_empr.Contains(rutEmpresa));
            }
            var paraEmpresa = new List<GYCEmpresa.Models.remepage>();
            paraEmpresa = db.remepage.Where(i => i.rut_empr == rutEmpresa && i.nom_tabla == "PAGO      " && i.cod_param == "1         ").ToList();
            DateTime fecproc1 = DateTime.Now.Date;
            foreach (var p in paraEmpresa)
            {
                fecproc1 = (DateTime)p.fec_param;
            }
            DateTime fecpro = fecproc1;

            DateTime fecproc2 = (DateTime)fecproc1;
            int mes, año, dia;
            String fechas;
            string fecmmaaaa = Convert.ToString(fecproc2);
            mes = fecproc2.Month;
            año = fecproc2.Year;
            fechas = año + "-" + mes + "-" + "01";
            fecini = Convert.ToDateTime(fechas);
            if (mes == 1 || mes == 3 || mes == 5 || mes == 7 || mes == 8 || mes == 10 || mes == 12)
                dia = 31;
            else
                dia = 30;
            if (mes == 2) dia = 28;
            fechas = año + "-" + mes + "-" + dia;
            fecfin = Convert.ToDateTime(fechas);
            fec_inicio = Convert.ToString(fecini);
            fec_termin = Convert.ToString(fecfin);
            string mmaaaa = Convert.ToString(mes)+Convert.ToString(año);

            //RECORRER LISTA DE TRABAJADORES Y LLAMAR calculo 
            //TextWriter Escribir = new StreamWriter(pathFile);
            int ind;
            string inds;
            //for (ind = 1; ind <= 110; ind++)
            //{
            //    inds = Convert.ToString(ind);
            //    dt.Columns.Add(inds, typeof(string));
            //}

// Datos del Trabajador
            dt.Columns.Add("RUT", typeof(string));
            dt.Columns.Add("DV", typeof(string));
            dt.Columns.Add("App.paterno", typeof(string));
            dt.Columns.Add("App.materno", typeof(string));
            dt.Columns.Add("Nombres", typeof(string));
            dt.Columns.Add("Sexo", typeof(string));
            dt.Columns.Add("Nacionalidad", typeof(string));
            dt.Columns.Add("Tip.pago", typeof(string));
            dt.Columns.Add("Per.desde", typeof(string));
            dt.Columns.Add("Per.hasta", typeof(string));
            dt.Columns.Add("Regimen", typeof(string));
            dt.Columns.Add("Tip.trab", typeof(string));
            dt.Columns.Add("Dias", typeof(string));
            dt.Columns.Add("Tip.lin", typeof(string));
            dt.Columns.Add("Cod.mov", typeof(string));
            dt.Columns.Add("Desde1", typeof(string));
            dt.Columns.Add("Hasta1", typeof(string));
            dt.Columns.Add("Tramo", typeof(string));
            dt.Columns.Add("Sim.", typeof(string));
            dt.Columns.Add("Mat.", typeof(string));
            dt.Columns.Add("Inv", typeof(string));
            dt.Columns.Add("Asignación", typeof(string));
            dt.Columns.Add("Retroactiva", typeof(string));
            dt.Columns.Add("Reintegro", typeof(string));
            dt.Columns.Add("Joven", typeof(string));

 // Datos de la AFP
            dt.Columns.Add("AFP", typeof(string));
            dt.Columns.Add("Imp.AFP", typeof(string));
            dt.Columns.Add("Cot.AFP", typeof(string));
            dt.Columns.Add("Seguro", typeof(string));
            dt.Columns.Add("Aho.AFP", typeof(string));
            dt.Columns.Add("Renta sust.", typeof(string));
            dt.Columns.Add("Tasa", typeof(string));
            dt.Columns.Add("Ap.indem", typeof(string));
            dt.Columns.Add("Periodos", typeof(string));
            dt.Columns.Add("Desde2", typeof(string));
            dt.Columns.Add("hasta2", typeof(string));
            dt.Columns.Add("Puesto", typeof(string));
            dt.Columns.Add("Pesado", typeof(string));
            dt.Columns.Add("Cotización", typeof(string));

// Ahorro Voluntario Individual
            dt.Columns.Add("APVI", typeof(string));
            dt.Columns.Add("Cont.Aho", typeof(string));
            dt.Columns.Add("Form.Aho", typeof(string));
            dt.Columns.Add("Coti.Aho", typeof(string));
            dt.Columns.Add("Deposito", typeof(string));

// Ahorro Voluntario Colectivo
            dt.Columns.Add("APVC", typeof(string));
            dt.Columns.Add("Cont.APVC", typeof(string));
            dt.Columns.Add("Form.APVC", typeof(string));
            dt.Columns.Add("Trab.APVC", typeof(string));
            dt.Columns.Add("Empl.APVC", typeof(string));

 // Datos Afiliado Voluntario
            dt.Columns.Add("Rut.afil.", typeof(string));
            dt.Columns.Add("DV1", typeof(string));
            dt.Columns.Add("Apaterno", typeof(string));
            dt.Columns.Add("Amaterno", typeof(string));
            dt.Columns.Add("Nombres1", typeof(string));
            dt.Columns.Add("Movim.", typeof(string));
            dt.Columns.Add("Desde3", typeof(string));
            dt.Columns.Add("Hasta3", typeof(string));
            dt.Columns.Add("Cod.AFP", typeof(string));
            dt.Columns.Add("Voluntaria", typeof(string));
            dt.Columns.Add("Ahorro", typeof(string));
            dt.Columns.Add("Periodos1", typeof(string));

 // Datos IPS ISL Fonasa
            dt.Columns.Add("Caja", typeof(string));
            dt.Columns.Add("Cot.IPS", typeof(string));
            dt.Columns.Add("Impon.IPS", typeof(string));
            dt.Columns.Add("Oblig.IPS", typeof(string));
            dt.Columns.Add("Desahucio", typeof(string));
            dt.Columns.Add("Cod.caja", typeof(string));
            dt.Columns.Add("Tasa des.", typeof(string));
            dt.Columns.Add("Cotiz.des", typeof(string));
            dt.Columns.Add("Cotiz.fonasa", typeof(string));
            dt.Columns.Add("Cotiz.acc", typeof(string));
            dt.Columns.Add("ley 15386", typeof(string));
            dt.Columns.Add("Descuento", typeof(string));
            dt.Columns.Add("Bono", typeof(string));
 
 // Datos Salud
            dt.Columns.Add("Isapre", typeof(string));
            dt.Columns.Add("FUN", typeof(string));
            dt.Columns.Add("Impon.isapre", typeof(string));
            dt.Columns.Add("Moneda", typeof(string));
            dt.Columns.Add("Pactada", typeof(string));
            dt.Columns.Add("Obligatoria", typeof(string));
            dt.Columns.Add("Voluntaria1", typeof(string));
            dt.Columns.Add("Ges", typeof(string));

 // Datos caja compensación
            dt.Columns.Add("CCAF", typeof(string));
            dt.Columns.Add("Impon.CCAF", typeof(string));
            dt.Columns.Add("Credito", typeof(string));
            dt.Columns.Add("Dental", typeof(string));
            dt.Columns.Add("Leasing", typeof(string));
            dt.Columns.Add("Seg.CCAF", typeof(string));
            dt.Columns.Add("Cotiz.CCAF", typeof(string));
            dt.Columns.Add("Fam.CCAF", typeof(string));
            dt.Columns.Add("Otros1", typeof(string));
            dt.Columns.Add("Otros2", typeof(string));
            dt.Columns.Add("Bonos", typeof(string));
            dt.Columns.Add("Sucursal", typeof(string));

 // Datos Mutualidad
            dt.Columns.Add("Mutual", typeof(string));
            dt.Columns.Add("Imp.mutual", typeof(string));
            dt.Columns.Add("Cot.mutual", typeof(string));
            dt.Columns.Add("Suc.mutual", typeof(string));

// Seguro cesantia
            dt.Columns.Add("Imp.SC", typeof(string));
            dt.Columns.Add("Ap.trabajador", typeof(string));
            dt.Columns.Add("Ap.empleador", typeof(string));

// Datos pagador de subcidios
            dt.Columns.Add("Rut.Pagador", typeof(string));
            dt.Columns.Add("DV2", typeof(string));

// Otros Datos Empresa
            dt.Columns.Add("Sucursal1", typeof(string));
            dt.Columns.Add("Region", typeof(string));


            string linea;
            foreach (var t in trabajadores)
            {
                try
                {
                    linea = ProcesaTrabajador(t.nrt_ruttr, t.dgv_ruttr, t.nom_appat, t.nom_apmat, t.nom_nomtr, t.flg_sexo, fecpro, mmaaaa);


                }
                catch (Exception)
                {
                    linea = "";
                }
                //Escribir.WriteLine(linea);

                dt.Rows.Add(t.nrt_ruttr, t.dgv_ruttr, t.nom_appat, t.nom_apmat, t.nom_nomtr, t.flg_sexo, "0", "01", mmaaaa, mmaaaa, "AFP","0"
                         , dias_trab,  tip_linea, cod_mov, fec_inicio, fec_termin, cod_tramo, cnt_simple, mto_asig, nro_mater
                         , nro_inval, mto_retro, mto_reint, cod_joven, cod_afp, mto_impon1, mto_pension, mto_seguro, mto_cta2, rta_sustit
                         , por_pact, apo_indem, nro_peri, fec_desde, fec_hasta, gls_trape, por_trape, mto_trape, ins_apvi, con_apvi, fla_apvi
                         , mto_apvi, mto_conv, ins_apvc, con_apvc, fla_apvc, mto_apvct,mto_apvce, rut_afvol, dgv_afvol, app_afvol, apm_afvol, nom_afvol
                         , cod_movav, fec_desav, fec_hasav, cod_afpav, mto_penav, mto_ahvav, nro_perav, cod_cajips, tas_cotips, mto_impips
                         , mto_oblips, mto_desips, cod_regips, tas_desips, cot_desips, cot_fonips, cot_accips, bon_leyips, des_carips, bon_gobips
                         , cod_inssal, nro_funsal, mto_impsal,moneda, mto_plasal, cot_oblsal, cot_volsal, mto_gessal, cod_cajcc, ren_impcc
                         , cre_percc, des_dencc, des_lescc, des_segcc, cot_niscc, des_famcc, des_ot1cc, des_ot2cc, bon_gobcc, cod_succc
                         , cod_mutual, ren_impmut, cot_accmut, suc_pagmut, ren_impsc, apo_trasc, apo_empsc, rut_subsid, dgv_subsid, cen_costo);
            }
            //Escribir.Close();
            dt.Rows.RemoveAt(1);
            oSLDocument.ImportDataTable(1, 1, dt, true);
            var file = new MemoryStream();
            oSLDocument.SaveAs(file);
            return file;

        }
        public string ProcesaTrabajador(string nrorut, string dgv, string app, string apm, string nom, string sexo, DateTime fecpro, string mmaaaa)
        {
            var inforesul = new List<GYCEmpresa.Models.remeresu>();
            inforesul = db.remeresu.Where(i => i.nrt_ruttr == nrorut && i.fec_pago == fecpro).ToList();
            string linea = null;
            string linea00 = null;
            string linea01 = null;
            string linea02 = null;
            string linea03 = null;
            string linea04 = null;
            string linea05 = null;
            string linea06 = null;
            string linea07 = null;
            string linea08 = null;
            string linea09 = null;
            string linea10 = null;
            string linea11 = null;
            string linea12 = null;
            foreach (var i in inforesul)
            {
                // Dias Trabajados
                if (i.cod_conce == 1)
                {
                    dias_trab = (int)i.cnt_conce;
                }

                // Cargas familiares
                if (i.cod_conce == 14)
                {
                    if (i.correl == 1) cod_tramo = "A";
                    if (i.correl == 2) cod_tramo = "B";
                    if (i.correl == 3) cod_tramo = "C";
                    cnt_simple = (int)i.cnt_conce;
                    mto_asig = (int)i.mto_pagar;
                }

                // Imponible con tope
                if (i.cod_conce == 2011)
                {
                    mto_impon1 = (int)i.mto_pagar;
                    mto_impips = mto_impon1;
                    mto_impsal = mto_impon1;
                }
                // Datos AFP
                if (i.cod_conce == 910)
                {
                    mto_pension = (int)i.mto_pagar;
                    cod_afp = Convert.ToString(i.cnt_conce);
                    por_pen = (Decimal) i.val_conce;
                }
                if (i.cod_conce == 2401)
                {
                    mto_seguro = (int)i.mto_pagar;
                }
                //Datos Salud
                if (i.cod_conce == 921)
                {
                    mto_plasal = (int)i.mto_pagar;
                    cot_oblsal = (Decimal)i.mto_pagar;
                    cod_inssal = (int)i.cnt_conce;
                }
                //Datos seguro Cesantia
                if (i.cod_conce == 2400)
                {
                    apo_empsc = (int)i.mto_pagar;
                }
                if (i.cod_conce == 916)
                {
                    apo_trasc = (int)i.mto_pagar;
                }

                if (i.cod_conce == 2012)
                {
                    ren_impsc = (int)i.mto_pagar;
                }

                if (i.cod_conce == 2402 && i.mto_pagar >0)
                {
                    ren_impmut = mto_impon1;
                    cot_accmut = (int)i.mto_pagar;
                }


            }

            // Datos Trabajador
            linea00 = nrorut + ";" + dgv + ";" + app + ";" + apm + ";" + nom + ";" + sexo + ";0;01;" + mmaaaa + ";" + fecpro + ";AFP;";
            linea01 = tip_trab + ";" + dias_trab + ";" + tip_linea + ";" + cod_mov + ";" + fec_inicio + ";" + fec_termin + ";" +
                cod_tramo + ";" + cnt_simple + ";" + nro_mater + ";" + nro_inval + ";" + mto_asig + ";" + mto_retro + ";" + mto_reint + ";" + cod_joven;

            // Datos AFP
            linea02 = cod_afp + ";" + mto_impon1 + ";" + mto_pension + ";" + mto_seguro + ";" + mto_cta2 + ";" + rta_sustit + ";" +
                por_pact + ";" + apo_indem + ";" + nro_peri + ";" + fec_desde + ";" + fec_hasta + ";" + gls_trape + ";" + por_trape + ";" + mto_trape;

            // Datos APVI
            linea03 = ins_apvi + ";" + con_apvi + ";" + fla_apvi + ";" + mto_apvi + ";" + mto_conv;

            // Datos APVC
            linea04 = ins_apvc + ";" + con_apvc + ";" + fla_apvc + ";" + mto_apvct+";"+ mto_apvce;

            // Datos AV
            linea05 = rut_afvol + ";" + dgv_afvol + ";" + app_afvol + ";" + apm_afvol + ";" + nom_afvol + ";" + cod_movav + ";" +
                fec_desav + ";" + fec_hasav + ";" + cod_afpav + ";" + mto_penav + ";" + mto_ahvav + ";" + nro_perav;

            //Datos IPS
            linea06 = cod_cajips + ";" + tas_cotips + ";" + mto_impips + ";" + mto_oblips + ";" + mto_desips + ";" + cod_regips + ";" +
                tas_desips + ";" + cot_desips + ";" + cot_fonips + ";" + cot_accips + ";" + bon_leyips + ";" + des_carips + ";" + bon_gobips;

            //Datos Salud
            linea07 = cod_inssal + ";" + nro_funsal + ";" + mto_impsal + ";" + mto_plasal + ";" + cot_pacsal + ";" + cot_oblsal + ";" +
                cot_volsal + ";" + mto_gessal;

            //Datos Caja Compensacion
            linea08 = cod_cajcc + ";" + ren_impcc + ";" + cre_percc + ";" + des_dencc + ";" + des_lescc + ";" + des_segcc + ";" +
                des_otrcc + ";" + cot_niscc + ";" + des_famcc + ";" + des_ot1cc + ";" + des_ot2cc + ";" + bon_gobcc + ";" + cod_succc;

            //Datos Mutualidad
            linea09 = cod_mutual + ";" + ren_impmut + ";" + cot_accmut + ";" + suc_pagmut;

            //Datos seguro Cesantia
            linea10 = ren_impsc + ";" + apo_trasc + ";" + apo_empsc;

            // Datos subsidio
            linea11 = rut_subsid + ";" + dgv_subsid;

            //Datos Otros
            linea12 = cen_costo;



            linea = linea00 + linea01 + ";" + linea02 + ";" + linea03 + ";" + linea04 + ";" + linea05 + ";" + linea06 + ";" +
                    linea07 + ";" + linea08 + ";" + linea09 + ";" + linea10 + ";" + linea11 + ";" + linea12;

            return linea;
        }

    

    public struct marcas
        {
            public string fec;
            public string tip;
            public string mar;
        }

        marcas[] marrel = new marcas[1000];
        int dias;
        DateTime fecini, fecfin;



        int ndias;
        string rut_trab;
        Decimal dias_tra;
        Decimal sueldo;
        Decimal sue_pag;
        decimal hrs1, hrs2, hrs3;
        decimal sob1, sob2, sob3;
        Decimal dcol, dmov;
        Decimal col, mov, bono, grati, fami, espe, impon;
        Decimal tot_hab, tot_otr;
        Decimal por_pen, pension, por_seg, seguro, uf_sal, salud, cesantia, tot_ley, apv;
        Decimal tri, imp, ant, pre, otros_dct, tot_dct, saldo;
        String nom, app, apm, permiso;
        DateTime[] fechas = new DateTime[100];
        string[] fecstr = new string[100];
        String[] entrada = new string[100];
        String[] salida = new string[100];
        string linea;
        int bit;
        public Object Generar_ResumenDePago()
        {


            SLDocument oSLDocument = new SLDocument();

            System.Data.DataTable dt = new System.Data.DataTable();

            //columnas
            dt.Columns.Add("RUT", typeof(string));
            dt.Columns.Add("Nombres", typeof(string));
            dt.Columns.Add("App.Paterno", typeof(string));
            dt.Columns.Add("App.Materno", typeof(string));
            dt.Columns.Add("Sdo.Base", typeof(Decimal));
            dt.Columns.Add("Trab.", typeof(Decimal));
            dt.Columns.Add("Sueldo", typeof(Decimal));
            dt.Columns.Add("Hrs 1", typeof(Decimal));
            dt.Columns.Add("H.extras 1", typeof(Decimal));
            dt.Columns.Add("Hrs 2", typeof(Decimal));
            dt.Columns.Add("H.extra 2", typeof(Decimal));
            dt.Columns.Add("Hrs. 3", typeof(Decimal));
            dt.Columns.Add("H.extra 3", typeof(Decimal));
            dt.Columns.Add("Bono", typeof(Decimal));
            dt.Columns.Add("Gratificacion", typeof(Decimal));
            dt.Columns.Add("D.col", typeof(Decimal));
            dt.Columns.Add("Colacion", typeof(Decimal));
            dt.Columns.Add("D.mov", typeof(Decimal));
            dt.Columns.Add("Movilizacion", typeof(Decimal));
            dt.Columns.Add("Familiar", typeof(Decimal));
            dt.Columns.Add("Asig.Espec.", typeof(Decimal));
            dt.Columns.Add("Otros Hab.", typeof(Decimal));
            dt.Columns.Add("Total haberes", typeof(Decimal));
            dt.Columns.Add("Imponible", typeof(Decimal));
            dt.Columns.Add("Cot.Pen", typeof(Decimal));
            dt.Columns.Add("Pension", typeof(Decimal));
            dt.Columns.Add("Cot.Seg", typeof(Decimal));
            dt.Columns.Add("Seguro", typeof(Decimal));
            dt.Columns.Add("U.F.", typeof(Decimal));
            dt.Columns.Add("Salud", typeof(Decimal));
            dt.Columns.Add("Cesantia", typeof(Decimal));
            dt.Columns.Add("Tot. Leyes", typeof(Decimal));
            dt.Columns.Add("APV", typeof(Decimal));
            dt.Columns.Add("Tributable", typeof(Decimal));
            dt.Columns.Add("Impuesto", typeof(Decimal));
            dt.Columns.Add("Anticipos", typeof(Decimal));
            dt.Columns.Add("Prestamos", typeof(Decimal));
            dt.Columns.Add("Otros Dct.", typeof(Decimal));
            dt.Columns.Add("Total Descuentos", typeof(Decimal));
            dt.Columns.Add("Liquido", typeof(Decimal));

            //registros
            var rutEmpresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            var trabajadores = from m in db.remepers
                               select m;

            if (!String.IsNullOrEmpty(rutEmpresa))
            {
                trabajadores = trabajadores.Where(s => s.rut_empr.Contains(rutEmpresa));
            }
            string rut;

            //RECORRER LISTA DE TRABAJADORES Y LLAMAR calculo 
            foreach (var t in trabajadores)
            {
                try
                {
                    tot_hab = 0;
                    tot_otr = 0;
                    otros_dct = 0;
                    tot_dct = 0;
                    sueldo = 0;
                    dias_tra = 0;
                    sue_pag = 0;
                    hrs1 = 0;
                    sob1 = 0;
                    hrs2 = 0;
                    sob2 = 0;
                    hrs3 = 0;
                    sob3 = 0;
                    bono = 0;
                    grati = 0;
                    dcol = 0;
                    col = 0;
                    dmov = 0;
                    mov = 0;
                    fami = 0;
                    espe = 0;
                    tot_otr = 0;
                    tot_hab = 0;
                    impon = 0;
                    por_pen = 0;
                    pension = 0;
                    por_seg = 0;
                    seguro = 0;
                    uf_sal = 0;
                    salud = 0;
                    cesantia = 0;
                    tot_ley = 0;
                    apv = 0;
                    tri = 0;
                    imp = 0;
                    ant = 0;
                    pre = 0;
                    otros_dct = 0;
                    tot_dct = 0;
                    saldo = 0;
                    rut = t.nrt_ruttr.ToString() + "-" + t.dgv_ruttr;
                    rut_trab = t.nrt_ruttr;
                    int resu = Detalle_resultados();
                    tot_otr = tot_hab - tot_otr;
                    otros_dct = tot_dct - otros_dct - tot_ley - imp;
                    dt.Rows.Add(rut, t.nom_nomtr, t.nom_appat, t.nom_apmat, sueldo, dias_tra, sue_pag, hrs1, sob1, hrs2,
                                sob2, hrs3, sob3, bono, grati, dcol, col, dmov, mov, fami, espe, tot_otr, tot_hab,
                                impon, por_pen, pension, por_seg, seguro, uf_sal, salud, cesantia, tot_ley, apv, tri, imp, ant, pre,
                                otros_dct, tot_dct, saldo);
                }
                catch (Exception)
                {

                }
            }



            oSLDocument.ImportDataTable(1, 1, dt, true);
            var file = new MemoryStream();
            oSLDocument.SaveAs(file);
            return file;
        }
        int Detalle_resultados()
        {
            var detresultados = new List<GYCEmpresa.Models.remeresu>();
            detresultados = db.remeresu.Where(i => i.nrt_ruttr == rut_trab).ToList();
            int conc;
            foreach (var h in detresultados)
            {
                conc = h.cod_conce;

                if (conc < 100 && conc > 0)
                {
                    tot_hab = tot_hab + (Decimal)h.mto_pagar;
                }
                if (conc == 1)
                {
                    sueldo = (Decimal)h.val_conce;
                    dias_tra = (Decimal)h.cnt_conce;
                    sue_pag = (Decimal)h.mto_pagar;
                    tot_otr = tot_otr + (Decimal)h.mto_pagar;

                }
                if (conc == 3)
                {
                    hrs1 = (Decimal)h.cnt_conce;
                    sob1 = (Decimal)h.mto_pagar;
                    tot_otr = tot_otr + (Decimal)h.mto_pagar;
                }
                if (conc == 4)
                {
                    hrs2 = (Decimal)h.cnt_conce;
                    sob2 = (Decimal)h.mto_pagar;
                    tot_otr = tot_otr + (Decimal)h.mto_pagar;
                }
                if (conc == 6)
                {
                    grati = (Decimal)h.mto_pagar;
                    tot_otr = tot_otr + (Decimal)h.mto_pagar;
                }
                if (conc == 8)
                {
                    bono = (Decimal)h.mto_pagar;
                    tot_otr = tot_otr + (Decimal)h.mto_pagar;
                }
                if (conc == 9)
                {
                    dcol = (Decimal)h.cnt_conce;
                    col = (Decimal)h.mto_pagar;
                    tot_otr = tot_otr + (Decimal)h.mto_pagar;
                }
                if (conc == 12)
                {
                    espe = (Decimal)h.mto_pagar;
                    tot_otr = tot_otr + (Decimal)h.mto_pagar;
                }
                if (conc == 14)
                {
                    fami = (Decimal)h.mto_pagar;
                    tot_otr = tot_otr + (Decimal)h.mto_pagar;
                }
                if (conc == 2011)
                {
                    impon = (Decimal)h.mto_pagar;
                }
                if (conc == 910)
                {
                    por_pen = (Decimal)h.val_conce;
                    pension = (Decimal)h.mto_pagar;
                }
                if (conc == 911)
                {
                    por_seg = (Decimal)h.val_conce;
                    seguro = (Decimal)h.mto_pagar;
                }
                if (conc == 921)
                {
                    uf_sal = (Decimal)h.val_conce;
                    salud = (Decimal)h.mto_pagar;
                }
                if (conc == 916)
                {
                    cesantia = (Decimal)h.mto_pagar;
                }
                if (conc == 2210)
                {
                    tot_ley = (Decimal)h.mto_pagar;
                }
                if (conc == 940)
                {
                    apv = (Decimal)h.mto_pagar;
                }
                if (conc == 2030)
                {
                    tri = (Decimal)h.mto_pagar;
                }
                if (conc == 990)
                {
                    imp = (Decimal)h.mto_pagar;
                }
                if (conc == 101)
                {
                    ant = (Decimal)h.mto_pagar;
                    otros_dct = otros_dct + (decimal)h.mto_pagar;
                }
                if (conc == 200)
                {
                    pre = (Decimal)h.mto_pagar;
                    otros_dct = otros_dct + (decimal)h.mto_pagar;
                }
                if (conc > 99 && conc < 1000)
                {
                    tot_dct = tot_dct + (decimal)h.mto_pagar;
                }
                if (conc == 2300)
                {
                    saldo = (Decimal)h.mto_pagar;
                }


            }
            return 1;
        }


        public int Cargar_Archivo(HttpPostedFileBase file, string rutemp)
        {
            string titulo;

            int iRow = 2;

            SLDocument oSLDocument = new SLDocument(file.InputStream);

            string ruttr;
            int correl;
            int haber;
            string afecta;
            string pago;
            string mtopo;
            decimal valor;
            int cantid;
            string desde;
            string hasta;
            string ingre;
            int pagina;
            string empre;
            int descu;
            int rutbe;
            string fecasi;
            int diaina;
            //string codina;
            Decimal hrsob1;
            Decimal hrsob2;
            Decimal hrsob3;
            int diacol;
            int hrscol;
            int diamov;

            titulo = oSLDocument.GetCellValueAsString(1, 3);
            if (titulo == "cod_haber")
            {
                while (!string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(iRow, 1)))
                {
                    DateTime inicio, termino, ingreso;

                    ruttr = oSLDocument.GetCellValueAsString(iRow, 1);
                    correl = oSLDocument.GetCellValueAsInt32(iRow, 2);
                    haber = oSLDocument.GetCellValueAsInt32(iRow, 3);
                    afecta = oSLDocument.GetCellValueAsString(iRow, 4);
                    pago = oSLDocument.GetCellValueAsString(iRow, 5);
                    mtopo = oSLDocument.GetCellValueAsString(iRow, 6);
                    valor = oSLDocument.GetCellValueAsDecimal(iRow, 7);
                    cantid = oSLDocument.GetCellValueAsInt32(iRow, 8);
                    desde = oSLDocument.GetCellValueAsString(iRow, 9);
                    hasta = oSLDocument.GetCellValueAsString(iRow, 10);
                    ingre = oSLDocument.GetCellValueAsString(iRow, 11);
                    pagina = oSLDocument.GetCellValueAsInt32(iRow, 12);
                    empre = oSLDocument.GetCellValueAsString(iRow, 13);

                    DateTime.TryParse(desde, out inicio);
                    DateTime.TryParse(hasta, out termino);
                    DateTime.TryParse(ingre, out ingreso);


                    var remehain = new remehain();
                    remehain.nrt_ruttr = ruttr;
                    remehain.nro_correl = correl;
                    remehain.cod_haber = haber;
                    remehain.flg_afecta = afecta;
                    remehain.cod_pago = pago;
                    remehain.flg_mtopo = mtopo;
                    remehain.val_haber = valor;
                    remehain.cnt_dias = cantid;
                    remehain.fec_desde = inicio;
                    remehain.fec_hasta = termino;
                    remehain.fec_ingre = ingreso;
                    remehain.num_pagin = Convert.ToInt16(pagina);
                    remehain.rut_empr = empre;

                    db.remehain.Add(remehain);
                    db.SaveChanges();

                    iRow++;
                }
            }
            else
            {
                if (titulo == "cod_descu")
                {

                    while (!string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(iRow, 1)))
                    {
                        DateTime inicio, termino, ingreso;

                        ruttr = oSLDocument.GetCellValueAsString(iRow, 1);
                        correl = oSLDocument.GetCellValueAsInt32(iRow, 2);
                        descu = oSLDocument.GetCellValueAsInt32(iRow, 3);
                        afecta = oSLDocument.GetCellValueAsString(iRow, 4);
                        haber = oSLDocument.GetCellValueAsInt32(iRow, 5);
                        pago = oSLDocument.GetCellValueAsString(iRow, 6);
                        mtopo = oSLDocument.GetCellValueAsString(iRow, 7);
                        valor = oSLDocument.GetCellValueAsDecimal(iRow, 8);
                        desde = oSLDocument.GetCellValueAsString(iRow, 9);
                        hasta = oSLDocument.GetCellValueAsString(iRow, 10);
                        ingre = oSLDocument.GetCellValueAsString(iRow, 11);
                        pagina = oSLDocument.GetCellValueAsInt32(iRow, 12);
                        rutbe = oSLDocument.GetCellValueAsInt32(iRow, 13);
                        empre = oSLDocument.GetCellValueAsString(iRow, 14);

                        DateTime.TryParse(desde, out inicio);
                        DateTime.TryParse(hasta, out termino);
                        DateTime.TryParse(ingre, out ingreso);


                        var remedein = new remedein();
                        remedein.nrt_ruttr = ruttr;
                        remedein.nro_correl = Convert.ToInt16(correl);
                        remedein.cod_descu = haber;
                        remedein.flg_afecta = afecta;
                        remedein.cod_haber = haber;
                        remedein.cod_pago = pago;
                        remedein.flg_forde = mtopo;
                        remedein.val_descu = valor;
                        remedein.fec_desde = inicio;
                        remedein.fec_hasta = termino;
                        remedein.fec_ingre = ingreso;
                        remedein.num_pagin = Convert.ToInt16(pagina);
                        remedein.nrt_rutbe = rutbe;
                        remedein.rut_empr = empre;

                        db.remedein.Add(remedein);
                        db.SaveChanges();

                        iRow++;
                    }
                }
                else
                {
                    if (titulo == "cnt_inasi")
                    {

                        while (!string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(iRow, 1)))
                        {
                            DateTime inicio;

                            ruttr = oSLDocument.GetCellValueAsString(iRow, 1);
                            fecasi = oSLDocument.GetCellValueAsString(iRow, 2);
                            diaina = oSLDocument.GetCellValueAsInt32(iRow, 3);
                            //codina = oSLDocument.GetCellValueAsString(iRow, 4);
                            hrsob1 = oSLDocument.GetCellValueAsDecimal(iRow, 4);
                            hrsob2 = oSLDocument.GetCellValueAsDecimal(iRow, 5);
                            hrsob3 = oSLDocument.GetCellValueAsDecimal(iRow, 6);
                            diacol = oSLDocument.GetCellValueAsInt32(iRow, 7);
                            hrscol = oSLDocument.GetCellValueAsInt32(iRow, 8);
                            diamov = oSLDocument.GetCellValueAsInt32(iRow, 9);
                            empre = oSLDocument.GetCellValueAsString(iRow, 10);

                            DateTime.TryParse(fecasi, out inicio);


                            var remeasis = new remeasis();
                            remeasis.nrt_ruttr = ruttr;
                            remeasis.fec_asist = inicio;
                            remeasis.cnt_inasi = diaina;
                            remeasis.hrs_sob1 = hrsob1;
                            remeasis.hrs_sob2 = hrsob2;
                            remeasis.hrs_sob3 = hrsob3;
                            remeasis.dia_colac = diacol;
                            remeasis.hrs_colac = hrscol;
                            remeasis.dia_movil = diamov;
                            remeasis.rut_empr = empre;

                            db.remeasis.Add(remeasis);
                            db.SaveChanges();

                            iRow++;

                        }
                    }
                    else
                    {
                        if (titulo == "nom_appat")
                        {

                            while (!string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(iRow, 1)))
                            {
                                DateTime inicio;

                                ruttr = oSLDocument.GetCellValueAsString(iRow, 1);
                                string dgvrut = oSLDocument.GetCellValueAsString(iRow, 2);
                                string nomapp = oSLDocument.GetCellValueAsString(iRow, 3);
                                string nomapm = oSLDocument.GetCellValueAsString(iRow, 4);
                                string nomnom = oSLDocument.GetCellValueAsString(iRow, 5);
                                string fecnac = oSLDocument.GetCellValueAsString(iRow, 6);
                                string sexo = oSLDocument.GetCellValueAsString(iRow, 7);
                                string estciv = oSLDocument.GetCellValueAsString(iRow, 8);
                                string direcc = oSLDocument.GetCellValueAsString(iRow, 9);
                                string ciudad = oSLDocument.GetCellValueAsString(iRow, 10);
                                string rut = oSLDocument.GetCellValueAsString(iRow, 11);

                                //string nacion = oSLDocument.GetCellValueAsString(iRow, 11);

                                DateTime.TryParse(fecnac, out inicio);


                                var remepers = new remepers();
                                remepers.nrt_ruttr = ruttr;
                                remepers.dgv_ruttr = dgvrut;
                                remepers.nom_apmat = nomapp;
                                remepers.nom_apmat = nomapm;
                                remepers.nom_nomtr = nomnom;
                                //remepers.fec_nacim = inicio;
                                remepers.flg_sexo = sexo;
                                remepers.flg_estci = estciv;
                                remepers.gls_direcc = direcc;
                                remepers.gls_ciudad = ciudad;
                                remepers.rut_empr = rut;

                                db.remepers.Add(remepers);
                                db.SaveChanges();

                                iRow++;

                            }
                        }

                        else
                        {
                            if (titulo == "cod_roltr")
                            {

                                while (!string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(iRow, 1)))
                                {
                                    DateTime inicio, indem, categ, sindi, cargo;

                                    ruttr = oSLDocument.GetCellValueAsString(iRow, 1);
                                    string fecing = oSLDocument.GetCellValueAsString(iRow, 2);
                                    string codrol = oSLDocument.GetCellValueAsString(iRow, 3);
                                    string codcen = oSLDocument.GetCellValueAsString(iRow, 4);
                                    string fecind = oSLDocument.GetCellValueAsString(iRow, 5);
                                    string codcat = oSLDocument.GetCellValueAsString(iRow, 6);
                                    string feccat = oSLDocument.GetCellValueAsString(iRow, 7);
                                    int sueldo = oSLDocument.GetCellValueAsInt32(iRow, 8);
                                    string codsin = oSLDocument.GetCellValueAsString(iRow, 9);
                                    string fecsin = oSLDocument.GetCellValueAsString(iRow, 10);
                                    string codine = oSLDocument.GetCellValueAsString(iRow, 11);
                                    string codcor = oSLDocument.GetCellValueAsString(iRow, 12);
                                    string codcar = oSLDocument.GetCellValueAsString(iRow, 13);
                                    string feccar = oSLDocument.GetCellValueAsString(iRow, 14);
                                    string tipcon = oSLDocument.GetCellValueAsString(iRow, 15);
                                    string codhor = oSLDocument.GetCellValueAsString(iRow, 16);
                                    string flgsup = oSLDocument.GetCellValueAsString(iRow, 17);
                                    //string rut = oSLDocument.GetCellValueAsString(iRow, 18);
                                    //string nacion = oSLDocument.GetCellValueAsString(iRow, 11);

                                    DateTime.TryParse(fecing, out inicio);
                                    DateTime.TryParse(fecind, out indem);
                                    DateTime.TryParse(feccat, out categ);
                                    DateTime.TryParse(fecsin, out sindi);
                                    DateTime.TryParse(feccar, out cargo);


                                    var remelabo = new remelabo();
                                    remelabo.nrt_ruttr = ruttr;
                                    remelabo.fec_ingre = inicio;
                                    remelabo.cod_roltr = codrol;
                                    remelabo.cod_cenre = codcen;
                                    remelabo.fec_indem = indem;
                                    remelabo.cod_categ = codcat;
                                    remelabo.fec_categ = categ;
                                    remelabo.mto_sueldo = sueldo;
                                    remelabo.cod_sindic = codsin;
                                    remelabo.fec_sindic = sindi;
                                    remelabo.cod_ine = codine;
                                    remelabo.cod_corfo = codcor;
                                    remelabo.cod_cargo = codcar;
                                    remelabo.fec_cargo = cargo;
                                    remelabo.flg_tipcon = tipcon;
                                    remelabo.cod_horar = codhor;
                                    remelabo.flg_suple = flgsup;
                                    remelabo.rut_empr = rutemp;

                                    db.remelabo.Add(remelabo);
                                    db.SaveChanges();

                                    iRow++;

                                }
                            }
                            else
                            {
                                if (titulo == "cod_instit")
                                {

                                    while (!string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(iRow, 1)))
                                    {
                                        DateTime ini, ing, ter;

                                        ruttr = oSLDocument.GetCellValueAsString(iRow, 1);
                                        string tipins = oSLDocument.GetCellValueAsString(iRow, 2);
                                        string codins = oSLDocument.GetCellValueAsString(iRow, 3);
                                        string nrocor = oSLDocument.GetCellValueAsString(iRow, 4);
                                        string tipapo = oSLDocument.GetCellValueAsString(iRow, 5);
                                        Decimal valapo = oSLDocument.GetCellValueAsDecimal(iRow, 6);
                                        string fecini = oSLDocument.GetCellValueAsString(iRow, 7);
                                        string fecter = oSLDocument.GetCellValueAsString(iRow, 9);
                                        string fecing = oSLDocument.GetCellValueAsString(iRow, 10);

                                        DateTime.TryParse(fecing, out ing);
                                        DateTime.TryParse(fecter, out ter);
                                        DateTime.TryParse(fecini, out ini);


                                        var remeinst = new remeinst();
                                        remeinst.nrt_rutr = ruttr;
                                        remeinst.fec_ingreso = ing;
                                        remeinst.tip_instit = tipins;
                                        remeinst.cod_instis = codins;
                                        remeinst.correl = Convert.ToInt16(nrocor);
                                        remeinst.tip_aporte = tipapo;
                                        remeinst.val_aporte = valapo;
                                        remeinst.fec_inicio = ini;
                                        remeinst.fec_termin = ter;
                                        remeinst.rut_empr = rutemp;

                                        db.remeinst.Add(remeinst);
                                        db.SaveChanges();

                                        iRow++;

                                    }
                                }
                                //else
                                //{
                                //    // Otro Formato

                                //    string mes;
                                //    string anio;
                                //    String monto;
                                //    DateTime inicio;
                                //    DateTime termino;
                                //    DateTime hoydia;
                                //    string fec;
                                //    int codigo;
                                //    while (!string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(iRow, 1)))
                                //    {

                                //        mes = oSLDocument.GetCellValueAsString(iRow, 1);
                                //        anio = oSLDocument.GetCellValueAsString(iRow, 2);
                                //        fec = "01-" + mes + "-" + anio;
                                //        DateTime.TryParse(fec, out inicio);
                                //        fec = "28-" + mes + "-" + anio;
                                //        DateTime.TryParse(fec, out termino);
                                //        hoydia = DateTime.Today;
                                //        string[] ruti = oSLDocument.GetCellValueAsString(iRow, 3).Split('-');
                                //        ruttr = ruti[0];
                                //        codigo = oSLDocument.GetCellValueAsInt32(iRow, 4);

                                //        monto = oSLDocument.GetCellValueAsString(iRow, 5);
                                //        empre = rutemp;

                                //        if (codigo == 1000)
                                //        {

                                //            var remeasis = new remeasis();
                                //            remeasis.nrt_ruttr = ruttr;
                                //            remeasis.fec_asist = inicio;
                                //            remeasis.cnt_inasi = Convert.ToInt32(monto);
                                //            remeasis.hrs_sob1 = 0;
                                //            remeasis.hrs_sob2 = 0;
                                //            remeasis.hrs_sob3 = 0;
                                //            remeasis.dia_colac = 0;
                                //            remeasis.hrs_colac = 0;
                                //            remeasis.dia_movil = 0;
                                //            remeasis.rut_empr = rutemp;

                                //            db.remeasis.Add(remeasis);
                                //            db.SaveChanges();
                                //        }
                                //        if (codigo == 1001)
                                //        {

                                //            var remeasis = new remeasis();
                                //            remeasis.nrt_ruttr = ruttr;
                                //            remeasis.fec_asist = inicio;
                                //            remeasis.hrs_sob1 = Convert.ToDecimal(monto) / 100;
                                //            remeasis.cnt_inasi = 0;
                                //            remeasis.hrs_sob2 = 0;
                                //            remeasis.hrs_sob3 = 0;
                                //            remeasis.dia_colac = 0;
                                //            remeasis.hrs_colac = 0;
                                //            remeasis.dia_movil = 0;
                                //            remeasis.rut_empr = rutemp;

                                //            db.remeasis.Add(remeasis);
                                //            db.SaveChanges();
                                //        }
                                //        if (codigo > 0 && codigo < 100)
                                //        {
                                //            var remehain = new remehain();

                                //            remehain.nrt_ruttr = ruttr;
                                //            remehain.nro_correl = 1;
                                //            remehain.cod_haber = codigo;
                                //            remehain.flg_afecta = "T";
                                //            remehain.cod_pago = "L";
                                //            remehain.flg_mtopo = "M";
                                //            remehain.val_haber = Convert.ToDecimal(monto);
                                //            remehain.cnt_dias = 1;
                                //            remehain.fec_desde = inicio;
                                //            remehain.fec_hasta = termino;
                                //            remehain.fec_ingre = hoydia;
                                //            remehain.num_pagin = Convert.ToInt16(codigo);
                                //            remehain.rut_empr = rutemp;

                                //            db.remehain.Add(remehain);
                                //            db.SaveChanges();
                                //        }
                                //        if (codigo > 99 && codigo < 1000)
                                //        {
                                //            var remedein = new remedein();

                                //            remedein.nrt_ruttr = ruttr;
                                //            remedein.nro_correl = 1;
                                //            remedein.cod_descu = codigo;
                                //            remedein.cod_haber = 0;
                                //            remedein.flg_afecta = "T";
                                //            remedein.cod_pago = "L";
                                //            remedein.flg_forde = "M";
                                //            remedein.val_descu = Convert.ToDecimal(monto);
                                //            remedein.fec_desde = inicio;
                                //            remedein.fec_hasta = termino;
                                //            remedein.fec_ingre = hoydia;
                                //            remedein.num_pagin = Convert.ToInt16(codigo);
                                //            remedein.rut_empr = rutemp;

                                //            db.remedein.Add(remedein);
                                //            db.SaveChanges();
                                //        }

                                //        iRow++;

                                //    }
                                //}
                            }
                        }
                    }
                }
            }

            return 1;
        }
        public int Traspasa_Maestros(string empresa,string per,string lab, string ins, string ban,string asi)
        {
            // Borra Informacion Anterior
            if(per == "S")
            {
            var res = db.remepers.Where(i => i.rut_empr == empresa).ToList();
            db.remepers.RemoveRange(res);
            db.SaveChanges();
            }
            if (lab == "S")
            {
                var rel = db.remelabo.Where(i => i.rut_empr == empresa).ToList();
            db.remelabo.RemoveRange(rel);
            db.SaveChanges();
            }
            if (ins == "S")
            {
                var rei = db.remeinst.Where(i => i.rut_empr == empresa).ToList();
            db.remeinst.RemoveRange(rei);
            db.SaveChanges();
            }
            if (ban == "S")
            {
                var reb = db.remebanc.Where(i => i.rut_empr == empresa).ToList();
            db.remebanc.RemoveRange(reb);
            db.SaveChanges();
            }
            if (asi == "S")
            {
                var tie = db.remeasis.Where(i => i.rut_empr == empresa).ToList();
            db.remeasis.RemoveRange(tie);
            db.SaveChanges();
            }



            DateTime hoy = DateTime.Now.Date;
            var infoContrato = new List<GYCEmpresa.Models.CONTRATO>();
            infoContrato = db.CONTRATO.Where(i => i.EMPRESA == empresa && i.FTERMNO >= hoy && i.FINICIO <= hoy
            && i.FIRMAEMPRESA == true && i.FIRMATRABAJADOR == true).ToList();

            foreach (var p in infoContrato)
            {
                var persona = (db.PERSONA.Where(x => x.RUT == p.PERSONA)).SingleOrDefault();
                var ciudad = db.CIUDAD.Where(x => x.ID == persona.CIUDAD).SingleOrDefault();
                var sindic = db.SINDICATOTRABAJADOR.Where(x => x.nrt_ruttr == p.PERSONA).ToList();
                string sind = "1";
                DateTime fecsin = p.FINICIO;
                if(sindic != null)
                {
                    foreach(var s in sindic)
                    {
                        sind = s.cod_sindi;
                        fecsin = (DateTime) s.fec_afili;
                    }
                }
                int afp = persona.PREVISION;
                int newafp = 0;
                if (afp == 1) newafp = 1033;
                if (afp == 2) newafp = 1003;
                if (afp == 3) newafp = 1005;
                if (afp == 4) newafp = 1034;
                if (afp == 5) newafp = 1028;
                if (afp == 6) newafp = 1008;
                if (afp == 8) newafp = 1035;

                int isa = persona.SALUD;
                int newisa = 0;
                if (isa == 1) newisa = 53;
                if (isa == 2) newisa = 71;
                if (isa == 3) newisa = 59;
                if (isa == 4) newisa = 52;
                if (isa == 5) newisa = 88;
                if (isa == 6) newisa = 80;


                remepers pers = new remepers();
                pers.nrt_ruttr = p.PERSONA;
                pers.dgv_ruttr = Convert.ToString(p.TCONTRATO);
                pers.nom_appat = persona.APATERNO;
                pers.nom_apmat = persona.AMATERNO;
                pers.nom_nomtr = persona.NOMBRE;
                pers.fec_nacim = persona.FNACIMIENTO.Date;
                pers.flg_sexo = persona.SEXO.Substring(0, 1);
                pers.flg_estci = Convert.ToString(persona.ECIVIL);
                int largo = persona.DIRECCION.Length;
                if (largo > 30) largo = 30;
                pers.gls_direcc = persona.DIRECCION.Substring(0,largo);
                pers.gls_ciudad = ciudad.DESCRIPCION;
                pers.rut_empr = empresa;
                if (per == "S")
                {
                    db.remepers.Add(pers);
                    db.SaveChanges();
                }
                remelabo labo = new remelabo();
                labo.nrt_ruttr = p.PERSONA;
                labo.fec_ingre = p.FINICIO.Date;
                labo.cod_roltr = "ADM";
                labo.cod_cenre = null;
                labo.fec_indem = p.FINICIO.Date;
                labo.cod_categ = null;
                labo.fec_categ = null;
                labo.mto_sueldo = p.MBRUTO;
                labo.cod_sindic = sind;
                labo.fec_sindic = fecsin;
                labo.cod_ine = null;
                labo.cod_corfo = null;
                largo = p.CARGO.Length;
                if (largo > 10) largo = 10;
                labo.cod_cargo = p.CARGO.Substring(0, largo);
                labo.fec_cargo = p.FINICIO.Date;
                labo.flg_tipcon = Convert.ToString(p.TCONTRATO);
                labo.cod_horar = "ADMIN";
                labo.flg_suple = "N";
                labo.rut_empr = empresa;
                if (lab == "S")
                {
                    db.remelabo.Add(labo);
                    db.SaveChanges();
                }
                remebanc banc = new remebanc();
                banc.nrt_ruttr = p.PERSONA;
                banc.cod_banco = Convert.ToString(persona.BANCO);
                banc.tip_cta = Convert.ToString(persona.TCUENTA);
                banc.nro_cta = persona.NCUENTA;
                banc.fec_inicio = DateTime.Now.Date;
                banc.fec_termin = DateTime.Now.Date;
                banc.rut_empr = empresa;
                if (ban == "S")
                {
                    db.remebanc.Add(banc);
                    db.SaveChanges();
                }
                remeinst inst = new remeinst();
                inst.nrt_rutr = p.PERSONA;
                inst.tip_instit = "AFP";
                inst.cod_instis = Convert.ToString(newafp);
                inst.nro_correl = 1;
                inst.val_aporte = 0;
                inst.fec_inicio = DateTime.Now.Date;
                inst.fec_termin = DateTime.Now.Date;
                inst.fec_ingreso = DateTime.Now.Date;
                inst.rut_empr = empresa;
                if (ins == "S")
                {
                    db.remeinst.Add(inst);
                    db.SaveChanges();
                } 
                remeinst inst1 = new remeinst();
                inst1.nrt_rutr = p.PERSONA;
                inst1.tip_instit = "ISAPRE";
                inst1.cod_instis = Convert.ToString(newisa);
                inst1.nro_correl = 1;
                inst1.val_aporte = 0;
                inst1.fec_inicio = DateTime.Now.Date;
                inst1.fec_termin = DateTime.Now.Date;
                inst1.fec_ingreso = DateTime.Now.Date;
                inst1.rut_empr = empresa;
                if (ins == "S")
                {
                    db.remeinst.Add(inst1);
                db.SaveChanges();
                }
                remepage para = db.remepage.Where(i => i.rut_empr == rutEmpresa && i.nom_tabla == "PAGO      " && i.cod_param == "1         ").SingleOrDefault();
                DateTime fecproc = (DateTime) para.fec_param;
                int mes, año, dia;
                String fechas;
                string fecmmaaaa = Convert.ToString(fecproc);
                mes = fecproc.Month;
                año = fecproc.Year;
                fechas = año + "-" + mes + "-" + "01";
                fecini = Convert.ToDateTime(fechas);
                if (mes == 1 || mes == 3 || mes == 5 || mes == 7 || mes == 8 || mes == 10 || mes == 12)
                    dia = 31;
                else
                    dia = 30;
                if (mes == 2) dia = 28;
                fechas = año + "-" + mes + "-" + dia;
                fecfin = Convert.ToDateTime(fechas);
                var infoferia = new List<GYCEmpresa.Models.remepage>();
                infoferia = db.remepage.Where(x => x.nom_tabla == "FERIADOS" && x.fec_param >= fecini && x.fec_param <= fecfin).ToList();
                remepage dettiemp = new remepage();
                int holguraent;
                var infoTiemp = new List<GYCEmpresa.Models.remepage>();
                infoTiemp = db.remepage.Where(x => x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).ToList();
                dettiemp = infoTiemp.Where(x => "1         " == x.cod_param).SingleOrDefault();
                holguraent = 0;
                if (dettiemp != null)
                    holguraent = Convert.ToInt32(dettiemp.val_param);
                var ndias = new List<DetalleTiempo>();

                ndias = AsistenciaDiaria(p.PERSONA, fecini, fecfin, p.FAENA, infoferia, holguraent);

                foreach(var t in ndias)
                {
                    if (t.M_EXTRA <= 0)
                    { t.M_EXTRA = 0; }
                    else
                    {
                        int hh = t.M_EXTRA / 60;
                        int mm = t.M_EXTRA - hh * 60;
                        mm = (mm * 100) / 60;
                        t.M_EXTRA = hh + mm;
                    }
                    var remeasis = new remeasis();
                    remeasis.nrt_ruttr = t.RUT;
                    remeasis.fec_asist = t.FECHA;
                    remeasis.cod_inasi = t.C_INASI;
                    remeasis.cnt_inasi = 0;
                    if (t.C_INASI == null || t.C_INASI == "LICENCIA" || t.C_INASI == "PERMISO" ||  t.C_INASI == "") remeasis.cnt_inasi = 1;
                    remeasis.hrs_sob1 = t.M_EXTRA;
                    remeasis.hrs_sob2 = 0;
                    remeasis.hrs_sob3 = 0;
                    remeasis.dia_colac = 0;
                    remeasis.hrs_colac = 0;
                    remeasis.dia_movil = 0;
                    remeasis.rut_empr = p.EMPRESA;
                    if (asi == "S")
                    {
                        db.remeasis.Add(remeasis);
                        db.SaveChanges();
                    }
                }



            }
            return 1;
        }
        public List<DetalleTiempo> AsistenciaDiaria(String RUT, DateTime finicio, DateTime ftermino, int faena, List<GYCEmpresa.Models.remepage> infoFeria, int holguraent)
        {
            int Descanso = 0;
            int Trabajados = 0;
            int Permiso = 0;
            int Vacaciones = 0;
            int Licencia = 0;
            int Feriado;

            DateTime fecloop;
            int ind, diasem, extras, tardas;
            DateTime salpro, salrea;
            string rut;
            DateTime entrada, salida;
            string modent, modsal;
            string t_entrada, t_salida, codina;
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;

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
            var nrdia = new List<DetalleTiempo>();

            rut = RUT;
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
            detContr = db.CONTRATO.Where(x => x.PERSONA == rut && x.EMPRESA == empresa && x.FTERMNO > finicio && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false).SingleOrDefault();
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
                if (fecloop >= detContr.FINICIO && fecloop <= detContr.FTERMNO)
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
                            {
                                entrada = m.CHECKTIME; if (Convert.ToBoolean(m.MODIFICADA)) { modent = "*"; }
                                TotTar = (TimeSpan)turdet.HoraInicio;
                                int mintur = TotTar.Minutes + (TotTar.Hours) * 60;
                                if (mintur > 0)
                                {

                                    int minent = m.CHECKTIME.Hour * 60 + m.CHECKTIME.Minute;
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
                            extras = (salrea.Subtract(salpro).Hours) * 60 + salrea.Subtract(salpro).Minutes;
                        }
                    }

                    nrdia.Add(new DetalleTiempo()
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
                        M_PERMI = 0
                    });
                }
                fecloop = fecloop.AddDays(1);
            }
            return nrdia;
        }

    }
}
namespace GYCEmpresa.Models
{
    public class DetalleTiempo
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
