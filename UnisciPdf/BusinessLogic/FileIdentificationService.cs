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

                int? guessedNumber = TryGuessAnOrderByFileName(filename);

                list.Add(new FileAndOrder { Number = guessedNumber ?? number, FileFullPath = filePath });
            }
        }

        private int? TryGuessAnOrderByFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || fileName.Length < 5)
                return null;

            string filenameLower = fileName.ToLower();

            if (filenameLower.StartsWith("p"))
            {
                int number = 0;
                int idx = 1;
                while(char.IsNumber(filenameLower[idx]))
                {
                    number = number * 10 + Convert.ToInt32(""+filenameLower[idx]);
                    ++idx;
                }

                if (number != 0)
                    return number;
            }

            return null;
        }
    }
}
