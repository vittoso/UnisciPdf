using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnisciPdf.Model
{
    public class FileAndOrder : PropertyChangedBase
    {
        private string fileFullPath = null;
        public string FileFullPath
        {
            get { return fileFullPath; }
            set
            {
                if (fileFullPath != value)
                {
                    fileFullPath = value;
                    NotifyOfPropertyChange(() => this.FileFullPath);
                }
            }
        }

        public string FileName
        {
            get { return fileFullPath != null ? System.IO.Path.GetFileName(fileFullPath) : null; }
        }

        private int number = 1;
        public int Number
        {
            get { return number; }
            set
            {
                if (number != value)
                {
                    number = value;
                    NotifyOfPropertyChange(() => this.Number);
                }
            }
        }
    }
}
