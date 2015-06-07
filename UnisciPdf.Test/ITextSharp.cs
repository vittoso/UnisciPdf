using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace UnisciPdf.Test
{
    [TestClass]
    public class ITextSharp
    {
        [TestMethod]
        public void Merge_Copy()
        {
            string file1 = @"C:\Users\Vittorio\Desktop\test\MBp1mg15r1.pdf";
            string file2 = @"C:\Users\Vittorio\Desktop\test\MBp4mg15r1.pdf";
            List<string> files = new List<string>() { file1, file2 };
            string filename = @"C:\Users\Vittorio\Desktop\test\ConcatenatedDocument1.pdf";

            Rectangle r  = new Rectangle(210, 297);
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (Document doc = new Document(PageSize.A4))
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
                doc.Open();

                PdfReader reader = null;
                PdfImportedPage page = null;
            
                //fixed typo
                files.ForEach(file =>
                {
                    reader = new PdfReader(file);

                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        page = pdf.GetImportedPage(reader, i + 1);
                        pdf.AddPage(page);
                        pdf.SetPageSize(PageSize.A4);
                    }

                    pdf.FreeReader(reader);
                    reader.Close();
                });

                doc.SetPageSize(r);
            }
        }

        [TestMethod]
        public void Merge()
        {
            string file1 = @"C:\Users\Vittorio\Desktop\test\MBp1mg15r1.pdf";
            string file2 = @"C:\Users\Vittorio\Desktop\test\MBp4mg15r1.pdf";
            List<string> files = new List<string>() { file1, file2 };
            string filename = @"C:\Users\Vittorio\Desktop\test\ConcatenatedDocument1.pdf";

            Rectangle r = new Rectangle(210, 297);
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            using (Document doc = new Document(PageSize.A4))
            using (PdfWriter writer = PdfWriter.GetInstance(doc, stream))
            {
                doc.Open();
                PdfContentByte cb = writer.DirectContent;
                PdfReader reader = null;
                PdfImportedPage page = null;

                //fixed typo
                files.ForEach(file =>
                {
                    reader = new PdfReader(file);

                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        doc.NewPage();
                        doc.SetPageSize(PageSize.A4);
                        page = writer.GetImportedPage(reader, i+1);
                     //   cb.AddTemplate(page, r.Width, r.Height, false);

                        var widthFactor = doc.PageSize.Width / page.Width;
                        var heightFactor = doc.PageSize.Height / page.Height;
                        var factor = Math.Min(widthFactor, heightFactor);

                        var offsetX = (doc.PageSize.Width - (page.Width * factor)) / 2;
                        var offsetY = (doc.PageSize.Height - (page.Height * factor)) / 2;
                        cb.AddTemplate(page, factor, 0, 0, factor, offsetX, offsetY);
                    }

                    writer.FreeReader(reader);
                    reader.Close();

                });

               // doc.SetPageSize(r);
                doc.Close();
                
            }

            //PdfReader reader = new PdfReader(pdfIn);
            //Document doc = new Document(PageSize.LEGAL, 0, 0, 0, 0);
            //PdfWriter writer = PdfWriter.getInstance(doc, new FileOutputStream(pdfOut));
            //doc.open();
            //PdfContentByte cb = writer.getDirectContent();
            //for (int i = 1; i <= reader.getNumberOfPages(); i++)
            //{
            //    doc.newPage();
            //    PdfImportedPage page = writer.getImportedPage(reader, i);
            //    cb.addTemplate(page, scale, 0, 0, scale, x, y);
            //}
            //doc.close();
        }
    }
}
