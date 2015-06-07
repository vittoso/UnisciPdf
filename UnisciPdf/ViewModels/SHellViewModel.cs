using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnisciPdf.ViewModels
{
    public class ShellViewModel : PropertyChangedBase , IHaveDisplayName
    {

        public string DisplayName
        {
            get
            {
                return Properties.Resources.AppTitle;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Reset()
        {


        }

        public void CreateFile()
        {

        }
    }
}
