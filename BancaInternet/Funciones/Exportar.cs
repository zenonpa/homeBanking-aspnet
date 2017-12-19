using System;
using System.IO;
using System.Web;
using System.Linq;
using BancaInternet.EN;
using BancaInternet.UTL;
using System.Reflection;
using System.Configuration;
using BancaInternet.wsCuenta;
using BancaInternet.wsUsuario;
using BancaInternet.wsPrestamo;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BancaInternet.Funciones
{
    public class Exportar
    {

        private string KeyMaster = ConfigurationManager.AppSettings["KeyMaestra"];
        private string keyIV = ConfigurationManager.AppSettings["KeyModulo"];

        public void ExportarPDFPrincipal(Document document, PdfWriter writer, string vCodigoSiscredinka, string sNombreCliente)
        {
            try
            {
                /*Encripta la informacion*/
                rnSegRSA ornSegRSA = new rnSegRSA();
                string sValor1 = ornSegRSA.RSA_Encriptar(vCodigoSiscredinka);
                /*********************/

                /*Fuentes*/
                Font Font_Title = FontFactory.GetFont("Arial Black", 16, Font.NORMAL, new BaseColor(255, 128, 0));
                Font Font_SubTitle = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                Font Font_List = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                Font Font_Head_Table = FontFactory.GetFont("Arial", 8, Font.BOLD);
                Font Font_Content_Table = FontFactory.GetFont("Arial", 8, Font.NORMAL);

                document.Open();

                string url1 = HttpContext.Current.Server.MapPath("~/Content/Images/" + "logo-intranet.png");

                PdfContentByte pdfContent;
                Phrase p1Header = new Phrase("Información confidencial", FontFactory.GetFont("verdana", 8));
                Phrase p2Header = new Phrase("1 de 1", FontFactory.GetFont("verdana", 8));
                //Image imgPDF = Image.GetInstance(url);
                Image gif = Image.GetInstance(url1);
                gif.ScaleToFit(110f, 65f);
                PdfPTable pdfTab = new PdfPTable(3);
                PdfPCell pdfCell1 = new PdfPCell(gif);
                PdfPCell pdfCell2 = new PdfPCell(p1Header);
                PdfPCell pdfCell3 = new PdfPCell(p2Header);
                pdfCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                pdfCell1.Border = 0;
                pdfCell2.Border = 0;
                pdfCell3.Border = 0;
                pdfCell3.PaddingRight = 30;
                pdfCell1.PaddingLeft = 30;
                pdfTab.AddCell(pdfCell1);
                pdfTab.AddCell(pdfCell2);
                pdfTab.AddCell(pdfCell3);
                pdfTab.TotalWidth = document.PageSize.Width - 20;
                pdfTab.WriteSelectedRows(0, -1, 10, document.PageSize.Height - 15, writer.DirectContent);
                pdfContent = writer.DirectContent;
                pdfContent.MoveTo(30, document.PageSize.Height - 35);
                pdfContent.Stroke();

                /*Titulo*/
                PdfPTable pdfTab1 = new PdfPTable(1);
                Phrase Titulo = new Phrase("Banca por Internet", Font_Title);
                PdfPCell pdfCell5 = new PdfPCell(Titulo);
                pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell5.Border = 0;
                pdfCell5.PaddingTop = 10f;
                pdfTab1.AddCell(pdfCell5);
                pdfTab1.TotalWidth = document.PageSize.Width + 40;
                pdfContent = writer.DirectContent;
                document.Add(pdfTab1);

                /*Datos Cliente*/
                Paragraph TitleDatosCliente = new Paragraph(new Phrase("Datos de Cliente", Font_SubTitle));
                TitleDatosCliente.PaddingTop = 8f;

                Chunk c1 = new Chunk("Nombre del cliente : ", new Font(FontFactory.GetFont("Arial", 8, Font.BOLD)));
                Chunk c2 = new Chunk(sNombreCliente, new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                Paragraph p2 = new Paragraph();
                p2.PaddingTop = 5f;
                p2.Add(c1);
                p2.Add(c2);
                PdfDiv dDatosCliente = new PdfDiv();
                dDatosCliente.PaddingLeft = 30f;
                dDatosCliente.AddElement(p2);

                /*Informacion de Productos*/
                Paragraph TitleInfoProductos = new Paragraph(new Phrase("Información de Productos", Font_SubTitle));
                TitleInfoProductos.PaddingTop = 8f;

                List lMisCuentas = new List(List.UNORDERED, 10f);
                lMisCuentas.IndentationLeft = 30f;
                lMisCuentas.SetListSymbol("\u2022");
                lMisCuentas.Add(new ListItem(new Paragraph(new Chunk("Mis cuentas", Font_List))));
                List lMisPrestamos = new List(List.UNORDERED, 10f);
                lMisPrestamos.IndentationLeft = 30f;
                lMisPrestamos.SetListSymbol("\u2022");
                lMisPrestamos.Add(new ListItem(new Paragraph(new Chunk("Mis Prestamos", Font_List))));

                /*MIS CUENTAS*/
                PdfPTable tMisCuentas = new PdfPTable(4);
                tMisCuentas.PaddingTop = 15f;
                float[] widths = new float[] { 200f, 110f, 90f, 50f };
                tMisCuentas.LockedWidth = true;
                tMisCuentas.TotalWidth = 450f;
                tMisCuentas.SetWidths(widths);
                tMisCuentas.SpacingBefore = 10;

                /*Encabezado de la lista - MIS CUENTAS*/
                PdfPCell thCabeceraCuenta = new PdfPCell(new Phrase("Tipo de Producto", Font_Head_Table));
                thCabeceraCuenta.UseVariableBorders = true;
                thCabeceraCuenta.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraCuenta.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraCuenta.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraCuenta.BorderColorLeft = new BaseColor(241, 241, 241);
                thCabeceraCuenta.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraCuenta.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraCuenta.PaddingBottom = 3f;

                PdfPCell thCabeceraCuenta2 = new PdfPCell(new Phrase("Nro de Cuenta/Operación", Font_Head_Table));
                thCabeceraCuenta2.UseVariableBorders = true;
                thCabeceraCuenta2.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraCuenta2.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraCuenta2.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraCuenta2.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraCuenta2.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraCuenta2.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraCuenta2.PaddingBottom = 3f;

                PdfPCell thCabeceraCuenta3 = new PdfPCell(new Phrase("Saldo", Font_Head_Table));
                thCabeceraCuenta3.UseVariableBorders = true;
                thCabeceraCuenta3.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraCuenta3.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraCuenta3.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraCuenta3.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraCuenta3.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraCuenta3.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraCuenta3.PaddingBottom = 3f;

                PdfPCell thCabeceraCuenta4 = new PdfPCell(new Phrase("Estado", Font_Head_Table));
                thCabeceraCuenta4.UseVariableBorders = true;
                thCabeceraCuenta4.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraCuenta4.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraCuenta4.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraCuenta4.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraCuenta4.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraCuenta4.BorderColorRight = new BaseColor(241, 241, 241);
                thCabeceraCuenta4.PaddingBottom = 3f;

                tMisCuentas.AddCell(thCabeceraCuenta);
                tMisCuentas.AddCell(thCabeceraCuenta2);
                tMisCuentas.AddCell(thCabeceraCuenta3);
                tMisCuentas.AddCell(thCabeceraCuenta4);

                /*ARMA el cuerpo de tabla - MIS CUENTAS*/
                List<enCuenta> loencuenta = new List<enCuenta>();
                using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
                {
                    loencuenta = wsCuenta.WSObtenerCuentasPasivasCliente(sValor1).ToList<enCuenta>();
                }
                if (loencuenta.Count == 0)
                {
                    PdfPCell ttbContenidoCuenta = new PdfPCell(new Phrase("No existen productos", Font_Content_Table));
                    ttbContenidoCuenta.Colspan = 4;
                    ttbContenidoCuenta.UseVariableBorders = true;
                    ttbContenidoCuenta.Border = 0;
                    ttbContenidoCuenta.BackgroundColor = new BaseColor(254, 248, 243);
                    ttbContenidoCuenta.HorizontalAlignment = Element.ALIGN_CENTER;
                    ttbContenidoCuenta.PaddingBottom = 5f;
                    tMisCuentas.AddCell(ttbContenidoCuenta);
                }
                else 
                {
                    foreach (enCuenta reg in loencuenta)
                    {
                        PdfPCell ttbContenidoCuenta = new PdfPCell(new Phrase(reg.sNombreProducto, Font_Content_Table));
                        ttbContenidoCuenta.UseVariableBorders = true;
                        ttbContenidoCuenta.Border = 0;
                        ttbContenidoCuenta.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoCuenta.HorizontalAlignment = Element.ALIGN_LEFT;
                        ttbContenidoCuenta.PaddingBottom = 5f;

                        PdfPCell ttbContenidoCuenta2 = new PdfPCell(new Phrase(@reg.sNumeroCuenta, Font_Content_Table));
                        ttbContenidoCuenta2.UseVariableBorders = true;
                        ttbContenidoCuenta2.Border = 0;
                        ttbContenidoCuenta2.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoCuenta2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ttbContenidoCuenta2.PaddingBottom = 5f;

                        PdfPCell ttbContenidoCuenta3 = new PdfPCell(new Phrase(reg.sSaldo, Font_Content_Table));
                        ttbContenidoCuenta3.UseVariableBorders = true;
                        ttbContenidoCuenta3.Border = 0;
                        ttbContenidoCuenta3.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoCuenta3.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbContenidoCuenta3.PaddingBottom = 5f;

                        PdfPCell ttbContenidoCuenta4 = new PdfPCell(new Phrase(reg.sCodigoEstado, Font_Content_Table));
                        ttbContenidoCuenta4.UseVariableBorders = true;
                        ttbContenidoCuenta4.Border = 0;
                        ttbContenidoCuenta4.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoCuenta4.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbContenidoCuenta4.PaddingBottom = 5f;

                        tMisCuentas.AddCell(ttbContenidoCuenta);
                        tMisCuentas.AddCell(ttbContenidoCuenta2);
                        tMisCuentas.AddCell(ttbContenidoCuenta3);
                        tMisCuentas.AddCell(ttbContenidoCuenta4);
                    }
                }
               


                /*MIS PRESTAMOS*/
                PdfPTable tMisPrestamos = new PdfPTable(4);
                tMisPrestamos.PaddingTop = 15f;
                widths = new float[] { 200f, 110f, 70f, 70f };
                tMisPrestamos.LockedWidth = true;
                tMisPrestamos.TotalWidth = 450f;
                tMisPrestamos.SetWidths(widths);
                tMisPrestamos.SpacingBefore = 10;

                /*Encabezado de la lista - MIS CUENTAS*/
                PdfPCell thCabeceraPrestamo = new PdfPCell(new Phrase("Tipo de Producto", Font_Head_Table));
                thCabeceraPrestamo.UseVariableBorders = true;
                thCabeceraPrestamo.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraPrestamo.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraPrestamo.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraPrestamo.BorderColorLeft = new BaseColor(241, 241, 241);
                thCabeceraPrestamo.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraPrestamo.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraPrestamo.PaddingBottom = 3f;

                PdfPCell thCabeceraPrestamo2 = new PdfPCell(new Phrase("Nro de Cuenta/Operación", Font_Head_Table));
                thCabeceraPrestamo2.UseVariableBorders = true;
                thCabeceraPrestamo2.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraPrestamo2.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraPrestamo2.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraPrestamo2.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraPrestamo2.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraPrestamo2.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraPrestamo2.PaddingBottom = 3f;

                PdfPCell thCabeceraPrestamo3 = new PdfPCell(new Phrase("Monto Capital", Font_Head_Table));
                thCabeceraPrestamo3.UseVariableBorders = true;
                thCabeceraPrestamo3.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraPrestamo3.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraPrestamo3.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraPrestamo3.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraPrestamo3.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraPrestamo3.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraPrestamo3.PaddingBottom = 3f;

                PdfPCell thCabeceraPrestamo4 = new PdfPCell(new Phrase("Estado del préstamo", Font_Head_Table));
                thCabeceraPrestamo4.UseVariableBorders = true;
                thCabeceraPrestamo4.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraPrestamo4.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraPrestamo4.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraPrestamo4.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraPrestamo4.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraPrestamo4.BorderColorRight = new BaseColor(241, 241, 241);
                thCabeceraPrestamo4.PaddingBottom = 3f;

                tMisPrestamos.AddCell(thCabeceraPrestamo);
                tMisPrestamos.AddCell(thCabeceraPrestamo2);
                tMisPrestamos.AddCell(thCabeceraPrestamo3);
                tMisPrestamos.AddCell(thCabeceraPrestamo4);

                /*ARMA el cuerpo de tabla - MIS CUENTAS*/
                List<enPrestamo> loenPrestamo = new List<enPrestamo>();
                using (IwsPrestamoClient wsPrestamo = new IwsPrestamoClient())
                {
                    loenPrestamo = wsPrestamo.WSObtenerCuentasActivasCliente(sValor1).ToList<enPrestamo>();
                }

                if (loenPrestamo.Count == 0)
                {
                    PdfPCell ttbContenidoCuenta = new PdfPCell(new Phrase("No existen productos", Font_Content_Table));
                    ttbContenidoCuenta.Colspan = 4;
                    ttbContenidoCuenta.UseVariableBorders = true;
                    ttbContenidoCuenta.Border = 0;
                    ttbContenidoCuenta.BackgroundColor = new BaseColor(254, 248, 243);
                    ttbContenidoCuenta.HorizontalAlignment = Element.ALIGN_CENTER;
                    ttbContenidoCuenta.PaddingBottom = 5f;
                    tMisPrestamos.AddCell(ttbContenidoCuenta);
                }
                else 
                {
                    foreach (enPrestamo reg in loenPrestamo)
                    {
                        PdfPCell ttbContenidoPrestamo = new PdfPCell(new Phrase(reg.sNombreProducto, Font_Content_Table));
                        ttbContenidoPrestamo.UseVariableBorders = true;
                        ttbContenidoPrestamo.Border = 0;
                        ttbContenidoPrestamo.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoPrestamo.HorizontalAlignment = Element.ALIGN_LEFT;
                        ttbContenidoPrestamo.PaddingBottom = 5f;

                        PdfPCell ttbContenidoPrestamo2 = new PdfPCell(new Phrase(@reg.sSaldoCapital, Font_Content_Table));
                        ttbContenidoPrestamo2.UseVariableBorders = true;
                        ttbContenidoPrestamo2.Border = 0;
                        ttbContenidoPrestamo2.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoPrestamo2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ttbContenidoPrestamo2.PaddingBottom = 5f;

                        PdfPCell ttbContenidoPrestamo3 = new PdfPCell(new Phrase(reg.sCodigoEstado, Font_Content_Table));
                        ttbContenidoPrestamo3.UseVariableBorders = true;
                        ttbContenidoPrestamo3.Border = 0;
                        ttbContenidoPrestamo3.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoPrestamo3.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbContenidoPrestamo3.PaddingBottom = 5f;

                        PdfPCell ttbContenidoPrestamo4 = new PdfPCell(new Phrase(reg.sCodigoEstado, Font_Content_Table));
                        ttbContenidoPrestamo4.UseVariableBorders = true;
                        ttbContenidoPrestamo4.Border = 0;
                        ttbContenidoPrestamo4.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoPrestamo4.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbContenidoPrestamo4.PaddingBottom = 5f;

                        tMisPrestamos.AddCell(ttbContenidoPrestamo);
                        tMisPrestamos.AddCell(ttbContenidoPrestamo2);
                        tMisPrestamos.AddCell(ttbContenidoPrestamo3);
                        tMisPrestamos.AddCell(ttbContenidoPrestamo4);
                    }
                
                }

                //Guarda informacion en el documento.
                document.Add(TitleDatosCliente);
                document.Add(dDatosCliente);
                document.Add(TitleInfoProductos);
                document.Add(lMisCuentas);
                document.Add(tMisCuentas);
                document.Add(lMisPrestamos);
                document.Add(tMisPrestamos);

            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                throw ex;
            }
        }

        public void ExportarPDFMisCuentas(Document document, PdfWriter writer, string vCodigoSiscredinka, string sNombreCliente)
        {
            try
            {
                /*Encripta la informacion*/
                rnSegRSA ornSegRSA = new rnSegRSA(); 
                string sValor1 = ornSegRSA.RSA_Encriptar(vCodigoSiscredinka);
                /*********************/

                /*Fuentes*/
                Font Font_Title = FontFactory.GetFont("Arial Black", 16, Font.NORMAL, new BaseColor(255, 128, 0));
                Font Font_SubTitle = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                Font Font_List = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                Font Font_Head_Table = FontFactory.GetFont("Arial", 8, Font.BOLD);
                Font Font_Content_Table = FontFactory.GetFont("Arial", 8, Font.NORMAL);

                document.Open();

                string url1 = HttpContext.Current.Server.MapPath("~/Content/Images/" + "logo-intranet.png");

                PdfContentByte pdfContent;
                Phrase p1Header = new Phrase("Información confidencial", FontFactory.GetFont("verdana", 8));
                Phrase p2Header = new Phrase("1 de 1", FontFactory.GetFont("verdana", 8));
                //Image imgPDF = Image.GetInstance(url);
                Image gif = Image.GetInstance(url1);
                gif.ScaleToFit(110f, 65f);
                PdfPTable pdfTab = new PdfPTable(3);
                PdfPCell pdfCell1 = new PdfPCell(gif);
                PdfPCell pdfCell2 = new PdfPCell(p1Header);
                PdfPCell pdfCell3 = new PdfPCell(p2Header);
                pdfCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                pdfCell1.Border = 0;
                pdfCell2.Border = 0;
                pdfCell3.Border = 0;
                pdfCell3.PaddingRight = 30;
                pdfCell1.PaddingLeft = 30;
                pdfTab.AddCell(pdfCell1);
                pdfTab.AddCell(pdfCell2);
                pdfTab.AddCell(pdfCell3);
                pdfTab.TotalWidth = document.PageSize.Width - 20;
                pdfTab.WriteSelectedRows(0, -1, 10, document.PageSize.Height - 15, writer.DirectContent);
                pdfContent = writer.DirectContent;
                pdfContent.MoveTo(30, document.PageSize.Height - 35);
                pdfContent.Stroke();

                /*Titulo*/
                PdfPTable pdfTab1 = new PdfPTable(1);
                Phrase Titulo = new Phrase("Banca por Internet", Font_Title);
                PdfPCell pdfCell5 = new PdfPCell(Titulo);
                pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell5.Border = 0;
                pdfCell5.PaddingTop = 10f;
                pdfTab1.AddCell(pdfCell5);
                pdfTab1.TotalWidth = document.PageSize.Width + 40;
                pdfContent = writer.DirectContent;
                document.Add(pdfTab1);

                /*Datos Cliente*/
                Paragraph TitleDatosCliente = new Paragraph(new Phrase("Datos de Cliente", Font_SubTitle));
                TitleDatosCliente.PaddingTop = 8f;

                Chunk c1 = new Chunk("Nombre del cliente : ", new Font(FontFactory.GetFont("Arial", 8, Font.BOLD)));
                Chunk c2 = new Chunk(sNombreCliente, new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                Paragraph p2 = new Paragraph();
                p2.PaddingTop = 5f;
                p2.Add(c1);
                p2.Add(c2);
                PdfDiv dDatosCliente = new PdfDiv();
                dDatosCliente.PaddingLeft = 30f;
                dDatosCliente.AddElement(p2);

                /*Informacion de Productos*/
                Paragraph TitleInfoProductos = new Paragraph(new Phrase("Información de Productos", Font_SubTitle));
                TitleInfoProductos.PaddingTop = 8f;

                List lMisCuentas = new List(List.UNORDERED, 10f);
                lMisCuentas.IndentationLeft = 30f;
                lMisCuentas.SetListSymbol("\u2022");
                lMisCuentas.Add(new ListItem(new Paragraph(new Chunk("Mis cuentas", Font_List))));

                /*MIS CUENTAS*/
                PdfPTable tMisCuentas = new PdfPTable(4);
                tMisCuentas.PaddingTop = 15f;
                float[] widths = new float[] { 200f, 110f, 90f, 50f };
                tMisCuentas.LockedWidth = true;
                tMisCuentas.TotalWidth = 450f;
                tMisCuentas.SetWidths(widths);
                tMisCuentas.SpacingBefore = 10;

                /*Encabezado de la lista - MIS CUENTAS*/
                PdfPCell thCabeceraCuenta = new PdfPCell(new Phrase("Tipo de Producto", Font_Head_Table));
                thCabeceraCuenta.UseVariableBorders = true;
                thCabeceraCuenta.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraCuenta.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraCuenta.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraCuenta.BorderColorLeft = new BaseColor(241, 241, 241);
                thCabeceraCuenta.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraCuenta.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraCuenta.PaddingBottom = 3f;

                PdfPCell thCabeceraCuenta2 = new PdfPCell(new Phrase("Nro de Cuenta/Operación", Font_Head_Table));
                thCabeceraCuenta2.UseVariableBorders = true;
                thCabeceraCuenta2.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraCuenta2.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraCuenta2.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraCuenta2.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraCuenta2.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraCuenta2.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraCuenta2.PaddingBottom = 3f;

                PdfPCell thCabeceraCuenta3 = new PdfPCell(new Phrase("Saldo", Font_Head_Table));
                thCabeceraCuenta3.UseVariableBorders = true;
                thCabeceraCuenta3.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraCuenta3.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraCuenta3.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraCuenta3.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraCuenta3.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraCuenta3.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraCuenta3.PaddingBottom = 3f;

                PdfPCell thCabeceraCuenta4 = new PdfPCell(new Phrase("Estado", Font_Head_Table));
                thCabeceraCuenta4.UseVariableBorders = true;
                thCabeceraCuenta4.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraCuenta4.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraCuenta4.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraCuenta4.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraCuenta4.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraCuenta4.BorderColorRight = new BaseColor(241, 241, 241);
                thCabeceraCuenta4.PaddingBottom = 3f;

                tMisCuentas.AddCell(thCabeceraCuenta);
                tMisCuentas.AddCell(thCabeceraCuenta2);
                tMisCuentas.AddCell(thCabeceraCuenta3);
                tMisCuentas.AddCell(thCabeceraCuenta4);

                /*ARMA el cuerpo de tabla - MIS CUENTAS*/
                List<enCuenta> loencuenta = new List<enCuenta>();
                using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
                {
                    loencuenta = wsCuenta.WSObtenerCuentasPasivasCliente(sValor1).ToList<enCuenta>();
                }
                if (loencuenta.Count == 0)
                {
                    PdfPCell ttbContenidoCuenta = new PdfPCell(new Phrase("No existen productos", Font_Content_Table));
                    ttbContenidoCuenta.Colspan = 4;
                    ttbContenidoCuenta.UseVariableBorders = true;
                    ttbContenidoCuenta.Border = 0;
                    ttbContenidoCuenta.BackgroundColor = new BaseColor(254, 248, 243);
                    ttbContenidoCuenta.HorizontalAlignment = Element.ALIGN_CENTER;
                    ttbContenidoCuenta.PaddingBottom = 5f;
                    tMisCuentas.AddCell(ttbContenidoCuenta);
                }
                else
                {
                    foreach (enCuenta reg in loencuenta)
                    {
                        PdfPCell ttbContenidoCuenta = new PdfPCell(new Phrase(reg.sNombreProducto, Font_Content_Table));
                        ttbContenidoCuenta.UseVariableBorders = true;
                        ttbContenidoCuenta.Border = 0;
                        ttbContenidoCuenta.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoCuenta.HorizontalAlignment = Element.ALIGN_LEFT;
                        ttbContenidoCuenta.PaddingBottom = 5f;

                        PdfPCell ttbContenidoCuenta2 = new PdfPCell(new Phrase(@reg.sNumeroCuenta, Font_Content_Table));
                        ttbContenidoCuenta2.UseVariableBorders = true;
                        ttbContenidoCuenta2.Border = 0;
                        ttbContenidoCuenta2.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoCuenta2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ttbContenidoCuenta2.PaddingBottom = 5f;

                        PdfPCell ttbContenidoCuenta3 = new PdfPCell(new Phrase(reg.sSaldo, Font_Content_Table));
                        ttbContenidoCuenta3.UseVariableBorders = true;
                        ttbContenidoCuenta3.Border = 0;
                        ttbContenidoCuenta3.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoCuenta3.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbContenidoCuenta3.PaddingBottom = 5f;

                        PdfPCell ttbContenidoCuenta4 = new PdfPCell(new Phrase(reg.sCodigoEstado, Font_Content_Table));
                        ttbContenidoCuenta4.UseVariableBorders = true;
                        ttbContenidoCuenta4.Border = 0;
                        ttbContenidoCuenta4.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoCuenta4.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbContenidoCuenta4.PaddingBottom = 5f;

                        tMisCuentas.AddCell(ttbContenidoCuenta);
                        tMisCuentas.AddCell(ttbContenidoCuenta2);
                        tMisCuentas.AddCell(ttbContenidoCuenta3);
                        tMisCuentas.AddCell(ttbContenidoCuenta4);
                    }
                }

                //Guarda informacion en el documento.
                document.Add(TitleDatosCliente);
                document.Add(dDatosCliente);
                document.Add(TitleInfoProductos);
                document.Add(lMisCuentas);
                document.Add(tMisCuentas);

            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                throw ex;
            }
        }

        public void ExportarPDFMisPrestamos(Document document, PdfWriter writer, string vCodigoSiscredinka, string sNombreCliente)
        {
            try
            {
                /*Encripta la informacion*/
                rnSegRSA ornSegRSA = new rnSegRSA();
                string sValor1 = ornSegRSA.RSA_Encriptar(vCodigoSiscredinka);
                /*********************/

                /*Fuentes*/
                Font Font_Title = FontFactory.GetFont("Arial Black", 16, Font.NORMAL, new BaseColor(255, 128, 0));
                Font Font_SubTitle = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                Font Font_List = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                Font Font_Head_Table = FontFactory.GetFont("Arial", 8, Font.BOLD);
                Font Font_Content_Table = FontFactory.GetFont("Arial", 8, Font.NORMAL);

                document.Open();

                string url1 = HttpContext.Current.Server.MapPath("~/Content/Images/" + "logo-intranet.png");

                PdfContentByte pdfContent;
                Phrase p1Header = new Phrase("Información confidencial", FontFactory.GetFont("verdana", 8));
                Phrase p2Header = new Phrase("1 de 1", FontFactory.GetFont("verdana", 8));
                //Image imgPDF = Image.GetInstance(url);
                Image gif = Image.GetInstance(url1);
                gif.ScaleToFit(110f, 65f);
                PdfPTable pdfTab = new PdfPTable(3);
                PdfPCell pdfCell1 = new PdfPCell(gif);
                PdfPCell pdfCell2 = new PdfPCell(p1Header);
                PdfPCell pdfCell3 = new PdfPCell(p2Header);
                pdfCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                pdfCell1.Border = 0;
                pdfCell2.Border = 0;
                pdfCell3.Border = 0;
                pdfCell3.PaddingRight = 30;
                pdfCell1.PaddingLeft = 30;
                pdfTab.AddCell(pdfCell1);
                pdfTab.AddCell(pdfCell2);
                pdfTab.AddCell(pdfCell3);
                pdfTab.TotalWidth = document.PageSize.Width - 20;
                pdfTab.WriteSelectedRows(0, -1, 10, document.PageSize.Height - 15, writer.DirectContent);
                pdfContent = writer.DirectContent;
                pdfContent.MoveTo(30, document.PageSize.Height - 35);
                pdfContent.Stroke();

                /*Titulo*/
                PdfPTable pdfTab1 = new PdfPTable(1);
                Phrase Titulo = new Phrase("Banca por Internet", Font_Title);
                PdfPCell pdfCell5 = new PdfPCell(Titulo);
                pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell5.Border = 0;
                pdfCell5.PaddingTop = 10f;
                pdfTab1.AddCell(pdfCell5);
                pdfTab1.TotalWidth = document.PageSize.Width + 40;
                pdfContent = writer.DirectContent;
                document.Add(pdfTab1);

                /*Datos Cliente*/
                Paragraph TitleDatosCliente = new Paragraph(new Phrase("Datos de Cliente", Font_SubTitle));
                TitleDatosCliente.PaddingTop = 8f;

                Chunk c1 = new Chunk("Nombre del cliente : ", new Font(FontFactory.GetFont("Arial", 8, Font.BOLD)));
                Chunk c2 = new Chunk(sNombreCliente, new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                Paragraph p2 = new Paragraph();
                p2.PaddingTop = 5f;
                p2.Add(c1);
                p2.Add(c2);
                PdfDiv dDatosCliente = new PdfDiv();
                dDatosCliente.PaddingLeft = 30f;
                dDatosCliente.AddElement(p2);

                /*Informacion de Productos*/
                Paragraph TitleInfoProductos = new Paragraph(new Phrase("Información de Productos", Font_SubTitle));
                TitleInfoProductos.PaddingTop = 8f;

                List lMisPrestamos = new List(List.UNORDERED, 10f);
                lMisPrestamos.IndentationLeft = 30f;
                lMisPrestamos.SetListSymbol("\u2022");
                lMisPrestamos.Add(new ListItem(new Paragraph(new Chunk("Mis Prestamos", Font_List))));

                /*MIS PRESTAMOS*/
                PdfPTable tMisPrestamos = new PdfPTable(4);
                tMisPrestamos.PaddingTop = 15f;
                float[] widths = new float[] { 200f, 110f, 70f, 70f };
                tMisPrestamos.LockedWidth = true;
                tMisPrestamos.TotalWidth = 450f;
                tMisPrestamos.SetWidths(widths);
                tMisPrestamos.SpacingBefore = 10;

                /*Encabezado de la lista - MIS CUENTAS*/
                PdfPCell thCabeceraPrestamo = new PdfPCell(new Phrase("Tipo de Producto", Font_Head_Table));
                thCabeceraPrestamo.UseVariableBorders = true;
                thCabeceraPrestamo.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraPrestamo.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraPrestamo.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraPrestamo.BorderColorLeft = new BaseColor(241, 241, 241);
                thCabeceraPrestamo.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraPrestamo.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraPrestamo.PaddingBottom = 3f;

                PdfPCell thCabeceraPrestamo2 = new PdfPCell(new Phrase("Nro de Cuenta/Operación", Font_Head_Table));
                thCabeceraPrestamo2.UseVariableBorders = true;
                thCabeceraPrestamo2.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraPrestamo2.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraPrestamo2.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraPrestamo2.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraPrestamo2.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraPrestamo2.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraPrestamo2.PaddingBottom = 3f;

                PdfPCell thCabeceraPrestamo3 = new PdfPCell(new Phrase("Monto Capital", Font_Head_Table));
                thCabeceraPrestamo3.UseVariableBorders = true;
                thCabeceraPrestamo3.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraPrestamo3.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraPrestamo3.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraPrestamo3.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraPrestamo3.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraPrestamo3.BorderColorRight = new BaseColor(252, 252, 252);
                thCabeceraPrestamo3.PaddingBottom = 3f;

                PdfPCell thCabeceraPrestamo4 = new PdfPCell(new Phrase("Estado del préstamo", Font_Head_Table));
                thCabeceraPrestamo4.UseVariableBorders = true;
                thCabeceraPrestamo4.HorizontalAlignment = Element.ALIGN_CENTER;
                thCabeceraPrestamo4.BackgroundColor = new BaseColor(241, 241, 241);
                thCabeceraPrestamo4.BorderColorTop = new BaseColor(241, 241, 241);
                thCabeceraPrestamo4.BorderColorBottom = new BaseColor(230, 230, 230);
                thCabeceraPrestamo4.BorderColorLeft = new BaseColor(252, 252, 252);
                thCabeceraPrestamo4.BorderColorRight = new BaseColor(241, 241, 241);
                thCabeceraPrestamo4.PaddingBottom = 3f;

                tMisPrestamos.AddCell(thCabeceraPrestamo);
                tMisPrestamos.AddCell(thCabeceraPrestamo2);
                tMisPrestamos.AddCell(thCabeceraPrestamo3);
                tMisPrestamos.AddCell(thCabeceraPrestamo4);

                /*ARMA el cuerpo de tabla - MIS CUENTAS*/
                List<enPrestamo> loenPrestamo = new List<enPrestamo>();
                using (IwsPrestamoClient wsPrestamo = new IwsPrestamoClient())
                {
                    loenPrestamo = wsPrestamo.WSObtenerCuentasActivasCliente(sValor1).ToList<enPrestamo>();
                }

                if (loenPrestamo.Count == 0)
                {
                    PdfPCell ttbContenidoCuenta = new PdfPCell(new Phrase("No existen productos", Font_Content_Table));
                    ttbContenidoCuenta.Colspan = 4;
                    ttbContenidoCuenta.UseVariableBorders = true;
                    ttbContenidoCuenta.Border = 0;
                    ttbContenidoCuenta.BackgroundColor = new BaseColor(254, 248, 243);
                    ttbContenidoCuenta.HorizontalAlignment = Element.ALIGN_CENTER;
                    ttbContenidoCuenta.PaddingBottom = 5f;
                    tMisPrestamos.AddCell(ttbContenidoCuenta);
                }
                else
                {
                    foreach (enPrestamo reg in loenPrestamo)
                    {
                        PdfPCell ttbContenidoPrestamo = new PdfPCell(new Phrase(reg.sNombreProducto, Font_Content_Table));
                        ttbContenidoPrestamo.UseVariableBorders = true;
                        ttbContenidoPrestamo.Border = 0;
                        ttbContenidoPrestamo.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoPrestamo.HorizontalAlignment = Element.ALIGN_LEFT;
                        ttbContenidoPrestamo.PaddingBottom = 5f;

                        PdfPCell ttbContenidoPrestamo2 = new PdfPCell(new Phrase(@reg.sSaldoCapital, Font_Content_Table));
                        ttbContenidoPrestamo2.UseVariableBorders = true;
                        ttbContenidoPrestamo2.Border = 0;
                        ttbContenidoPrestamo2.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoPrestamo2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ttbContenidoPrestamo2.PaddingBottom = 5f;

                        PdfPCell ttbContenidoPrestamo3 = new PdfPCell(new Phrase(reg.sCodigoEstado, Font_Content_Table));
                        ttbContenidoPrestamo3.UseVariableBorders = true;
                        ttbContenidoPrestamo3.Border = 0;
                        ttbContenidoPrestamo3.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoPrestamo3.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbContenidoPrestamo3.PaddingBottom = 5f;

                        PdfPCell ttbContenidoPrestamo4 = new PdfPCell(new Phrase(reg.sCodigoEstado, Font_Content_Table));
                        ttbContenidoPrestamo4.UseVariableBorders = true;
                        ttbContenidoPrestamo4.Border = 0;
                        ttbContenidoPrestamo4.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbContenidoPrestamo4.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbContenidoPrestamo4.PaddingBottom = 5f;

                        tMisPrestamos.AddCell(ttbContenidoPrestamo);
                        tMisPrestamos.AddCell(ttbContenidoPrestamo2);
                        tMisPrestamos.AddCell(ttbContenidoPrestamo3);
                        tMisPrestamos.AddCell(ttbContenidoPrestamo4);
                    }
                }

                //Guarda informacion en el documento.
                document.Add(TitleDatosCliente);
                document.Add(dDatosCliente);
                document.Add(TitleInfoProductos);
                document.Add(lMisPrestamos);
                document.Add(tMisPrestamos);

            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                ex.Message.ToString());
                throw ex;
            }
        }

        public void ExportarPDFDetalleCuenta(Document document, PdfWriter writer, string vCodigoSiscredinka, string sNombreCliente, string codCuenta)
        {
            try
            {
                enCuenta oenCuenta = new enCuenta();
                oenCuenta.sCodigoSiscredinka = vCodigoSiscredinka;
                oenCuenta.sNumeroCuenta = codCuenta;

                /*Encripta la informacion*/
                string ObjetoSerializado = new JavaScriptSerializer().Serialize(oenCuenta);
                rnSegRSA ornSegRSA = new rnSegRSA();
                string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                /*********************/

                /*Fuentes*/
                Font Font_Title = FontFactory.GetFont("Arial Black", 16, Font.NORMAL, new BaseColor(255, 128, 0));
                Font Font_SubTitle = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                Font Font_List = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                Font Font_Head_Table = FontFactory.GetFont("Arial", 8, Font.BOLD);
                Font Font_Content_Table = FontFactory.GetFont("Arial", 8, Font.NORMAL);

                document.Open();

                string url1 = HttpContext.Current.Server.MapPath("~/Content/Images/" + "logo-intranet.png");

                PdfContentByte pdfContent;
                Phrase p1Header = new Phrase("Información confidencial", FontFactory.GetFont("verdana", 8));
                Phrase p2Header = new Phrase("1 de 1", FontFactory.GetFont("verdana", 8));
                Image gif = Image.GetInstance(url1);
                gif.ScaleToFit(110f, 65f);
                PdfPTable pdfTab = new PdfPTable(3);
                PdfPCell pdfCell1 = new PdfPCell(gif);
                PdfPCell pdfCell2 = new PdfPCell(p1Header);
                PdfPCell pdfCell3 = new PdfPCell(p2Header);
                pdfCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                pdfCell1.Border = 0;
                pdfCell2.Border = 0;
                pdfCell3.Border = 0;
                pdfCell3.PaddingRight = 30;
                pdfCell1.PaddingLeft = 30;
                pdfTab.AddCell(pdfCell1);
                pdfTab.AddCell(pdfCell2);
                pdfTab.AddCell(pdfCell3);
                pdfTab.TotalWidth = document.PageSize.Width - 20;
                pdfTab.WriteSelectedRows(0, -1, 10, document.PageSize.Height - 15, writer.DirectContent);
                pdfContent = writer.DirectContent;
                pdfContent.MoveTo(30, document.PageSize.Height - 35);
                pdfContent.Stroke();

                /*Titulo*/
                PdfPTable pdfTab1 = new PdfPTable(1);
                Phrase Titulo = new Phrase("Banca por Internet", Font_Title);
                PdfPCell pdfCell5 = new PdfPCell(Titulo);
                pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell5.Border = 0;
                pdfCell5.PaddingTop = 10f;
                pdfTab1.AddCell(pdfCell5);
                pdfTab1.TotalWidth = document.PageSize.Width + 40;
                pdfContent = writer.DirectContent;
                document.Add(pdfTab1);

                /*Datos Cliente*/
                Paragraph TitleDatosCliente = new Paragraph(new Phrase("Datos de Cliente", Font_SubTitle));
                TitleDatosCliente.PaddingTop = 8f;

                Chunk c1 = new Chunk("Nombre del cliente : ", new Font(FontFactory.GetFont("Arial", 8, Font.BOLD)));
                Chunk c2 = new Chunk(sNombreCliente, new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                Paragraph p2 = new Paragraph();
                p2.PaddingTop = 5f;
                p2.Add(c1);
                p2.Add(c2);
                PdfDiv dDatosCliente = new PdfDiv();
                dDatosCliente.PaddingLeft = 30f;
                dDatosCliente.AddElement(p2);

                /*Detalle de la cuenta*/
                Paragraph TitleDetalleCuenta = new Paragraph(new Phrase("Detalle de la cuenta", Font_SubTitle));
                TitleDetalleCuenta.PaddingTop = 8f;

                /*Detalle Cuenta*/
                PdfPTable TDetalle = new PdfPTable(4);
                TDetalle.LockedWidth = true;
                TDetalle.TotalWidth = 450f;
                TDetalle.PaddingTop = 15f;
                float[] widdthDetalle = new float[] { 100f, 150f, 100f, 100f };
                TDetalle.SetWidths(widdthDetalle);
                TDetalle.SpacingBefore = 10;

                /*Cabecera*/
                PdfPCell thDetalleCell = new PdfPCell(new Phrase("Nombre del cliente", Font_Head_Table));
                thDetalleCell.UseVariableBorders = true;
                thDetalleCell.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell.BorderWidthTop = 0;
                thDetalleCell.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell.BorderWidthBottom = 0;
                thDetalleCell.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell.PaddingLeft = 5f;
                thDetalleCell.PaddingTop = 8f;
                thDetalleCell.PaddingBottom = 5f;

                PdfPCell thDetalleCell1 = new PdfPCell(new Phrase("Número de cuenta", Font_Head_Table));
                thDetalleCell1.UseVariableBorders = true;
                thDetalleCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell1.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell1.BorderWidthTop = 0;
                thDetalleCell1.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell1.BorderWidthBottom = 0;
                thDetalleCell1.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell1.PaddingLeft = 5f;
                thDetalleCell1.PaddingBottom = 5f;
                thDetalleCell1.PaddingTop = 5f;

                PdfPCell thDetalleCell2 = new PdfPCell(new Phrase("Código de cliente", Font_Head_Table));
                thDetalleCell2.UseVariableBorders = true;
                thDetalleCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell2.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell2.BorderWidthTop = 0;
                thDetalleCell2.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell2.BorderWidthBottom = 0;
                thDetalleCell2.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell2.PaddingLeft = 5f;
                thDetalleCell2.PaddingBottom = 5f;
                thDetalleCell2.PaddingTop = 5f;

                PdfPCell thDetalleCell3 = new PdfPCell(new Phrase("Familia de Producto", Font_Head_Table));
                thDetalleCell3.UseVariableBorders = true;
                thDetalleCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell3.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell3.BorderWidthTop = 0;
                thDetalleCell3.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell3.BorderWidthBottom = 0;
                thDetalleCell3.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell3.PaddingLeft = 5f;
                thDetalleCell3.PaddingBottom = 5f;
                thDetalleCell3.PaddingTop = 5f;

                PdfPCell thDetalleCell4 = new PdfPCell(new Phrase("Producto", Font_Head_Table));
                thDetalleCell4.UseVariableBorders = true;
                thDetalleCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell4.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell4.BorderWidthTop = 0;
                thDetalleCell4.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell4.BorderWidthBottom = 0;
                thDetalleCell4.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell4.PaddingLeft = 5f;
                thDetalleCell4.PaddingBottom = 5f;
                thDetalleCell4.PaddingTop = 5f;

                PdfPCell thDetalleCell5 = new PdfPCell(new Phrase("Saldo Contable", Font_Head_Table));
                thDetalleCell5.UseVariableBorders = true;
                thDetalleCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell5.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell5.BorderWidthTop = 0;
                thDetalleCell5.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell5.BorderWidthBottom = 0;
                thDetalleCell5.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell5.PaddingLeft = 5f;
                thDetalleCell5.PaddingBottom = 8f;
                thDetalleCell5.PaddingTop = 5f;

                PdfPCell thDetalleCell6 = new PdfPCell(new Phrase("Saldo Disponible", Font_Head_Table));
                thDetalleCell6.UseVariableBorders = true;
                thDetalleCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell6.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell6.BorderWidthTop = 0;
                thDetalleCell6.BorderColorLeft = new BaseColor(249, 249, 249);
                thDetalleCell6.BorderWidthBottom = 0;
                thDetalleCell6.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell6.PaddingLeft = 5f;
                thDetalleCell6.PaddingBottom = 8f;
                thDetalleCell6.PaddingTop = 5f;

                PdfPCell thDetalleCell7 = new PdfPCell(new Phrase("Fecha Vencimiento", Font_Head_Table));
                thDetalleCell7.UseVariableBorders = true;
                thDetalleCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell7.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell7.BorderWidthTop = 0;
                thDetalleCell7.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell7.BorderWidthBottom = 0;
                thDetalleCell7.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell7.PaddingLeft = 5f;
                thDetalleCell7.PaddingBottom = 5f;
                thDetalleCell7.PaddingTop = 5f;

                using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
                {
                    oenCuenta = wsCuenta.WSObtenerDetalleCuentasPasivasCliente(strEncriptado);
                }

                /*Cuerpo Detalle cuenta*/
                PdfPCell thContenidoCell = new PdfPCell(new Phrase(oenCuenta.sNombreCompletoCliente, Font_Content_Table));
                thContenidoCell.Colspan = 3;
                thContenidoCell.UseVariableBorders = true;
                thContenidoCell.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell.PaddingTop = 8f;
                thContenidoCell.PaddingBottom = 5f;

                PdfPCell thContenidoCell1 = new PdfPCell(new Phrase(oenCuenta.sNumeroCuenta, Font_Content_Table));
                thContenidoCell1.Colspan = 3;
                thContenidoCell1.UseVariableBorders = true;
                thContenidoCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell1.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell1.PaddingBottom = 5f;
                thContenidoCell1.PaddingTop = 5f;

                PdfPCell thContenidoCell2 = new PdfPCell(new Phrase(oenCuenta.sCodigoSiscredinka, Font_Content_Table));
                thContenidoCell2.Colspan = 3;
                thContenidoCell2.UseVariableBorders = true;
                thContenidoCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell2.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell2.PaddingBottom = 5f;
                thContenidoCell2.PaddingTop = 5f;

                PdfPCell thContenidoCell3 = new PdfPCell(new Phrase(oenCuenta.sProducto, Font_Content_Table));
                thContenidoCell3.Colspan = 3;
                thContenidoCell3.UseVariableBorders = true;
                thContenidoCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell3.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell3.PaddingBottom = 5f;
                thContenidoCell3.PaddingTop = 5f;

                PdfPCell thContenidoCell4 = new PdfPCell(new Phrase(oenCuenta.sNombreProducto, Font_Content_Table));
                thContenidoCell4.Colspan = 3;
                thContenidoCell4.UseVariableBorders = true;
                thContenidoCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell4.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell4.PaddingBottom = 5f;
                thContenidoCell4.PaddingTop = 5f;

                PdfPCell thContenidoCell5 = new PdfPCell(new Phrase(oenCuenta.sSaldoContable, Font_Content_Table));
                thContenidoCell5.UseVariableBorders = true;
                thContenidoCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell5.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell5.PaddingBottom = 8f;
                thContenidoCell5.PaddingTop = 5f;

                PdfPCell thContenidoCell6 = new PdfPCell(new Phrase(oenCuenta.sSaldoDisponible, Font_Content_Table));
                thContenidoCell6.UseVariableBorders = true;
                thContenidoCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell6.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell6.PaddingBottom = 8f;
                thContenidoCell6.PaddingTop = 5f;

                PdfPCell thContenidoCell7 = new PdfPCell(new Phrase(oenCuenta.sFechaVencimiento, Font_Content_Table));
                thContenidoCell7.Colspan = 3;
                thContenidoCell7.UseVariableBorders = true;
                thContenidoCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell7.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell7.PaddingBottom = 5f;
                thContenidoCell7.PaddingTop = 5f;

                TDetalle.AddCell(thDetalleCell);
                TDetalle.AddCell(thContenidoCell);
                TDetalle.AddCell(thDetalleCell1);
                TDetalle.AddCell(thContenidoCell1);
                TDetalle.AddCell(thDetalleCell2);
                TDetalle.AddCell(thContenidoCell2);
                TDetalle.AddCell(thDetalleCell3);
                TDetalle.AddCell(thContenidoCell3);
                TDetalle.AddCell(thDetalleCell4);
                TDetalle.AddCell(thContenidoCell4);
                TDetalle.AddCell(thDetalleCell5);
                TDetalle.AddCell(thContenidoCell5);
                TDetalle.AddCell(thDetalleCell6);
                TDetalle.AddCell(thContenidoCell6);
                if (!oenCuenta.sFechaVencimiento.Equals("01/01/1900"))
                {
                    TDetalle.AddCell(thDetalleCell7);
                    TDetalle.AddCell(thContenidoCell7);
                }

                PdfPTable tContenidoDetalle = new PdfPTable(1);
                PdfPCell tcContenidoDetalle = new PdfPCell(TDetalle);
                tcContenidoDetalle.VerticalAlignment = Element.ALIGN_LEFT;
                tcContenidoDetalle.Border = 0;
                tcContenidoDetalle.PaddingLeft = -30;
                tContenidoDetalle.AddCell(tcContenidoDetalle);
                tContenidoDetalle.TotalWidth = document.PageSize.Width - 20;
                tContenidoDetalle.LockedWidth = true;

                //Guarda informacion en el documento.
                document.Add(TitleDatosCliente);
                document.Add(dDatosCliente);
                document.Add(TitleDetalleCuenta);
                document.Add(tContenidoDetalle);

            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                    ex.Message.ToString());
                throw ex;
            }
        }

        public void ExportarPDFDetallePrestamo(Document document, PdfWriter writer, string vCodigoSiscredinka, string sNombreCliente, string codCuenta)
        {
            try
            {
                enCuenta oenCuenta = new enCuenta();
                oenCuenta.sCodigoSiscredinka = vCodigoSiscredinka;
                oenCuenta.sNumeroCuenta = codCuenta;

                /*Encripta la informacion*/
                string ObjetoSerializado = new JavaScriptSerializer().Serialize(oenCuenta);
                rnSegRSA ornSegRSA = new rnSegRSA();
                string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                /*********************/

                /*Fuentes*/
                Font Font_Title = FontFactory.GetFont("Arial Black", 16, Font.NORMAL, new BaseColor(255, 128, 0));
                Font Font_SubTitle = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                Font Font_List = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                Font Font_Head_Table = FontFactory.GetFont("Arial", 8, Font.BOLD);
                Font Font_Content_Table = FontFactory.GetFont("Arial", 8, Font.NORMAL);

                document.Open();

                string url1 = HttpContext.Current.Server.MapPath("~/Content/Images/" + "logo-intranet.png");

                PdfContentByte pdfContent;
                Phrase p1Header = new Phrase("Información confidencial", FontFactory.GetFont("verdana", 8));
                Phrase p2Header = new Phrase("1 de 1", FontFactory.GetFont("verdana", 8));
                Image gif = Image.GetInstance(url1);
                gif.ScaleToFit(110f, 65f);
                PdfPTable pdfTab = new PdfPTable(3);
                PdfPCell pdfCell1 = new PdfPCell(gif);
                PdfPCell pdfCell2 = new PdfPCell(p1Header);
                PdfPCell pdfCell3 = new PdfPCell(p2Header);
                pdfCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                pdfCell1.Border = 0;
                pdfCell2.Border = 0;
                pdfCell3.Border = 0;
                pdfCell3.PaddingRight = 30;
                pdfCell1.PaddingLeft = 30;
                pdfTab.AddCell(pdfCell1);
                pdfTab.AddCell(pdfCell2);
                pdfTab.AddCell(pdfCell3);
                pdfTab.TotalWidth = document.PageSize.Width - 20;
                pdfTab.WriteSelectedRows(0, -1, 10, document.PageSize.Height - 15, writer.DirectContent);
                pdfContent = writer.DirectContent;
                pdfContent.MoveTo(30, document.PageSize.Height - 35);
                pdfContent.Stroke();

                /*Titulo*/
                PdfPTable pdfTab1 = new PdfPTable(1);
                Phrase Titulo = new Phrase("Banca por Internet", Font_Title);
                PdfPCell pdfCell5 = new PdfPCell(Titulo);
                pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell5.Border = 0;
                pdfCell5.PaddingTop = 10f;
                pdfTab1.AddCell(pdfCell5);
                pdfTab1.TotalWidth = document.PageSize.Width + 40;
                pdfContent = writer.DirectContent;
                document.Add(pdfTab1);

                /*Datos Cliente*/
                Paragraph TitleDatosCliente = new Paragraph(new Phrase("Datos de Cliente", Font_SubTitle));
                TitleDatosCliente.PaddingTop = 8f;

                Chunk c1 = new Chunk("Nombre del cliente : ", new Font(FontFactory.GetFont("Arial", 8, Font.BOLD)));
                Chunk c2 = new Chunk(sNombreCliente, new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                Paragraph p2 = new Paragraph();
                p2.PaddingTop = 5f;
                p2.Add(c1);
                p2.Add(c2);
                PdfDiv dDatosCliente = new PdfDiv();
                dDatosCliente.PaddingLeft = 30f;
                dDatosCliente.AddElement(p2);

                /*Detalle de la cuenta*/
                Paragraph TitleDetalleCuenta = new Paragraph(new Phrase("Detalle de la cuenta", Font_SubTitle));
                TitleDetalleCuenta.PaddingTop = 8f;

                /*Detalle Cuenta*/
                PdfPTable TDetalle = new PdfPTable(4);
                TDetalle.LockedWidth = true;
                TDetalle.TotalWidth = 450f;
                TDetalle.PaddingTop = 15f;
                float[] widdthDetalle = new float[] { 100f, 150f, 100f, 100f };
                TDetalle.SetWidths(widdthDetalle);
                TDetalle.SpacingBefore = 10;

                /*Cabecera*/
                PdfPCell thDetalleCell = new PdfPCell(new Phrase("Familia de crédito", Font_Head_Table));
                thDetalleCell.UseVariableBorders = true;
                thDetalleCell.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell.BorderWidthTop = 0;
                thDetalleCell.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell.BorderWidthBottom = 0;
                thDetalleCell.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell.PaddingLeft = 5f;
                thDetalleCell.PaddingTop = 8f;
                thDetalleCell.PaddingBottom = 5f;

                PdfPCell thDetalleCell1 = new PdfPCell(new Phrase("Producto", Font_Head_Table));
                thDetalleCell1.UseVariableBorders = true;
                thDetalleCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell1.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell1.BorderWidthTop = 0;
                thDetalleCell1.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell1.BorderWidthBottom = 0;
                thDetalleCell1.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell1.PaddingLeft = 5f;
                thDetalleCell1.PaddingBottom = 5f;
                thDetalleCell1.PaddingTop = 5f;

                PdfPCell thDetalleCell2 = new PdfPCell(new Phrase("Número de crédito", Font_Head_Table));
                thDetalleCell2.UseVariableBorders = true;
                thDetalleCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell2.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell2.BorderWidthTop = 0;
                thDetalleCell2.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell2.BorderWidthBottom = 0;
                thDetalleCell2.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell2.PaddingLeft = 5f;
                thDetalleCell2.PaddingBottom = 5f;
                thDetalleCell2.PaddingTop = 5f;

                PdfPCell thDetalleCell3 = new PdfPCell(new Phrase("Estado", Font_Head_Table));
                thDetalleCell3.UseVariableBorders = true;
                thDetalleCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell3.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell3.BorderWidthTop = 0;
                thDetalleCell3.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell3.BorderWidthBottom = 0;
                thDetalleCell3.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell3.PaddingLeft = 5f;
                thDetalleCell3.PaddingBottom = 5f;
                thDetalleCell3.PaddingTop = 5f;

                PdfPCell thDetalleCell4 = new PdfPCell(new Phrase("Tasa Efectiva Anual", Font_Head_Table));
                thDetalleCell4.UseVariableBorders = true;
                thDetalleCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell4.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell4.BorderWidthTop = 0;
                thDetalleCell4.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell4.BorderWidthBottom = 0;
                thDetalleCell4.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell4.PaddingLeft = 5f;
                thDetalleCell4.PaddingBottom = 5f;
                thDetalleCell4.PaddingTop = 5f;

                PdfPCell thDetalleCell5 = new PdfPCell(new Phrase("Total de cuotas", Font_Head_Table));
                thDetalleCell5.UseVariableBorders = true;
                thDetalleCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell5.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell5.BorderWidthTop = 0;
                thDetalleCell5.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell5.BorderWidthBottom = 0;
                thDetalleCell5.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell5.PaddingLeft = 5f;
                thDetalleCell5.PaddingBottom = 8f;
                thDetalleCell5.PaddingTop = 5f;

                PdfPCell thDetalleCell6 = new PdfPCell(new Phrase("Cuotas pendientes", Font_Head_Table));
                thDetalleCell6.UseVariableBorders = true;
                thDetalleCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell6.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell6.BorderWidthTop = 0;
                thDetalleCell6.BorderColorLeft = new BaseColor(249, 249, 249);
                thDetalleCell6.BorderWidthBottom = 0;
                thDetalleCell6.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell6.PaddingLeft = 5f;
                thDetalleCell6.PaddingBottom = 8f;
                thDetalleCell6.PaddingTop = 5f;

                PdfPCell thDetalleCell7 = new PdfPCell(new Phrase("Saldo Deudor", Font_Head_Table));
                thDetalleCell7.UseVariableBorders = true;
                thDetalleCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell7.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell7.BorderWidthTop = 0;
                thDetalleCell7.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell7.BorderWidthBottom = 0;
                thDetalleCell7.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell7.PaddingLeft = 5f;
                thDetalleCell7.PaddingBottom = 5f;
                thDetalleCell7.PaddingTop = 5f;

                PdfPCell thDetalleCell8 = new PdfPCell(new Phrase("Fecha de vencimiento del crédito", Font_Head_Table));
                thDetalleCell8.UseVariableBorders = true;
                thDetalleCell8.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell8.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell8.BorderWidthTop = 0;
                thDetalleCell8.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell8.BorderWidthBottom = 0;
                thDetalleCell8.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell8.PaddingLeft = 5f;
                thDetalleCell8.PaddingBottom = 5f;
                thDetalleCell8.PaddingTop = 5f;

                /*Obtiene Información*/
                enPrestamo oenPrestamo = new enPrestamo();
                using (IwsPrestamoClient wsPrestamo = new IwsPrestamoClient())
                {
                    oenPrestamo = wsPrestamo.WSObtenerDetalleCreditosActivosCliente(strEncriptado);
                }

                /*Cuerpo Detalle cuenta*/
                PdfPCell thContenidoCell = new PdfPCell(new Phrase(oenPrestamo.sTipoCredito, Font_Content_Table));
                thContenidoCell.Colspan = 3;
                thContenidoCell.UseVariableBorders = true;
                thContenidoCell.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell.PaddingTop = 8f;
                thContenidoCell.PaddingBottom = 5f;

                PdfPCell thContenidoCell1 = new PdfPCell(new Phrase(oenPrestamo.sNombreProducto, Font_Content_Table));
                thContenidoCell1.Colspan = 3;
                thContenidoCell1.UseVariableBorders = true;
                thContenidoCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell1.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell1.PaddingBottom = 5f;
                thContenidoCell1.PaddingTop = 5f;

                PdfPCell thContenidoCell2 = new PdfPCell(new Phrase(oenPrestamo.sNumeroCredito, Font_Content_Table));
                thContenidoCell2.Colspan = 3;
                thContenidoCell2.UseVariableBorders = true;
                thContenidoCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell2.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell2.PaddingBottom = 5f;
                thContenidoCell2.PaddingTop = 5f;

                PdfPCell thContenidoCell3 = new PdfPCell(new Phrase(oenPrestamo.sEstadoCuenta, Font_Content_Table));
                thContenidoCell3.Colspan = 3;
                thContenidoCell3.UseVariableBorders = true;
                thContenidoCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell3.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell3.PaddingBottom = 5f;
                thContenidoCell3.PaddingTop = 5f;

                PdfPCell thContenidoCell4 = new PdfPCell(new Phrase(oenPrestamo.sTEA.ToString(), Font_Content_Table));
                thContenidoCell4.Colspan = 3;
                thContenidoCell4.UseVariableBorders = true;
                thContenidoCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell4.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell4.PaddingBottom = 5f;
                thContenidoCell4.PaddingTop = 5f;

                PdfPCell thContenidoCell5 = new PdfPCell(new Phrase(oenPrestamo.nTotalCuotas.ToString(), Font_Content_Table));
                thContenidoCell5.UseVariableBorders = true;
                thContenidoCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell5.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell5.PaddingBottom = 8f;
                thContenidoCell5.PaddingTop = 5f;

                PdfPCell thContenidoCell6 = new PdfPCell(new Phrase(oenPrestamo.nTotalCuotasPendientes.ToString(), Font_Content_Table));
                thContenidoCell6.UseVariableBorders = true;
                thContenidoCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell6.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell6.PaddingBottom = 8f;
                thContenidoCell6.PaddingTop = 5f;

                PdfPCell thContenidoCell7 = new PdfPCell(new Phrase(oenPrestamo.sSaldoCapital, Font_Content_Table));
                thContenidoCell7.Colspan = 3;
                thContenidoCell7.UseVariableBorders = true;
                thContenidoCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell7.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell7.PaddingBottom = 5f;
                thContenidoCell7.PaddingTop = 5f;

                PdfPCell thContenidoCell8 = new PdfPCell(new Phrase(oenPrestamo.sFechaVencimiento, Font_Content_Table));
                thContenidoCell8.Colspan = 3;
                thContenidoCell8.UseVariableBorders = true;
                thContenidoCell8.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell8.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell8.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell8.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell8.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell8.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell8.PaddingBottom = 5f;
                thContenidoCell8.PaddingTop = 5f;

                TDetalle.AddCell(thDetalleCell);
                TDetalle.AddCell(thContenidoCell);
                TDetalle.AddCell(thDetalleCell1);
                TDetalle.AddCell(thContenidoCell1);
                TDetalle.AddCell(thDetalleCell2);
                TDetalle.AddCell(thContenidoCell2);
                TDetalle.AddCell(thDetalleCell3);
                TDetalle.AddCell(thContenidoCell3);
                TDetalle.AddCell(thDetalleCell4);
                TDetalle.AddCell(thContenidoCell4);
                TDetalle.AddCell(thDetalleCell5);
                TDetalle.AddCell(thContenidoCell5);
                TDetalle.AddCell(thDetalleCell6);
                TDetalle.AddCell(thContenidoCell6);
                TDetalle.AddCell(thDetalleCell7);
                TDetalle.AddCell(thContenidoCell7);
                TDetalle.AddCell(thDetalleCell8);
                TDetalle.AddCell(thContenidoCell8);

                PdfPTable tContenidoDetalle = new PdfPTable(1);
                PdfPCell tcContenidoDetalle = new PdfPCell(TDetalle);
                tcContenidoDetalle.VerticalAlignment = Element.ALIGN_LEFT;
                tcContenidoDetalle.Border = 0;
                tcContenidoDetalle.PaddingLeft = -30;
                tContenidoDetalle.AddCell(tcContenidoDetalle);
                tContenidoDetalle.TotalWidth = document.PageSize.Width - 20;
                tContenidoDetalle.LockedWidth = true;

                //Guarda informacion en el documento.
                document.Add(TitleDatosCliente);
                document.Add(dDatosCliente);
                document.Add(TitleDetalleCuenta);
                document.Add(tContenidoDetalle);

            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                    ex.Message.ToString());
                throw ex;
            }
        }

        public void ExportarPDFMovimientos(Document document, PdfWriter writer, string vCodigoSiscredinka, string sNombreCliente, string codCuenta, string NumeroRegistros)
        {
            try
            {
                enCuenta oenCuenta = new enCuenta();
                oenCuenta.sNumeroCuenta = codCuenta;
                oenCuenta.sCodigoSiscredinka = vCodigoSiscredinka;
                oenCuenta.NumeroRegistros = int.Parse(NumeroRegistros);

                /*Encripta la informacion*/
                string ObjetoSerializado = new JavaScriptSerializer().Serialize(oenCuenta);
                rnSegRSA ornSegRSA = new rnSegRSA();
                string strEncriptado = ornSegRSA.RSA_Encriptar(ObjetoSerializado);
                /*********************/

                /*Fuentes*/
                Font Font_Title = FontFactory.GetFont("Arial Black", 16, Font.NORMAL, new BaseColor(255, 128, 0));
                Font Font_SubTitle = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                Font Font_List = FontFactory.GetFont("Arial", 10, Font.NORMAL);
                Font Font_Head_Table = FontFactory.GetFont("Arial", 8, Font.BOLD);
                Font Font_Content_Table = FontFactory.GetFont("Arial", 8, Font.NORMAL);

                document.Open();

                string url1 = HttpContext.Current.Server.MapPath("~/Content/Images/" + "logo-intranet.png");

                PdfContentByte pdfContent;
                Phrase p1Header = new Phrase("Información confidencial", FontFactory.GetFont("verdana", 8));
                Phrase p2Header = new Phrase("1 de 1", FontFactory.GetFont("verdana", 8));
                Image gif = Image.GetInstance(url1);
                gif.ScaleToFit(110f, 65f);
                PdfPTable pdfTab = new PdfPTable(3);
                PdfPCell pdfCell1 = new PdfPCell(gif);
                PdfPCell pdfCell2 = new PdfPCell(p1Header);
                PdfPCell pdfCell3 = new PdfPCell(p2Header);
                pdfCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                pdfCell1.Border = 0;
                pdfCell2.Border = 0;
                pdfCell3.Border = 0;
                pdfCell3.PaddingRight = 30;
                pdfCell1.PaddingLeft = 30;
                pdfTab.AddCell(pdfCell1);
                pdfTab.AddCell(pdfCell2);
                pdfTab.AddCell(pdfCell3);
                pdfTab.TotalWidth = document.PageSize.Width - 20;
                pdfTab.WriteSelectedRows(0, -1, 10, document.PageSize.Height - 15, writer.DirectContent);
                pdfContent = writer.DirectContent;
                pdfContent.MoveTo(30, document.PageSize.Height - 35);
                pdfContent.Stroke();

                /*Titulo*/
                PdfPTable pdfTab1 = new PdfPTable(1);
                Phrase Titulo = new Phrase("Banca por Internet", Font_Title);
                PdfPCell pdfCell5 = new PdfPCell(Titulo);
                pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell5.Border = 0;
                pdfCell5.PaddingTop = 10f;
                pdfTab1.AddCell(pdfCell5);
                pdfTab1.TotalWidth = document.PageSize.Width + 40;
                pdfContent = writer.DirectContent;
                document.Add(pdfTab1);

                /*Datos Cliente*/
                Paragraph TitleDatosCliente = new Paragraph(new Phrase("Datos de Cliente", Font_SubTitle));
                TitleDatosCliente.PaddingTop = 8f;

                Chunk c1 = new Chunk("Nombre del cliente : ", new Font(FontFactory.GetFont("Arial", 8, Font.BOLD)));
                Chunk c2 = new Chunk(sNombreCliente, new Font(FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                Paragraph p2 = new Paragraph();
                p2.PaddingTop = 5f;
                p2.Add(c1);
                p2.Add(c2);
                PdfDiv dDatosCliente = new PdfDiv();
                dDatosCliente.PaddingLeft = 30f;
                dDatosCliente.AddElement(p2);

                /*Detalle de la cuenta*/
                Paragraph TitleDetalleCuenta = new Paragraph(new Phrase("Detalle de la cuenta", Font_SubTitle));
                TitleDetalleCuenta.PaddingTop = 8f;

                /*Detalle Cuenta*/
                PdfPTable TDetalle = new PdfPTable(4);
                TDetalle.LockedWidth = true;
                TDetalle.TotalWidth = 450f;
                TDetalle.PaddingTop = 15f;
                float[] widdthDetalle = new float[] { 100f, 150f, 100f, 100f };
                TDetalle.SetWidths(widdthDetalle);
                TDetalle.SpacingBefore = 10;

                /*Cabecera*/
                PdfPCell thDetalleCell = new PdfPCell(new Phrase("Nombre del cliente", Font_Head_Table));
                thDetalleCell.UseVariableBorders = true;
                thDetalleCell.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell.BorderWidthTop = 0;
                thDetalleCell.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell.BorderWidthBottom = 0;
                thDetalleCell.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell.PaddingLeft = 5f;
                thDetalleCell.PaddingTop = 8f;
                thDetalleCell.PaddingBottom = 5f;

                PdfPCell thDetalleCell1 = new PdfPCell(new Phrase("Número de cuenta", Font_Head_Table));
                thDetalleCell1.UseVariableBorders = true;
                thDetalleCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell1.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell1.BorderWidthTop = 0;
                thDetalleCell1.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell1.BorderWidthBottom = 0;
                thDetalleCell1.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell1.PaddingLeft = 5f;
                thDetalleCell1.PaddingBottom = 5f;
                thDetalleCell1.PaddingTop = 5f;

                PdfPCell thDetalleCell2 = new PdfPCell(new Phrase("Código de cliente", Font_Head_Table));
                thDetalleCell2.UseVariableBorders = true;
                thDetalleCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell2.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell2.BorderWidthTop = 0;
                thDetalleCell2.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell2.BorderWidthBottom = 0;
                thDetalleCell2.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell2.PaddingLeft = 5f;
                thDetalleCell2.PaddingBottom = 5f;
                thDetalleCell2.PaddingTop = 5f;

                PdfPCell thDetalleCell3 = new PdfPCell(new Phrase("Familia de Producto", Font_Head_Table));
                thDetalleCell3.UseVariableBorders = true;
                thDetalleCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell3.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell3.BorderWidthTop = 0;
                thDetalleCell3.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell3.BorderWidthBottom = 0;
                thDetalleCell3.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell3.PaddingLeft = 5f;
                thDetalleCell3.PaddingBottom = 5f;
                thDetalleCell3.PaddingTop = 5f;

                PdfPCell thDetalleCell4 = new PdfPCell(new Phrase("Producto", Font_Head_Table));
                thDetalleCell4.UseVariableBorders = true;
                thDetalleCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell4.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell4.BorderWidthTop = 0;
                thDetalleCell4.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell4.BorderWidthBottom = 0;
                thDetalleCell4.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell4.PaddingLeft = 5f;
                thDetalleCell4.PaddingBottom = 5f;
                thDetalleCell4.PaddingTop = 5f;

                PdfPCell thDetalleCell5 = new PdfPCell(new Phrase("Saldo Contable", Font_Head_Table));
                thDetalleCell5.UseVariableBorders = true;
                thDetalleCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell5.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell5.BorderWidthTop = 0;
                thDetalleCell5.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell5.BorderWidthBottom = 0;
                thDetalleCell5.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell5.PaddingLeft = 5f;
                thDetalleCell5.PaddingBottom = 8f;
                thDetalleCell5.PaddingTop = 5f;

                PdfPCell thDetalleCell6 = new PdfPCell(new Phrase("Saldo Disponible", Font_Head_Table));
                thDetalleCell6.UseVariableBorders = true;
                thDetalleCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell6.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell6.BorderWidthTop = 0;
                thDetalleCell6.BorderColorLeft = new BaseColor(249, 249, 249);
                thDetalleCell6.BorderWidthBottom = 0;
                thDetalleCell6.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell6.PaddingLeft = 5f;
                thDetalleCell6.PaddingBottom = 8f;
                thDetalleCell6.PaddingTop = 5f;

                PdfPCell thDetalleCell7 = new PdfPCell(new Phrase("Fecha Vencimiento", Font_Head_Table));
                thDetalleCell7.UseVariableBorders = true;
                thDetalleCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                thDetalleCell7.BackgroundColor = new BaseColor(249, 249, 249);
                thDetalleCell7.BorderWidthTop = 0;
                thDetalleCell7.BorderColorLeft = new BaseColor(234, 108, 29);
                thDetalleCell7.BorderWidthBottom = 0;
                thDetalleCell7.BorderColorRight = new BaseColor(249, 249, 249);
                thDetalleCell7.PaddingLeft = 5f;
                thDetalleCell7.PaddingBottom = 5f;
                thDetalleCell7.PaddingTop = 5f;

                /*Obtiene Información*/
                using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
                {
                    oenCuenta = wsCuenta.WSObtenerDetalleCuentasPasivasCliente(strEncriptado);
                }
                List<enCuenta> loencuenta = new List<enCuenta>();
                using (IwsCuentaClient wsCuenta = new IwsCuentaClient())
                {
                    loencuenta = wsCuenta.WSObtenerMovimientosCuentaPasivasCliente(strEncriptado).ToList<enCuenta>();
                }

                /*Cuerpo Detalle cuenta*/
                PdfPCell thContenidoCell = new PdfPCell(new Phrase(oenCuenta.sNombreCompletoCliente, Font_Content_Table));
                thContenidoCell.Colspan = 3;
                thContenidoCell.UseVariableBorders = true;
                thContenidoCell.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell.PaddingTop = 8f;
                thContenidoCell.PaddingBottom = 5f;

                PdfPCell thContenidoCell1 = new PdfPCell(new Phrase(oenCuenta.sNumeroCuenta, Font_Content_Table));
                thContenidoCell1.Colspan = 3;
                thContenidoCell1.UseVariableBorders = true;
                thContenidoCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell1.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell1.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell1.PaddingBottom = 5f;
                thContenidoCell1.PaddingTop = 5f;

                PdfPCell thContenidoCell2 = new PdfPCell(new Phrase(oenCuenta.sCodigoSiscredinka, Font_Content_Table));
                thContenidoCell2.Colspan = 3;
                thContenidoCell2.UseVariableBorders = true;
                thContenidoCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell2.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell2.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell2.PaddingBottom = 5f;
                thContenidoCell2.PaddingTop = 5f;

                PdfPCell thContenidoCell3 = new PdfPCell(new Phrase(oenCuenta.sProducto, Font_Content_Table));
                thContenidoCell3.Colspan = 3;
                thContenidoCell3.UseVariableBorders = true;
                thContenidoCell3.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell3.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell3.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell3.PaddingBottom = 5f;
                thContenidoCell3.PaddingTop = 5f;

                PdfPCell thContenidoCell4 = new PdfPCell(new Phrase(oenCuenta.sNombreProducto, Font_Content_Table));
                thContenidoCell4.Colspan = 3;
                thContenidoCell4.UseVariableBorders = true;
                thContenidoCell4.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell4.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell4.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell4.PaddingBottom = 5f;
                thContenidoCell4.PaddingTop = 5f;

                PdfPCell thContenidoCell5 = new PdfPCell(new Phrase(oenCuenta.sSaldoContable, Font_Content_Table));
                thContenidoCell5.UseVariableBorders = true;
                thContenidoCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell5.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell5.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell5.PaddingBottom = 8f;
                thContenidoCell5.PaddingTop = 5f;

                PdfPCell thContenidoCell6 = new PdfPCell(new Phrase(oenCuenta.sSaldoDisponible, Font_Content_Table));
                thContenidoCell6.UseVariableBorders = true;
                thContenidoCell6.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell6.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell6.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell6.PaddingBottom = 8f;
                thContenidoCell6.PaddingTop = 5f;

                PdfPCell thContenidoCell7 = new PdfPCell(new Phrase(oenCuenta.sFechaVencimiento, Font_Content_Table));
                thContenidoCell7.Colspan = 3;
                thContenidoCell7.UseVariableBorders = true;
                thContenidoCell7.HorizontalAlignment = Element.ALIGN_LEFT;
                thContenidoCell7.BackgroundColor = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorTop = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorLeft = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorBottom = new BaseColor(249, 249, 249);
                thContenidoCell7.BorderColorRight = new BaseColor(249, 249, 249);
                thContenidoCell7.PaddingBottom = 5f;
                thContenidoCell7.PaddingTop = 5f;

                TDetalle.AddCell(thDetalleCell);
                TDetalle.AddCell(thContenidoCell);
                TDetalle.AddCell(thDetalleCell1);
                TDetalle.AddCell(thContenidoCell1);
                TDetalle.AddCell(thDetalleCell2);
                TDetalle.AddCell(thContenidoCell2);
                TDetalle.AddCell(thDetalleCell3);
                TDetalle.AddCell(thContenidoCell3);
                TDetalle.AddCell(thDetalleCell4);
                TDetalle.AddCell(thContenidoCell4);
                TDetalle.AddCell(thDetalleCell5);
                TDetalle.AddCell(thContenidoCell5);
                TDetalle.AddCell(thDetalleCell6);
                TDetalle.AddCell(thContenidoCell6);
                if (!oenCuenta.sFechaVencimiento.Equals("01/01/1900"))
                {
                    TDetalle.AddCell(thDetalleCell7);
                    TDetalle.AddCell(thContenidoCell7);
                }

                PdfPTable tContenidoDetalle = new PdfPTable(1);
                PdfPCell tcContenidoDetalle = new PdfPCell(TDetalle);
                tcContenidoDetalle.VerticalAlignment = Element.ALIGN_LEFT;
                tcContenidoDetalle.Border = 0;
                tcContenidoDetalle.PaddingLeft = -30;
                tContenidoDetalle.AddCell(tcContenidoDetalle);
                tContenidoDetalle.TotalWidth = document.PageSize.Width - 20;
                tContenidoDetalle.LockedWidth = true;

                /*Movimiento de la cuenta*/
                Paragraph TitleMovimientos = new Paragraph(new Phrase("Movimiento de la cuenta", Font_SubTitle));
                TitleMovimientos.PaddingTop = 8f;

                /*Cabecera Movimiento de cuenta*/
                PdfPTable tMovimientos = new PdfPTable(8);
                tMovimientos.PaddingTop = 15f;
                float[] twMovimientos = new float[] { 75f, 75f, 80f, 40f, 75f, 60f, 40f, 55f };
                tMovimientos.LockedWidth = true;
                tMovimientos.TotalWidth = 500f;
                tMovimientos.SetWidths(twMovimientos);
                tMovimientos.SpacingBefore = 10;

                PdfPCell thMovimiento1 = new PdfPCell(new Phrase("Fecha de movimiento", Font_Head_Table));
                thMovimiento1.UseVariableBorders = true;
                thMovimiento1.HorizontalAlignment = Element.ALIGN_CENTER;
                thMovimiento1.BackgroundColor = new BaseColor(241, 241, 241);
                thMovimiento1.BorderColorTop = new BaseColor(241, 241, 241);
                thMovimiento1.BorderColorLeft = new BaseColor(241, 241, 241);
                thMovimiento1.BorderColorBottom = new BaseColor(230, 230, 230);
                thMovimiento1.BorderColorRight = new BaseColor(252, 252, 252);
                thMovimiento1.PaddingBottom = 3f;

                PdfPCell thMovimiento2 = new PdfPCell(new Phrase("Nro. de Transacción", Font_Head_Table));
                thMovimiento2.UseVariableBorders = true;
                thMovimiento2.HorizontalAlignment = Element.ALIGN_CENTER;
                thMovimiento2.BackgroundColor = new BaseColor(241, 241, 241);
                thMovimiento2.BorderColorTop = new BaseColor(241, 241, 241);
                thMovimiento2.BorderColorLeft = new BaseColor(252, 252, 252);
                thMovimiento2.BorderColorRight = new BaseColor(252, 252, 252);
                thMovimiento2.BorderColorBottom = new BaseColor(230, 230, 230);
                thMovimiento2.PaddingBottom = 3f;

                PdfPCell thMovimiento3 = new PdfPCell(new Phrase("Movimiento", Font_Head_Table));
                thMovimiento3.UseVariableBorders = true;
                thMovimiento3.HorizontalAlignment = Element.ALIGN_CENTER;
                thMovimiento3.BackgroundColor = new BaseColor(241, 241, 241);
                thMovimiento3.BorderColorTop = new BaseColor(241, 241, 241);
                thMovimiento3.BorderColorBottom = new BaseColor(230, 230, 230);
                thMovimiento3.BorderColorLeft = new BaseColor(252, 252, 252);
                thMovimiento3.BorderColorRight = new BaseColor(252, 252, 252);
                thMovimiento3.PaddingBottom = 3f;

                PdfPCell thMovimiento4 = new PdfPCell(new Phrase("Canal", Font_Head_Table));
                thMovimiento4.UseVariableBorders = true;
                thMovimiento4.HorizontalAlignment = Element.ALIGN_CENTER;
                thMovimiento4.BackgroundColor = new BaseColor(241, 241, 241);
                thMovimiento4.BorderColorTop = new BaseColor(241, 241, 241);
                thMovimiento4.BorderColorBottom = new BaseColor(230, 230, 230);
                thMovimiento4.BorderColorLeft = new BaseColor(252, 252, 252);
                thMovimiento4.BorderColorRight = new BaseColor(252, 252, 252);
                thMovimiento4.PaddingBottom = 3f;

                PdfPCell thMovimiento5 = new PdfPCell(new Phrase("Importe de Movimiento", Font_Head_Table));
                thMovimiento5.UseVariableBorders = true;
                thMovimiento5.HorizontalAlignment = Element.ALIGN_CENTER;
                thMovimiento5.BackgroundColor = new BaseColor(241, 241, 241);
                thMovimiento5.BorderColorTop = new BaseColor(241, 241, 241);
                thMovimiento5.BorderColorBottom = new BaseColor(230, 230, 230);
                thMovimiento5.BorderColorLeft = new BaseColor(252, 252, 252);
                thMovimiento5.BorderColorRight = new BaseColor(252, 252, 252);
                thMovimiento5.PaddingBottom = 3f;

                PdfPCell thMovimiento6 = new PdfPCell(new Phrase("Moneda de Pago", Font_Head_Table));
                thMovimiento6.UseVariableBorders = true;
                thMovimiento6.HorizontalAlignment = Element.ALIGN_CENTER;
                thMovimiento6.BackgroundColor = new BaseColor(241, 241, 241);
                thMovimiento6.BorderColorTop = new BaseColor(241, 241, 241);
                thMovimiento6.BorderColorBottom = new BaseColor(230, 230, 230);
                thMovimiento6.BorderColorLeft = new BaseColor(252, 252, 252);
                thMovimiento6.BorderColorRight = new BaseColor(252, 252, 252);
                thMovimiento6.PaddingBottom = 3f;

                PdfPCell thMovimiento7 = new PdfPCell(new Phrase("Signo", Font_Head_Table));
                thMovimiento7.UseVariableBorders = true;
                thMovimiento7.HorizontalAlignment = Element.ALIGN_CENTER;
                thMovimiento7.BackgroundColor = new BaseColor(241, 241, 241);
                thMovimiento7.BorderColorTop = new BaseColor(241, 241, 241);
                thMovimiento7.BorderColorBottom = new BaseColor(230, 230, 230);
                thMovimiento7.BorderColorLeft = new BaseColor(252, 252, 252);
                thMovimiento7.BorderColorRight = new BaseColor(252, 252, 252);
                thMovimiento7.PaddingBottom = 3f;

                PdfPCell thMovimiento8 = new PdfPCell(new Phrase("Importa", Font_Head_Table));
                thMovimiento8.UseVariableBorders = true;
                thMovimiento8.HorizontalAlignment = Element.ALIGN_CENTER;
                thMovimiento8.BackgroundColor = new BaseColor(241, 241, 241);
                thMovimiento8.BorderColorTop = new BaseColor(241, 241, 241);
                thMovimiento8.BorderColorBottom = new BaseColor(230, 230, 230);
                thMovimiento8.BorderColorLeft = new BaseColor(252, 252, 252);
                thMovimiento8.BorderColorRight = new BaseColor(241, 241, 241);
                thMovimiento8.PaddingBottom = 3f;

                tMovimientos.AddCell(thMovimiento1);
                tMovimientos.AddCell(thMovimiento2);
                tMovimientos.AddCell(thMovimiento3);
                tMovimientos.AddCell(thMovimiento4);
                tMovimientos.AddCell(thMovimiento5);
                tMovimientos.AddCell(thMovimiento6);
                tMovimientos.AddCell(thMovimiento7);
                tMovimientos.AddCell(thMovimiento8);


                if (loencuenta.Count == 0)
                {
                    PdfPCell ttbContenidoCuenta = new PdfPCell(new Phrase("No existen productos", Font_Content_Table));
                    ttbContenidoCuenta.Colspan = 8;
                    ttbContenidoCuenta.UseVariableBorders = true;
                    ttbContenidoCuenta.Border = 0;
                    ttbContenidoCuenta.BackgroundColor = new BaseColor(254, 248, 243);
                    ttbContenidoCuenta.HorizontalAlignment = Element.ALIGN_CENTER;
                    ttbContenidoCuenta.PaddingBottom = 5f;
                    tMovimientos.AddCell(ttbContenidoCuenta);
                }
                else
                {

                    /*Arma el Cuerpo - Movimientos*/
                    foreach (enCuenta reg in loencuenta)
                    {
                        PdfPCell ttbMovimiento = new PdfPCell(new Phrase(reg.sFechaMovimiento, Font_Content_Table));
                        ttbMovimiento.UseVariableBorders = true;
                        ttbMovimiento.Border = 0;
                        ttbMovimiento.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbMovimiento.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbMovimiento.PaddingBottom = 5f;

                        PdfPCell ttbMovimiento2 = new PdfPCell(new Phrase(reg.nNumeroTransaccion.ToString(), Font_Content_Table));
                        ttbMovimiento2.UseVariableBorders = true;
                        ttbMovimiento2.Border = 0;
                        ttbMovimiento2.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbMovimiento2.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbMovimiento2.PaddingBottom = 5f;

                        PdfPCell ttbMovimiento3 = new PdfPCell(new Phrase(reg.sDescripcionMovimiento, Font_Content_Table));
                        ttbMovimiento3.UseVariableBorders = true;
                        ttbMovimiento3.Border = 0;
                        ttbMovimiento3.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbMovimiento3.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbMovimiento3.PaddingBottom = 5f;

                        PdfPCell ttbMovimiento4 = new PdfPCell(new Phrase(reg.sCanal, Font_Content_Table));
                        ttbMovimiento4.UseVariableBorders = true;
                        ttbMovimiento4.Border = 0;
                        ttbMovimiento4.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbMovimiento4.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbMovimiento4.PaddingBottom = 5f;

                        PdfPCell ttbMovimiento5 = new PdfPCell(new Phrase(reg.sMontoMovimiento, Font_Content_Table));
                        ttbMovimiento5.UseVariableBorders = true;
                        ttbMovimiento5.Border = 0;
                        ttbMovimiento5.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbMovimiento5.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ttbMovimiento5.PaddingBottom = 5f;

                        PdfPCell ttbMovimiento6 = new PdfPCell(new Phrase(reg.sTipoMoneda, Font_Content_Table));
                        ttbMovimiento6.UseVariableBorders = true;
                        ttbMovimiento6.Border = 0;
                        ttbMovimiento6.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbMovimiento6.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ttbMovimiento6.PaddingBottom = 5f;

                        PdfPCell ttbMovimiento7 = new PdfPCell(new Phrase(reg.sSigno, Font_Content_Table));
                        ttbMovimiento7.UseVariableBorders = true;
                        ttbMovimiento7.Border = 0;
                        ttbMovimiento7.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbMovimiento7.HorizontalAlignment = Element.ALIGN_CENTER;
                        ttbMovimiento7.PaddingBottom = 5f;

                        PdfPCell ttbMovimiento8 = new PdfPCell(new Phrase(reg.sSaldoContable, Font_Content_Table));
                        ttbMovimiento8.UseVariableBorders = true;
                        ttbMovimiento8.Border = 0;
                        ttbMovimiento8.BackgroundColor = new BaseColor(254, 248, 243);
                        ttbMovimiento8.HorizontalAlignment = Element.ALIGN_RIGHT;
                        ttbMovimiento8.PaddingBottom = 5f;

                        tMovimientos.AddCell(ttbMovimiento);
                        tMovimientos.AddCell(ttbMovimiento2);
                        tMovimientos.AddCell(ttbMovimiento3);
                        tMovimientos.AddCell(ttbMovimiento4);
                        tMovimientos.AddCell(ttbMovimiento5);
                        tMovimientos.AddCell(ttbMovimiento6);
                        tMovimientos.AddCell(ttbMovimiento7);
                        tMovimientos.AddCell(ttbMovimiento8);

                    }
                }

                PdfPTable tContenidoMovimientos = new PdfPTable(1);
                PdfPCell tcContenidoMovimiento = new PdfPCell(tMovimientos);
                tcContenidoMovimiento.HorizontalAlignment = Element.ALIGN_LEFT;
                tcContenidoMovimiento.Border = 0;
                tcContenidoMovimiento.PaddingLeft = 20;
                tContenidoMovimientos.AddCell(tcContenidoMovimiento);
                tContenidoMovimientos.TotalWidth = document.PageSize.Width - 20;
                tContenidoMovimientos.LockedWidth = true;

                //Guarda informacion en el documento.
                document.Add(TitleDatosCliente);
                document.Add(dDatosCliente);
                document.Add(TitleDetalleCuenta);
                document.Add(tContenidoDetalle);
                document.Add(TitleMovimientos);
                document.Add(tContenidoMovimientos);

            }
            catch (Exception ex)
            {
                utlLog.toWrite(utlConstante.ModuloBancaWEB, utlConstante.LogNamespace_WEB, this.GetType().Name.ToString(), MethodBase.GetCurrentMethod().Name, utlConstante.LogTipoError, ex.StackTrace.ToString(),
                    ex.Message.ToString());
                throw ex;
            }
        }
 
    }
}