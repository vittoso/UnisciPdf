using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnisciPdf.BusinessLogic
{
   public  class PdfService
   {

       public void  MergePdf(List<string> fullPathFiles, string destinationFilePath, Rectangle pageSize)
       {
           Rectangle r = pageSize;
           using (FileStream stream = new FileStream(destinationFilePath, FileMode.Create))
           using (Document doc = new Document())
           using (PdfWriter writer = PdfWriter.GetInstance(doc, stream))
           {
               doc.Open();
               PdfContentByte cb = writer.DirectContent;
               PdfReader reader = null;
               PdfImportedPage page = null;

               //fixed typo
               fullPathFiles.ForEach(file =>
               {
                   reader = new PdfReader(file);

                   for (int i = 0; i < reader.NumberOfPages; i++)
                   {
                       doc.NewPage();
                       page = writer.GetImportedPage(reader, i + 1);
                       //   cb.AddTemplate(page, r.Width, r.Height, false);
                       if (r != null)
                       {

                       doc.SetPageSize(r);
                       
                       }

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

               doc.Close();

           }
       }
    }
}
