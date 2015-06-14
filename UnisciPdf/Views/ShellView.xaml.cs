using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnisciPdf.ViewModels;

namespace UnisciPdf.Views
{
    /// <summary>
    /// Logica di interazione per ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
        }



        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog d = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
           d.ShowDialog(this);
            
            if (d.SelectedPath != null)
            { // this brakes MVVM a little.... find another way as soon I dig more of Caliburn.Micro
                ShellViewModel vm = this.DataContext as ShellViewModel;
                vm.DestinationFilePath = d.SelectedPath;
            }
        }
    }
}
