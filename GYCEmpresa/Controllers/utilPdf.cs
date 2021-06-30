using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace GYCEmpresa.Controllers
{
    class utilPdf
    {
        private EncoderParameter myEncoderParameter;

        public void addLogo(string archivoFinal, string archivo, string TextoHash, string Nombre, string Rut, DateTime fecha)
        {
            PdfReader reader = new PdfReader(archivo);
            PdfStamper stamp = null;
            Document document = null;
            string newFile = archivoFinal;

            //StreamWriter sw = new StreamWriter(@"E:\TUTORIALES\CODIGOS\46-wpfPdf2\utilPdf.log", true);
            //sw.WriteLine(" ");
            //sw.WriteLine("Inicio addLogo");
            //sw.Flush();

            try
            {
                int pos = archivo.LastIndexOf(".");
                int num = 0;
                while (File.Exists(newFile))
                {
                    num++;
                    newFile = archivo.Substring(0, pos) + num.ToString() + archivo.Substring(pos);
                }

                //sw.WriteLine("Crea el documento");
                //sw.Flush();

                document = new Document(PageSize.A4);

                //sw.WriteLine("Creamos la imagen y le ajustamos el tamaño");
                //sw.Flush();


                //sw.WriteLine("Creamos el documento de salida con PdfStamper para poder modificarlo");
                //sw.Flush();
                FileStream file = new FileStream(newFile, FileMode.CreateNew);
                stamp = new PdfStamper(reader, file);
                document.Open();
                document.NewPage();

                //PdfWriter writer = PdfWriter.GetInstance(document, file);
                //PdfImportedPage page = writer.GetImportedPage(reader, 1);

                // Creamos la imagen y le ajustamos el tamaño

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(CreaImagen("EMPRESA \n" + Nombre  + "\n" + Rut + "\n" + fecha + "\n" + TextoHash));
                img.BorderWidth = 0;
                float Y = 600;
                float X = 20;
                img.SetAbsolutePosition(X, Y);
                float percentage = 0.0f;
                percentage = 20 / img.Width;        //Aplicamos un porcentaje sobre el tamaño original de la imagen
                img.ScalePercent(60);

                //document.OpenDocument();
                //Recorre todas las páginas para poner el logo
                //for (int nPag = 1; nPag <= reader.NumberOfPages; nPag++)  //-->ESTE SE USA CUANDO RECORRE TODAS LAS PÁGINAS
                //foreach (int pag in pags)
                //{
                //sw.WriteLine("Añade el logo a la página " + pag.ToString());
                //sw.Flush();

                //PdfContentByte over = stamp.GetOverContent(nPag);     //-->ESTE SE USA CUANDO RECORRE LAS PÁGINAS
                //PdfContentByte over = stamp.GetOverContent(pag);        // ESTE PARA CUANDO INDICO A QUE PÁGINAS QUIERO PONER EL LOGO
                PdfContentByte over = stamp.GetOverContent(reader.NumberOfPages);
                over.BeginText();
                over.AddImage(img);
                over.EndText();
                //}

                document.Close();
                stamp.Close();
                reader.Close();
                file.Close();
                //sw.WriteLine("Devuelve el archivo " + newFile);
                //sw.WriteLine("Borra el archivo original sin logo");
                //sw.Flush();

                //File.Delete(archivo);
            }
            catch (Exception ex)
            {
                //sw.WriteLine(ex.Message);
                //sw.Flush();

                Console.WriteLine(ex.Message);
            }
            finally
            {
               
                stamp.Close();
                document.Close();
                reader.Close();
                //sw.Close();
            }

            //return newFile;
        }


        public void addQR(string NuevoArchivo, string archivo, string QR)
        {
            Random rnd = new Random();
            string Random = rnd.Next(1, 9999).ToString();

            string Ruta = AppDomain.CurrentDomain.BaseDirectory;
            string Directorio = Ruta + "/temp/";

            PdfReader reader = new PdfReader(archivo);
            PdfStamper stamp = null;
            Document document = null;
            string newFile = NuevoArchivo;

            //StreamWriter sw = new StreamWriter(@"E:\TUTORIALES\CODIGOS\46-wpfPdf2\utilPdf.log", true);
            //sw.WriteLine(" ");
            //sw.WriteLine("Inicio addLogo");
            //sw.Flush();

            try
            {
                int pos = archivo.LastIndexOf(".");
                int num = 0;
                while (File.Exists(newFile))
                {
                    num++;
                    newFile = archivo.Substring(0, pos) + num.ToString() + archivo.Substring(pos);
                }

                //sw.WriteLine("Crea el documento");
                //sw.Flush();

                document = new Document(PageSize.A4);

                //sw.WriteLine("Creamos la imagen y le ajustamos el tamaño");
                //sw.Flush();


                //sw.WriteLine("Creamos el documento de salida con PdfStamper para poder modificarlo");
                //sw.Flush();
                FileStream file = new FileStream(newFile, FileMode.CreateNew);
                stamp = new PdfStamper(reader, file);
                document.Open();
                document.NewPage();

                //PdfWriter writer = PdfWriter.GetInstance(document, file);
                //PdfImportedPage page = writer.GetImportedPage(reader, 1);

                // Creamos la imagen y le ajustamos el tamaño

                //iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(CreaImagen("EMPRESA \n" + Nombre + "\n" + Rut + "\n" + fecha + "\n" + TextoHash));
                string imgQR = CreaQR(QR);
                //imgQR.Save()
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imgQR);
                iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(CreaImagen(QR));

                img.BorderWidth = 0;
                float Y = 400;
                float X = document.PageSize.Width/2 - img.Width/2;
                img.SetAbsolutePosition(X, Y);
                float percentage = 0.0f;
                percentage = 20 / img.Width;        //Aplicamos un porcentaje sobre el tamaño original de la imagen
                img.ScalePercent(90);

                img2.BorderWidth = 0;           //HASH
                float Y2 = 370;
                float X2 = document.PageSize.Width/2 - img2.Width/2;
                img2.SetAbsolutePosition(X2, Y2);
                float percentage2 = 0.0f;
                percentage2 = 20 / img2.Width;        //Aplicamos un porcentaje sobre el tamaño original de la imagen
                img2.ScalePercent(90);

                //document.OpenDocument();
                //Recorre todas las páginas para poner el logo
                //for (int nPag = 1; nPag <= reader.NumberOfPages; nPag++)  //-->ESTE SE USA CUANDO RECORRE TODAS LAS PÁGINAS
                //foreach (int pag in pags)
                //{
                //sw.WriteLine("Añade el logo a la página " + pag.ToString());
                //sw.Flush();

                //PdfContentByte over = stamp.GetOverContent(nPag);     //-->ESTE SE USA CUANDO RECORRE LAS PÁGINAS
                //PdfContentByte over = stamp.GetOverContent(pag);        // ESTE PARA CUANDO INDICO A QUE PÁGINAS QUIERO PONER EL LOGO
                PdfContentByte over = stamp.GetOverContent(reader.NumberOfPages);
                over.BeginText();
                over.AddImage(img);
                over.AddImage(img2);
                over.EndText();
                //}

                document.Close();
                stamp.Close();
                reader.Close();
                file.Close();
                //sw.WriteLine("Devuelve el archivo " + newFile);
                //sw.WriteLine("Borra el archivo original sin logo");
                //sw.Flush();

                //File.Delete(archivo);
            }
            catch (Exception ex)
            {
                //sw.WriteLine(ex.Message);
                //sw.Flush();

                Console.WriteLine(ex.Message);
            }
            finally
            {

                stamp.Close();
                document.Close();
                reader.Close();
                //sw.Close();
            }

            //return newFile;
        }

        private string CreaQR(string qR)
        {
            Random rnd = new Random();
            string Random = rnd.Next(1, 9999).ToString();

            string Ruta = AppDomain.CurrentDomain.BaseDirectory;
            string Directorio = Ruta + "/temp/";
            string RutacompletaImagen = Directorio + "_QR" + "_" + Random + ".jpg";

            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(qR, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
                                   
            var imgQR = code.GetGraphic(5);

            Bitmap myBitmap;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Create a Bitmap object based on a BMP file.
            myBitmap = new Bitmap(imgQR);

            // Get an ImageCodecInfo object that represents the JPEG codec.
            //myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID

            // for the Quality parameter category.
            myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.

            // An EncoderParameters object has an array of EncoderParameter

            // objects. In this case, there is only one

            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with quality level 25.
            myEncoderParameter = new EncoderParameter(myEncoder, 25L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            myBitmap.Save(RutacompletaImagen);


            return RutacompletaImagen;
        }

        private string CreaImagen(string sImageText)
        {
            Random rnd = new Random();
            string Random = rnd.Next(1, 9999).ToString();

            // creating object of Image
            Bitmap objBmpImage = new Bitmap(1, 1);

            // initial width & height of image  
            int intWidth = 0;
            int intHeight = 0;

            // creating object of font with these properties.
            System.Drawing.Font objFont = new System.Drawing.Font("Arial", 20, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

            // creating graphics object
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            // this will give the actual width & height of bitmap image.
            intWidth = (int)objGraphics.MeasureString(sImageText, objFont).Width;
            intHeight = (int)objGraphics.MeasureString(sImageText, objFont).Height;

            // create the bmpImage again with the correct size for the text and font.
            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));

            // add the colors to the new bitmap.
            objGraphics = Graphics.FromImage(objBmpImage);

            // setting Background color
            objGraphics.Clear(Color.White);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            objGraphics.DrawString(sImageText, objFont, new SolidBrush(Color.FromArgb(102, 102, 102)), 0, 0);
            objGraphics.Flush();

            // Now that we have our image final.
            // time to save it to a Drive.


            string Ruta = AppDomain.CurrentDomain.BaseDirectory;
            string Directorio = Ruta + "/temp/";
            string RutacompletaImagen = Directorio + "_HashEmpresa" + "_" + Random + ".jpg";

            if (!Directory.Exists(Directorio))
            {
                Directory.CreateDirectory(Directorio);
            }
            if (File.Exists(RutacompletaImagen))
            {
                Directory.Delete(RutacompletaImagen);
            }

            objBmpImage.Save(RutacompletaImagen);

            // loading it in Picturebox

            // returing this image to use
            return (RutacompletaImagen);
        }


    }
}
