using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Common.CommandTrees;

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
        int mto_apvc = 0;

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
        int mto_impsal = 0;
        int mto_plasal = 0;
        int cot_pacsal = 0;
        int cot_oblsal = 0;
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
            DateTime fecpro = Convert.ToDateTime("2020-11-01");
            string mmaaaa = "112020";
            var trabajadores = from m in db.remepers
                               select m;
            if (!String.IsNullOrEmpty(rutEmpresa))
            {
                trabajadores = trabajadores.Where(s => s.rut_empr.Contains(rutEmpresa));
            }

            //RECORRER LISTA DE TRABAJADORES Y LLAMAR calculo 
            //TextWriter Escribir = new StreamWriter(pathFile);
            int ind;
            string inds;
            for (ind = 1; ind <= 110; ind++)
            {
                inds = Convert.ToString(ind);
                dt.Columns.Add(inds, typeof(string));
            }

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

                dt.Rows.Add(t.nrt_ruttr, t.dgv_ruttr, t.nom_appat, t.nom_apmat, t.nom_nomtr, t.flg_sexo, "0", "01", mmaaaa, fecpro, "AFP"
                         , dias_trab, tip_trab, tip_linea, cod_mov, fec_inicio, fec_termin, cod_tramo, cnt_simple, mto_asig, nro_mater
                         , nro_inval, mto_retro, mto_reint, cod_joven, cod_afp, mto_impon1, mto_pension, mto_seguro, mto_cta2, rta_sustit
                         , por_pact, apo_indem, nro_peri, fec_desde, fec_hasta, gls_trape, por_trape, mto_trape, ins_apvi, con_apvi, fla_apvi
                         , mto_apvi, mto_conv, ins_apvc, con_apvc, fla_apvc, mto_apvc, rut_afvol, dgv_afvol, app_afvol, apm_afvol, nom_afvol
                         , cod_movav, fec_desav, fec_hasav, cod_afpav, mto_penav, mto_ahvav, nro_perav, cod_cajips, tas_cotips, mto_impips
                         , mto_oblips, mto_desips, cod_regips, tas_desips, cot_desips, cot_fonips, cot_accips, bon_leyips, des_carips, bon_gobips
                         , cod_inssal, nro_funsal, mto_impsal, mto_plasal, cot_pacsal, cot_oblsal, cot_volsal, mto_gessal, cod_cajcc, ren_impcc
                         , cre_percc, des_dencc, des_lescc, des_segcc, des_otrcc, cot_niscc, des_famcc, des_ot1cc, des_ot2cc, bon_gobcc, cod_succc
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

                if (i.cod_conce == 1)
                {
                    dias_trab = (int)i.cnt_conce;
                }
                if (i.cod_conce == 14)
                {
                    if (i.correl == 1) cod_tramo = "A";
                    if (i.correl == 2) cod_tramo = "B";
                    if (i.correl == 3) cod_tramo = "C";
                    cnt_simple = (int)i.cnt_conce;
                    mto_asig = (int)i.mto_pagar;
                }
                if (i.cod_conce == 2011)
                {
                    mto_impon1 = (int)i.mto_pagar;
                    mto_impips = mto_impon1;
                    mto_impsal = mto_impon1;
                }
                if (i.cod_conce == 910)
                {
                    mto_pension = (int)i.mto_pagar;
                    cod_afp = Convert.ToString(i.cnt_conce);
                }
                if (i.cod_conce == 2401)
                {
                    mto_seguro = (int)i.mto_pagar;
                }
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

                if (i.cod_conce == 921)
                {
                    mto_plasal = (int)i.mto_pagar;
                    cod_inssal = (int)i.cnt_conce;
                }
                if (i.cod_conce == 2402)
                {
                    ren_impmut = (int)i.mto_pagar;
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
            linea04 = ins_apvc + ";" + con_apvc + ";" + fla_apvc + ";" + mto_apvc;

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
                                else
                                {
                                    // Otro Formato

                                    string mes;
                                    string anio;
                                    String monto;
                                    DateTime inicio;
                                    DateTime termino;
                                    DateTime hoydia;
                                    string fec;
                                    int codigo;
                                    while (!string.IsNullOrEmpty(oSLDocument.GetCellValueAsString(iRow, 1)))
                                    {

                                        mes = oSLDocument.GetCellValueAsString(iRow, 1);
                                        anio = oSLDocument.GetCellValueAsString(iRow, 2);
                                        fec = "01-" + mes + "-" + anio;
                                        DateTime.TryParse(fec, out inicio);
                                        fec = "28-" + mes + "-" + anio;
                                        DateTime.TryParse(fec, out termino);
                                        hoydia = DateTime.Today;
                                        string[] ruti = oSLDocument.GetCellValueAsString(iRow, 3).Split('-');
                                        ruttr = ruti[0];
                                        codigo = oSLDocument.GetCellValueAsInt32(iRow, 4);

                                        monto = oSLDocument.GetCellValueAsString(iRow, 5);
                                        empre = rutemp;

                                        if (codigo == 1000)
                                        {

                                            var remeasis = new remeasis();
                                            remeasis.nrt_ruttr = ruttr;
                                            remeasis.fec_asist = inicio;
                                            remeasis.cnt_inasi = Convert.ToInt32(monto);
                                            remeasis.hrs_sob1 = 0;
                                            remeasis.hrs_sob2 = 0;
                                            remeasis.hrs_sob3 = 0;
                                            remeasis.dia_colac = 0;
                                            remeasis.hrs_colac = 0;
                                            remeasis.dia_movil = 0;
                                            remeasis.rut_empr = rutemp;

                                            db.remeasis.Add(remeasis);
                                            db.SaveChanges();
                                        }
                                        if (codigo == 1001)
                                        {

                                            var remeasis = new remeasis();
                                            remeasis.nrt_ruttr = ruttr;
                                            remeasis.fec_asist = inicio;
                                            remeasis.hrs_sob1 = Convert.ToDecimal(monto) / 100;
                                            remeasis.cnt_inasi = 0;
                                            remeasis.hrs_sob2 = 0;
                                            remeasis.hrs_sob3 = 0;
                                            remeasis.dia_colac = 0;
                                            remeasis.hrs_colac = 0;
                                            remeasis.dia_movil = 0;
                                            remeasis.rut_empr = rutemp;

                                            db.remeasis.Add(remeasis);
                                            db.SaveChanges();
                                        }
                                        if (codigo > 0 && codigo < 100)
                                        {
                                            var remehain = new remehain();

                                            remehain.nrt_ruttr = ruttr;
                                            remehain.nro_correl = 1;
                                            remehain.cod_haber = codigo;
                                            remehain.flg_afecta = "T";
                                            remehain.cod_pago = "L";
                                            remehain.flg_mtopo = "M";
                                            remehain.val_haber = Convert.ToDecimal(monto);
                                            remehain.cnt_dias = 1;
                                            remehain.fec_desde = inicio;
                                            remehain.fec_hasta = termino;
                                            remehain.fec_ingre = hoydia;
                                            remehain.num_pagin = Convert.ToInt16(codigo);
                                            remehain.rut_empr = rutemp;

                                            db.remehain.Add(remehain);
                                            db.SaveChanges();
                                        }
                                        if (codigo > 99 && codigo < 1000)
                                        {
                                            var remedein = new remedein();

                                            remedein.nrt_ruttr = ruttr;
                                            remedein.nro_correl = 1;
                                            remedein.cod_descu = codigo;
                                            remedein.cod_haber = 0;
                                            remedein.flg_afecta = "T";
                                            remedein.cod_pago = "L";
                                            remedein.flg_forde = "M";
                                            remedein.val_descu = Convert.ToDecimal(monto);
                                            remedein.fec_desde = inicio;
                                            remedein.fec_hasta = termino;
                                            remedein.fec_ingre = hoydia;
                                            remedein.num_pagin = Convert.ToInt16(codigo);
                                            remedein.rut_empr = rutemp;

                                            db.remedein.Add(remedein);
                                            db.SaveChanges();
                                        }

                                        iRow++;

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return 1;
        }
        public int Traspasa_Maestros(string empresa)
        {
            // Borra Informacion Anterior
            var res = db.remepers.Where(i => i.rut_empr == empresa).ToList();
            db.remepers.RemoveRange(res);
            db.SaveChanges();
            var rel = db.remelabo.Where(i => i.rut_empr == empresa).ToList();
            db.remelabo.RemoveRange(rel);
            db.SaveChanges();



            DateTime hoy = DateTime.Now.Date;
            var infoContrato = new List<GYCEmpresa.Models.CONTRATO>();
            infoContrato = db.CONTRATO.Where(i => i.EMPRESA == empresa && i.FTERMNO >= hoy && i.FINICIO <= hoy
            && i.FIRMAEMPRESA == true && i.FIRMATRABAJADOR == true).ToList();

            foreach (var p in infoContrato)
            {
                var persona = (db.PERSONA.Where(x => x.RUT == p.PERSONA)).SingleOrDefault();
                var ciudad = db.CIUDAD.Where(x => x.ID == persona.CIUDAD).SingleOrDefault();

                remepers pers = new remepers();
                pers.nrt_ruttr = p.PERSONA;
                pers.dgv_ruttr = Convert.ToString(p.TCONTRATO);
                pers.nom_appat = persona.APATERNO;
                pers.nom_apmat = persona.AMATERNO;
                pers.nom_nomtr = persona.NOMBRE;
                pers.fec_nacim = persona.FNACIMIENTO.Date;
                pers.flg_sexo = persona.SEXO.Substring(0, 1);
                pers.flg_estci = Convert.ToString(persona.ECIVIL);
                pers.gls_direcc = persona.DIRECCION;
                pers.gls_ciudad = ciudad.DESCRIPCION;
                pers.rut_empr = empresa;
                db.remepers.Add(pers);
                db.SaveChanges();

                remelabo labo = new remelabo();
                labo.nrt_ruttr = p.PERSONA;
                labo.fec_ingre = p.FINICIO.Date;
                labo.cod_roltr = "ADM";
                labo.cod_cenre = null;
                labo.fec_indem = p.FINICIO.Date;
                labo.cod_categ = null;
                labo.fec_categ = null;
                labo.mto_sueldo = p.MBRUTO;
                labo.cod_sindic = null;
                labo.fec_sindic = null;
                labo.cod_ine = null;
                labo.cod_corfo = null;
                labo.cod_cargo = p.CARGO.Substring(0, 10);
                labo.fec_cargo = p.FINICIO.Date;
                labo.flg_tipcon = Convert.ToString(p.TCONTRATO);
                labo.cod_horar = "ADMIN";
                labo.flg_suple = "N";
                labo.rut_empr = empresa;
                db.remelabo.Add(labo);
                db.SaveChanges();


            }
            return 1;
        }

    }
}