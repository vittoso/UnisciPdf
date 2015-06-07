using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharp.Pdf;
using System.Collections.Generic;
using PdfSharp.Pdf.IO;

namespace UnisciPdf.Test
{
    [TestClass]
    public class PdfSharpTest
    {
        [TestMethod]
        [TestCategory("ManualTest")]
        public void Merge()
        {
            string file1 = @"C:\Users\Vittorio\Desktop\test\MBp1mg15r1.pdf";
            string file2 = @"C:\Users\Vittorio\Desktop\test\MBp4mg15r1.pdf";
            List<string> files = new List<string>() { file1, file2 };
            // Open the output document
            PdfDocument outputDocument = new PdfDocument();

            // Iterate files
            foreach (string file in files)
            {
                // Open the document to import pages from it.
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfPage page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }

            // Save the document...
            string filename = @"C:\Users\Vittorio\Desktop\test\ConcatenatedDocument1.pdf";
            outputDocument.Save(filename);
        }
    }
}
