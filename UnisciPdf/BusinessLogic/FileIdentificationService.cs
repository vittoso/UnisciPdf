using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnisciPdf.Model;

namespace UnisciPdf.BusinessLogic
{
   public class FileIdentificationService
    {
       protected static string[] SUPPORTED_EXTENSIONS = { ".pdf" };

       public void CheckFileAndAdd(string filePath, Collection<FileAndOrder> list)
       {
           if (list == null)
               list = new Collection<FileAndOrder>();

           var ext = Path.GetExtension(filePath);
           var filename = Path.GetFileName(filePath);


           if (SUPPORTED_EXTENSIONS.Select(e => e.ToLower()).Any(e => e == ext.ToLower()) && !list.Any(f => f.FileFullPath.ToLower() == filePath))
           {
               int number = list.Any() ? list.Max(x => x.Number) + 1 : 1;
               list.Add(new FileAndOrder { Number = number, FileFullPath = filePath });
           }
       }
    }
}
