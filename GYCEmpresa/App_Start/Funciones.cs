using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using GYCEmpresa.Controllers;
using GYCEmpresa.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GYCEmpresa.App_Start
{

    public class Funciones
    {
        dbgycEntities3 db = new dbgycEntities3();

        public int Edad(DateTime fecha)
        {
            int edad = 0;
            DateTime hoy = DateTime.Now.Date;
            if (fecha >= hoy) return edad;
            edad = hoy.Year - fecha.Year;
            if (hoy < fecha.AddYears(edad)) return --edad;
            return edad;

        }
        public DateTime PrimerDia(DateTime fecha)
        {
            DateTime primero = Convert.ToDateTime("1900-01-01");
            if (fecha == null) return primero;
            string fecst;
            int dia, mes, año;
            mes = fecha.Month;
            año = fecha.Year;
            dia = 01;
            fecst = año + "-" + mes + "-" + dia;
            primero = Convert.ToDateTime(fecst);
            return primero;

        }
        public DateTime UltimoDia(DateTime fecha)
        {
            DateTime ultimo = Convert.ToDateTime("1900-01-01");
            if (fecha == null) return ultimo;
            string fecst;
            int dia, mes, año;
            mes = fecha.Month;
            año = fecha.Year;
            if (mes == 1 || mes == 3 || mes == 5 || mes == 7 || mes == 8 || mes == 10 || mes == 12)
                dia = 31;
            else
                dia = 30;
            if (mes == 2) dia = 28;
            fecst = año + "-" + mes + "-" + dia;
            ultimo = Convert.ToDateTime(fecst);
            return ultimo;

        }
        public SelectList Meses()
        {
            var nr = new List<ListaMeses>();
            nr.Add(new ListaMeses() { mes = 1, nombre = "Enero" });
            nr.Add(new ListaMeses() { mes = 2, nombre = "Febrero" });
            nr.Add(new ListaMeses() { mes = 3, nombre = "Marzo" });
            nr.Add(new ListaMeses() { mes = 4, nombre = "Abril" });
            nr.Add(new ListaMeses() { mes = 5, nombre = "Mayo" });
            nr.Add(new ListaMeses() { mes = 6, nombre = "Junio" });
            nr.Add(new ListaMeses() { mes = 7, nombre = "Julio" });
            nr.Add(new ListaMeses() { mes = 8, nombre = "Agosto" });
            nr.Add(new ListaMeses() { mes = 9, nombre = "Septiembre" });
            nr.Add(new ListaMeses() { mes = 10, nombre = "Octubre" });
            nr.Add(new ListaMeses() { mes = 11, nombre = "Noviembre" });
            nr.Add(new ListaMeses() { mes = 12, nombre = "Diciembre" });

            var MesesSL = new SelectList(nr, "Mes", "Nombre");

            return MesesSL;

        }
        public SelectList Años()
        {
            var nr = new List<Listaanos>();
            nr.Add(new Listaanos() { ano = 2020, nombre = "2020" });
            nr.Add(new Listaanos() { ano = 2021, nombre = "2021" });
            nr.Add(new Listaanos() { ano = 2022, nombre = "2022" });
            nr.Add(new Listaanos() { ano = 2023, nombre = "2023" });
            nr.Add(new Listaanos() { ano = 2024, nombre = "2024" });
            nr.Add(new Listaanos() { ano = 2025, nombre = "2025" });
            nr.Add(new Listaanos() { ano = 2026, nombre = "2026" });
            nr.Add(new Listaanos() { ano = 2027, nombre = "2027" });
            nr.Add(new Listaanos() { ano = 2028, nombre = "2028" });
            nr.Add(new Listaanos() { ano = 2029, nombre = "2029" });
            nr.Add(new Listaanos() { ano = 2030, nombre = "2030" });
            nr.Add(new Listaanos() { ano = 2031, nombre = "2031" });

            var AñosSL = new SelectList(nr, "Ano", "Nombre");
            return AñosSL;
        }

        public string Diasem(DateTime fecha)
        {
            string diatexto;
            int dia = (int)fecha.DayOfWeek;
            diatexto = "Domingo";
            if (dia == 1) diatexto = "Lunes";
            if (dia == 2) diatexto = "Martes";
            if (dia == 3) diatexto = "Miercoles";
            if (dia == 4) diatexto = "Jueves";
            if (dia == 5) diatexto = "Viernes";
            if (dia == 6) diatexto = "Sabado";
            return diatexto;
        }
        public string Difhoras(DateTime fecha1,DateTime fecha2)
        {
            string diatexto;
            int difmin = fecha2.Subtract(fecha1).Minutes;
            int difhrs = fecha2.Subtract(fecha1).Hours;
            diatexto = difhrs + ":" + difmin;
            if (fecha1 > fecha2) diatexto = "";
            return diatexto;
        }
        public int Dias_Proporcionales(DateTime ingreso, int dias)
        {
            int dprop;
            int diai = ingreso.Day;
            int mesi = ingreso.Month;
            DateTime hoy = DateTime.Now;
            int año = hoy.Year;
            string fecs = año + "-" + mesi + "-" + diai;
            DateTime ultimo = Convert.ToDateTime(fecs);
            if(hoy < ultimo)
            {
                fecs = año-1 + "-" + mesi + "-" + diai;
                ultimo = Convert.ToDateTime(fecs);
            }
            int tdia = (hoy - ultimo).Days;
            dprop = (tdia * dias) / 365;
           
            return dprop;
        }
        public string DiaOcupado(string ruttra, DateTime fechai, DateTime fechaf)
        {
            string ocupa;
            var esta = db.rhuesolv.Where(x => x.nrt_ruttr == ruttra && ((fechai >= x.fec_inicio && fechai <= x.fec_termin) || (fechaf >= x.fec_inicio && fechaf <= x.fec_termin))).SingleOrDefault();
            if (esta == null) ocupa = "N";
            else ocupa = "S";
            return ocupa;
        }

        public SelectList FaenasEmpresa()
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;
            var Faenas = db.FAENA.Where(x => x.EMPRESA == empresa ).OrderBy(x => x.DESCRIPCION).Select(x => new { Id = x.ID, Descripcion = x.DESCRIPCION }).ToList();

            var FaenaSL = new SelectList(Faenas, "Id", "Descripcion");


            return FaenaSL;

        }
        public SelectList FaenasEmpresaTodos(int vigente)
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
                infofaena = db.FAENA.Where(i => i.EMPRESA == empresa && i.FTERMINO > hoy).OrderBy(x => x.DESCRIPCION).ToList();
            }

            var nr = new List<ListaFaenas>();
            nr.Add(new ListaFaenas() { Id = 0, Descripcion = "TODOS" });
            foreach (var i in infofaena)
            {
                nr.Add(new ListaFaenas() { Id = i.ID, Descripcion = i.DESCRIPCION });
            }
            var FaenaSL = new SelectList(nr, "Id", "Descripcion");
            return FaenaSL;

        }
        public SelectList Turnos_General()
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;
            var Turnos = db.Turno.OrderBy(x => x.Descripcion).Select(x => new { Id = x.IdTurno, Descripcion = x.Descripcion }).ToList();

            var TurnoSL = new SelectList(Turnos, "Id", "Descripcion");


            return TurnoSL;

        }



        public void GenerarNotificacion(string tipo, string obs, bool sendToGYC, string tabla, int idTabla, bool haciaEmpresa, bool haciaTrabajador, string Trabajador)
        {
            NOTIFICACION notif = new NOTIFICACION();
            notif.TIPO = tipo;
            if (obs.Length > 150)
            {
                obs = obs.Substring(0, 150).ToString();
            }
            notif.OBSERVACION = obs;
            notif.GYC = true;
            notif.EMPRESA = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            notif.USUARIO = System.Web.HttpContext.Current.Session["sessionUsuario"].ToString();
            notif.TABLA = tabla;
            notif.IDTABLA = idTabla;
            notif.NOTIFEMPRESA = haciaEmpresa;
            notif.NOTIFTRABAJADOR = haciaTrabajador;
            notif.ESTADO = 0;
            notif.VISTO = false;
            notif.TRABAJADOR = Trabajador;
            notif.FECHA = DateTime.Now.AddHours(AjusteHora());

            db.NOTIFICACION.Add(notif);
            db.SaveChanges();

        }

        public void EliminaNotificacion(string Tabla, int idTabla)
        {
            var NotificacionVista = (from notif in db.NOTIFICACION
                                     where notif.TABLA == Tabla && notif.IDTABLA == idTabla && notif.NOTIFEMPRESA == true
                                     select notif).ToList(); ;
            foreach (var notif in NotificacionVista)
            {
                notif.VISTO = true;
                db.SaveChanges();
            }
        }

        public bool isLogged()
        {
            if (System.Web.HttpContext.Current.Session["sessionUsuario"] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public int AjusteHora()
        {
            int Ajuste = -3;
            return Ajuste;
        }

        public bool PermisoPorServicio(int idServicio)
        {

            IList<int> ServContratado = (IList<int>)System.Web.HttpContext.Current.Session["serviciosEmpresa"];

            int ExisteServicio = ServContratado.Where(x => x.Equals(idServicio)).Count();

            //var ServContratado = (db.SERVICIOCONTRATADO.Where(x => x.SERVICIO == idServicio && x.EMPRESA == empresa)).ToList();

            if (ExisteServicio > 0)
            {
                IList<PERMISO_USUARIO_Result> Permisos = (IList<PERMISO_USUARIO_Result>)System.Web.HttpContext.Current.Session["permisosUsuario"];

                ExisteServicio = Permisos.Where(x => x.ID_SERVICIO == idServicio && x.NIVEL_PERMISO != 99999).Count();
                if (ExisteServicio > 0)
                {
                    return true;
                }
            }

            return false;

        }

        public string FirmaDocumento(DateTime fechaFirma, string firmaHash, int iD, string Tabla, string NombreTrabajador, string Rut)
        {
            using (dbgyc2DataContext db2 = new dbgyc2DataContext())
            {
                string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

                var PDF = db2.GET_DOCUMENTO(Tabla.ToUpper(), iD).First();

                Random rnd = new Random();
                string Random = rnd.Next(1000, 9999).ToString();

                string Ruta = AppDomain.CurrentDomain.BaseDirectory;
                string Directorio = Ruta + "/temp/";
                string RutacompletaOriginal = Directorio + "_" + Tabla + "_" + Random + ".pdf";
                string RutaCompletaNueva = Directorio + "_" + Tabla + "_" + Random + "_Firmado.pdf";
                byte[] file_byte_Nuevo = null;

                if (!Directory.Exists(Directorio))
                {
                    Directory.CreateDirectory(Directorio);
                }
                if (File.Exists(RutacompletaOriginal))
                {
                    Directory.Delete(RutacompletaOriginal);
                }

                if (PDF.PDF != null)
                {
                    byte[] file_byte = null;

                    MemoryStream ms = new MemoryStream();
                    ms.Write(PDF.PDF.ToArray(), 0, PDF.PDF.Length);
                    file_byte = ms.ToArray();

                    File.WriteAllBytes(RutacompletaOriginal, file_byte);
                    //SAUTINSOFT---------------

                    //DocumentCore dc = DocumentCore.Load(RutacompletaOriginal);

                    ////FIRMA EMPRESA
                    //ContentRange cr = dc.Content.Find("FIRMA_EMPRESA").FirstOrDefault();

                    ////FIRMA TRABAJADOR
                    ////ContentRange cr = dc.Content.Find("FIRMA_TRABAJADOR").FirstOrDefault();

                    //if (cr!=null)
                    //{
                    //    cr.Replace(firmaHash);
                    //}
                    //dc.Save(RutaCompletaNueva);

                    //ITEXTSHARP ----------------


                    utilPdf util = new utilPdf();
                    //string cErr = util.addTexto(RutaCompletaNueva, RutacompletaOriginal, firmaHash.Trim(), 150, 150, 0, 90);

                    string RutFirma = AgregaGuionRut(Rut);
                    fechaFirma = fechaFirma.AddHours(AjusteHora());

                    util.addLogo(RutaCompletaNueva, RutacompletaOriginal, firmaHash.Trim(), NombreTrabajador, RutFirma, fechaFirma);



                    Stream myStream = File.Open(RutaCompletaNueva, FileMode.Open);

                    using (MemoryStream ms_N = new MemoryStream())
                    {
                        myStream.CopyTo(ms_N);
                        file_byte_Nuevo = ms_N.ToArray();
                        ms.Close();
                        ms_N.Close();
                    }
                    myStream.Close();


                    //string error = "null";
                    //return error;
                }
   

                var DocFirmado = db.HASH_DOCUMENTOS.Where(x => x.TABLA_DOCUMENTO == Tabla && x.ID_DOCUMENTO == iD).FirstOrDefault();

                if (DocFirmado == null)
                {
                    HASH_DOCUMENTOS Nuevo = new HASH_DOCUMENTOS();

                    Nuevo.CLAVE = Tabla.Substring(0, 4) + iD;
                    Nuevo.FECHA_FIRMA_EMPRESA = DateTime.Now.AddHours(AjusteHora());
                    Nuevo.HASH_FIRMA_EMPRESA = firmaHash;
                    Nuevo.TABLA_DOCUMENTO = Tabla;
                    Nuevo.ID_DOCUMENTO = iD;
                    Nuevo.USUARIO_FIRMA_EMPRESA = usuario;

                    db.HASH_DOCUMENTOS.Add(Nuevo);
                }
                else
                {
                    DocFirmado.HASH_FIRMA_EMPRESA = firmaHash;
                    DocFirmado.FECHA_FIRMA_EMPRESA = DateTime.Now.AddHours(AjusteHora());
                    DocFirmado.USUARIO_FIRMA_EMPRESA = usuario;
                }

                db.SaveChanges();

                db2.GUARDA_DOCUMENTO(Tabla, iD, file_byte_Nuevo);
                return RutaCompletaNueva;




            }
        }

        internal string AgregaGuionRut(string rUTTRABAJADOR)
        {
            string rut = rUTTRABAJADOR.Substring(0, rUTTRABAJADOR.Length - 1);
            string verif = rUTTRABAJADOR.Substring(rUTTRABAJADOR.Length - 1, 1);

            return rut + "-" + verif;
        }

        public void CierraDocumentoPDF(string Archivo, int Id, string Tabla)
        {
            using (dbgyc2DataContext db2 = new dbgyc2DataContext())
            {
                Random rnd = new Random();
                string Random = rnd.Next(1000, 9999).ToString();

                string Ruta = AppDomain.CurrentDomain.BaseDirectory;
                string Directorio = Ruta + "/temp/";
                string RutaCompletaNueva = Directorio + "_QR" + "_" + Random + "_Firmado.pdf";

                var HashDocumento = db2.GENERA_HASH(Id, Tabla.ToUpper().Trim()).First();

                string txtQRCode = HashDocumento.Column1.ToString();
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                //System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                //imgBarCode.Height = 150;
                //imgBarCode.Width = 150;
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        var imageBytes = ms.ToArray();
                        utilPdf utilPdf = new utilPdf();
                        utilPdf.addQR(RutaCompletaNueva, Archivo, txtQRCode);
                        //imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                    }
                }

                byte[] file_byte_Nuevo = null;

                Stream myStream = File.Open(RutaCompletaNueva, FileMode.Open);

                using (MemoryStream ms_N = new MemoryStream())
                {
                    myStream.CopyTo(ms_N);
                    file_byte_Nuevo = ms_N.ToArray();
                    ms_N.Close();
                }
                myStream.Close();


                var HashDoc = db2.GENERA_HASH(Id, Tabla).First();

                var DocFirmado = db.HASH_DOCUMENTOS.Where(x => x.TABLA_DOCUMENTO == Tabla || x.ID_DOCUMENTO == Id).FirstOrDefault();

                DocFirmado.FECHA_FINAL = DateTime.Now.AddHours(AjusteHora());
                DocFirmado.HASH_DOCUMENTO_FINAL = HashDoc.Column1;

                db2.GUARDA_DOCUMENTO(Tabla, Id, file_byte_Nuevo);
            }
        }

        public void LogAcceso(string Informacion)
        {

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"].ToString();

            LOG_ACCESO Nuevo = new LOG_ACCESO();
            Nuevo.FECHA = DateTime.Now;
            Nuevo.USUARIO = usuario;
            Nuevo.DESCRIPCION = "W.Empresa#" + Informacion;

            using (dbgycEntities3 db = new dbgycEntities3())
            {
                db.LOG_ACCESO.Add(Nuevo);
                db.SaveChanges();
            }
        }

        public List<DateTime> FechasLimiteFaena(DateTime FINICIO, DateTime FTERMINO, int FAENA)
        {
            List<DateTime> FECHAS = new List<DateTime>();
            var FechasFaena = db.FAENA.Where(x => x.ID == FAENA).Select(x => new { x.FINICIO, x.FTERMINO }).SingleOrDefault();
            if (FechasFaena != null)
            {
                if (FINICIO > FechasFaena.FINICIO)
                {
                    FECHAS.Add(FINICIO);
                }
                else
                {
                    FECHAS.Add(FechasFaena.FINICIO);
                }

                if (FTERMINO < FechasFaena.FTERMINO)
                {
                    FECHAS.Add(FTERMINO);
                }
                else
                {
                    if (FechasFaena.FTERMINO != null)
                    {
                        DateTime ftermino = FechasFaena.FTERMINO ?? FTERMINO;
                        FECHAS.Add(ftermino);
                    }
                    else
                    {
                        FECHAS.Add(FTERMINO);
                    }
                }
            }
            return FECHAS;


        }

        public bool CargaArchivoS3(string nombreBucket, int Directorio, string NombreArchivo, byte[] Archivo)
        {

            try
            {
                string Nombredirectorio = string.Empty;
                using (dbgycEntities3 db = new dbgycEntities3())
                {
                    Nombredirectorio = db.TIPO_BUCKET.Where(x => x.ID == Directorio).Select(x => x.DESCRIPCION).FirstOrDefault();
                }


                string bucketName = nombreBucket;
                string keyName = Nombredirectorio + "/" + NombreArchivo;
                string AccessKey = "AKIA32LK5L3Q6ETKJ2FT";
                string SecretKey = "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";

                RegionEndpoint bucketRegion = RegionEndpoint.SAEast1;
                IAmazonS3 s3Client = new AmazonS3Client(AccessKey, SecretKey, bucketRegion);

                string root = AppDomain.CurrentDomain.BaseDirectory + "/temp/";

                //Verifica si existe Bucket, sino lo crea
                if (!(AmazonS3Util.DoesS3BucketExistV2(s3Client, bucketName)))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    PutBucketResponse putBucketResponse = s3Client.PutBucket(putBucketRequest);
                }


                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }

                var fileTransferUtility = new TransferUtility(s3Client);
                string filePath = root + NombreArchivo;
                byte[] file_byte = null;

                MemoryStream ms = new MemoryStream();
                ms.Write(Archivo.ToArray(), 0, Archivo.Length);
                file_byte = ms.ToArray();

                System.IO.File.WriteAllBytes(filePath, Archivo);

                fileTransferUtility.UploadAsync(filePath, bucketName, keyName);

                return true;

            }
            catch (AmazonS3Exception e)
            {
                string Mensaje = e.Message;
                return false;
                //oR.message = "Error encountered on server. Message:'{0}' when writing an object # " + e.Message;
                //oR.result = 0;
            }
            catch (Exception e)
            {
                string Mensaje = e.Message;
                return false;
                //oR.message = e.Message;
                //oR.result = 0;
            }
        }

        public string DescargaArchivoS3(string bucketName, string Directorio, string Documento)
        {

            string AccessKey = "AKIA32LK5L3Q6ETKJ2FT";
            string SecretKey = "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";


            RegionEndpoint bucketRegion = RegionEndpoint.SAEast1;
            IAmazonS3 s3Client = new AmazonS3Client(AccessKey, SecretKey, bucketRegion);

            //string bucketName = bucketName;
            string keyName = Directorio + "/" + Documento;

            GetPreSignedUrlRequest solicitud = new GetPreSignedUrlRequest
            {
                BucketName = bucketName.ToLower(),
                Key = keyName,
                Expires = DateTime.Now.AddMinutes(2)
            };

            string URL = s3Client.GetPreSignedURL(solicitud);
            return URL;
        }

        public bool EliminaArchivoS3(string bucketName, string Directorio, string Documento)
        {
            try
            {
                string AccessKey = "AKIA32LK5L3Q6ETKJ2FT";
                string SecretKey = "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";


                RegionEndpoint bucketRegion = RegionEndpoint.SAEast1;
                IAmazonS3 s3Client = new AmazonS3Client(AccessKey, SecretKey, bucketRegion);

                //string bucketName = bucketName;
                string keyName = Directorio + "/" + Documento;

                DeleteObjectRequest ArchivoEliminar = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                DeleteObjectResponse response = s3Client.DeleteObject(ArchivoEliminar);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }



        public Tuple<byte[], string> DescargaArchivoPorID(int ID)
        {
            try
            {
                //byte[] bytesArchivo;



                var Archivo = db.DOCUMENTOS_CLOUD.Where(x => x.ID == ID).Select(x => new { x.DESCRIPCION, x.TRABAJADOR, x.S3_BUCKET, x.S3_DIRECTORIO, x.S3_DOCUEMNTO, x.EXTENSION, x.ARCHIVO }).FirstOrDefault();
                //bytesArchivo = Archivo.DOC;
                //return File(bytesArchivo, System.Net.Mime.MediaTypeNames.Application.Octet, Archivo.ARCHIVO);
                string URL = DescargaArchivoS3(Archivo.S3_BUCKET, Archivo.S3_DIRECTORIO, Archivo.S3_DOCUEMNTO);

                string ext = Path.GetExtension(Archivo.ARCHIVO).ToUpper();


                System.Net.WebClient client = new System.Net.WebClient();
                var bytearr = client.DownloadData(URL);


                return Tuple.Create(bytearr, ext);

            }
            catch (Exception e2)
            {
                string Mensaje = e2.Message;
                return null;
            }
        }



        public void SubirArchivoCloud(HttpPostedFileBase ARCHIVO, int TIPO_BUCKET, DateTime FINICIO, DateTime FTERMINO, string RUT, string NOTIFICA, string DESCRIPCION, string MAIL_NOTIFICA, int DIAS_NOTIFICACION1, int DIAS_NOTIFICACION2)
        {
            /*

            BUCKETS
            1 - CONTRATO
            2 - ANEXO
            3 - EXMEDICO
            4 - DAS
            5 - CV
            6 - EPP
            7 - FICHA
            8 - RIOHS
            9 - AFP
            10 - SALUD
            11 - ODI
            12 - TITULO
            13 - OTRO
            14 - LIQUIDACION
            16 - DOCUMENTO ESPECIAL
            17 - VACACIONES

            */

            Random rnd = new Random();
            string Random = rnd.Next(1, 999999).ToString();

            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
            string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

            PERSONA p = db.PERSONA.Where(x => x.RUT == RUT).FirstOrDefault();
            string TRABAJADOR = p.NOMBRE + " " + p.APATERNO + " " + p.AMATERNO;
            string NombreArchivoNuevo = "_" + TRABAJADOR + "_" + empresa + "_" + Random + Path.GetExtension(ARCHIVO.FileName);

            FileInfo InfoArchivo = new FileInfo(Path.GetFullPath(ARCHIVO.FileName));


            bool SubeArchivoATablaCloud = true;

            switch (TIPO_BUCKET)
            {
                case 1:
                    SubeArchivoATablaCloud = false;
                    break;
                case 2:
                    SubeArchivoATablaCloud = false;
                    break;

                default:
                    break;
            }


            using (dbgycEntities3 db = new dbgycEntities3())
            {
                int existe = db.DOCUMENTOS_CLOUD.Where(x => x.ARCHIVO == NombreArchivoNuevo).Count();

                while (existe > 0)
                {

                    if (existe > 0)
                    {
                        Random = rnd.Next(1, 999999).ToString();
                        NombreArchivoNuevo = "_" + empresa + "_" + Random + "." + Path.GetExtension(ARCHIVO.FileName);
                        existe = db.DOCUMENTOS_CLOUD.Where(x => x.ARCHIVO == NombreArchivoNuevo).Count();
                    }
                    else
                    {
                        existe = 0;
                    }

                }

                DOCUMENTOS_CLOUD RegistroDocumento = new DOCUMENTOS_CLOUD();

                RegistroDocumento.ARCHIVO = NombreArchivoNuevo;
                RegistroDocumento.EXTENSION = Path.GetExtension(ARCHIVO.FileName).ToUpper();
                RegistroDocumento.FSUBIDA = DateTime.Now;
                RegistroDocumento.EMPRESA = empresa;
                RegistroDocumento.USUARIO = usuario;
                RegistroDocumento.TAMANO = ARCHIVO.ContentLength / 1000000;
                RegistroDocumento.NOTIFICA_TERMINO = "1";
                RegistroDocumento.DESCRIPCION = DESCRIPCION.ToUpper();
                RegistroDocumento.FINICIO = FINICIO;
                RegistroDocumento.FTERMINO = FTERMINO;
                RegistroDocumento.TRABAJADOR = RUT;
                RegistroDocumento.MAIL_NOTIFICA = MAIL_NOTIFICA;
                RegistroDocumento.DIAS_NOTIFICACION1 = DIAS_NOTIFICACION1;
                RegistroDocumento.DIAS_NOTIFICACION2 = DIAS_NOTIFICACION2;


                if (NOTIFICA == "true")
                {
                    RegistroDocumento.NOTIFICA_TERMINO = "1";
                }
                else
                {
                    RegistroDocumento.NOTIFICA_TERMINO = "0";
                }
                byte[] bytesArchivo = null;

                System.Diagnostics.Debug.WriteLine("Antes de entrar al archivo");

                if (ARCHIVO != null)
                {
                    System.Diagnostics.Debug.WriteLine("ARchivo no nulo");
                    using (var binaryReader = new BinaryReader(ARCHIVO.InputStream))
                    {
                        bytesArchivo = binaryReader.ReadBytes(ARCHIVO.ContentLength);
                    }


                    if (SubeArchivoATablaCloud == true)
                    {
                        //RegistroDocumento.DOC = bytesArchivo;
                    }

                    switch (TIPO_BUCKET)
                    {
                        case 1:
                            var UpdateArchivoContrato = db.CONTRATO.Where(x => x.PERSONA == TRABAJADOR).OrderByDescending(x => x.ID).First();
                            UpdateArchivoContrato.PDF = bytesArchivo;
                            break;
                        case 2:
                            int IdContrato = db.CONTRATO.Where(x => x.PERSONA == TRABAJADOR).OrderByDescending(x => x.ID).Select(x => x.ID).First();
                            ANEXOS NuevoArchivoAnexo = new ANEXOS();
                            NuevoArchivoAnexo.CONTRATO = IdContrato;
                            NuevoArchivoAnexo.PDF = bytesArchivo;
                            NuevoArchivoAnexo.FECHA = DateTime.Now;
                            NuevoArchivoAnexo.FIRMAEMPRESA = true;
                            NuevoArchivoAnexo.FIRMATRABAJADOR = true;
                            NuevoArchivoAnexo.RECHAZADO = false;
                            NuevoArchivoAnexo.OBSERVACIONES = "";

                            db.ANEXOS.Add(NuevoArchivoAnexo);
                            break;

                        default:
                            break;
                    }

                }

                string bucketName = (RUT + "gycsol").ToLower();

                if (CargaArchivoS3(bucketName, TIPO_BUCKET, NombreArchivoNuevo, bytesArchivo))
                {
                    string Nombredirectorio = db.TIPO_BUCKET.Where(x => x.ID == TIPO_BUCKET).Select(x => x.DESCRIPCION).FirstOrDefault();

                    System.Diagnostics.Debug.WriteLine("s3 loop");

                    RegistroDocumento.S3_BUCKET = bucketName;
                    RegistroDocumento.S3_DIRECTORIO = Nombredirectorio;
                    RegistroDocumento.S3_DOCUEMNTO = NombreArchivoNuevo;

                    db.DOCUMENTOS_CLOUD.Add(RegistroDocumento);
                    db.SaveChanges();

                }





            }
        }

        public List<SelectListItemTrabajadores> filtroTrabajadoresVigencia(int faena, bool vigentes)
        {
            string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;

            DateTime hoy = DateTime.Today;

            if (vigentes)
            {
                var TrabajadoresVigentes = (from trabajador in db.PERSONA
                                            join contrato in db.CONTRATO on trabajador.RUT equals contrato.PERSONA
                                            where contrato.FINICIO <= hoy
                                            where contrato.FTERMNO > hoy
                                            where contrato.FIRMAEMPRESA == true
                                            where contrato.FIRMATRABAJADOR == true
                                            where contrato.RECHAZADO == false
                                            where contrato.FAENA == faena
                                            select new SelectListItemTrabajadores { RUT = trabajador.RUT, NOMBRE = trabajador.APATERNO + " " + trabajador.AMATERNO + ", " + trabajador.NOMBRE }
                                     ).GroupBy(q => q.RUT).SelectMany(g => g.Take(1)).ToList();
                if (faena == 0)
                {
                    TrabajadoresVigentes = (from trabajador in db.PERSONA
                                                join contrato in db.CONTRATO on trabajador.RUT equals contrato.PERSONA
                                                where contrato.FINICIO <= hoy
                                                where contrato.FTERMNO > hoy
                                                where contrato.FIRMAEMPRESA == true
                                                where contrato.FIRMATRABAJADOR == true
                                                where contrato.RECHAZADO == false
                                                where contrato.EMPRESA== empresa
                                                select new SelectListItemTrabajadores { RUT = trabajador.RUT, NOMBRE = trabajador.APATERNO + " " + trabajador.AMATERNO + ", " + trabajador.NOMBRE }
                                         ).GroupBy(q => q.RUT).SelectMany(g => g.Take(1)).ToList();
                }


                return TrabajadoresVigentes.OrderBy(t => t.NOMBRE).ToList();
            }
            else
            {
                var TrabajadoresVigentes = (from trabajador in db.PERSONA
                                            join contrato in db.CONTRATO on trabajador.RUT equals contrato.PERSONA
                                            where contrato.FTERMNO > hoy
                                            where contrato.FIRMAEMPRESA == true
                                            where contrato.FIRMATRABAJADOR == true
                                            where contrato.RECHAZADO == false
                                            where contrato.FAENA == faena
                                            select trabajador).GroupBy(q => q.RUT).SelectMany(g => g.Take(1)).ToList();
                if (faena == 0)
                {
                    TrabajadoresVigentes = (from trabajador in db.PERSONA
                                                join contrato in db.CONTRATO on trabajador.RUT equals contrato.PERSONA
                                                where contrato.FTERMNO > hoy
                                                where contrato.FIRMAEMPRESA == true
                                                where contrato.FIRMATRABAJADOR == true
                                                where contrato.RECHAZADO == false
                                                where contrato.EMPRESA == empresa
                                            select trabajador).GroupBy(q => q.RUT).SelectMany(g => g.Take(1)).ToList();

                }
                var trabajadores = (from trabajador in db.PERSONA
                                    join contrato in db.CONTRATO on trabajador.RUT equals contrato.PERSONA
                                    where contrato.EMPRESA == empresa
                                    select trabajador).GroupBy(q => q.RUT).SelectMany(g => g.Take(1)).ToList();

                List<SelectListItemTrabajadores> TrabajadoresNoVigentes = trabajadores.Except(TrabajadoresVigentes).Select(x => new SelectListItemTrabajadores { RUT = x.RUT, NOMBRE = x.APATERNO + " " + x.AMATERNO + ", " + x.NOMBRE }).ToList();


                return TrabajadoresNoVigentes.OrderBy(t => t.NOMBRE).ToList();
            }
        }


        public DateTime fechaTerminoVigencia(string rut)
        {
            if (!(rut == "1" || rut == null || rut == ""))
            {
                CONTRATO c = db.CONTRATO.Where(x => x.PERSONA == rut).OrderByDescending(v => v.FTERMNO).First();
                DateTime dt = c.FTERMNO;

                return dt;
            }
            else
            {
                return DateTime.Now;
            }
            
        }

        public int PoseePermiso(int Servicio)
        {

            IList<PERMISO_USUARIO_Result> Permisos = (IList<PERMISO_USUARIO_Result>)System.Web.HttpContext.Current.Session["permisosUsuario"];

            int Permiso = Permisos.Where(x => x.ID_SERVICIO == Servicio).Select(x => x.NIVEL_PERMISO).FirstOrDefault();


            return Permiso;
        }

        //Verifica que el trabajador tenga un contrato
        public bool PoseeContrato(string rut)
        {
            string empresaActual = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
            var con = from c in db.CONTRATO
                      where c.EMPRESA == empresaActual
                      select c;
            if (con.Where(x => x.PERSONA.Contains(rut)).Any())
            {
                return true;
            }
            return false;
        }
    }


    public class SelectListItemTrabajadores
    {
        public string RUT { get; set; }
        public string NOMBRE { get; set; }
    }
    public class ListaFaenas
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }


}
public class ListaMeses
{
    public int mes { get; set; }
    public string nombre { get; set; }
}
public class Listaanos
{
    public int ano { get; set; }
    public string nombre { get; set; }
}


